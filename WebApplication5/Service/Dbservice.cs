using Microsoft.Data.SqlClient;
using WebApplication5.DTO;
using WebApplication5.Exception;

namespace WebApplication5.Service;

public interface IDbService
{
    Task<string> AddPrescriptionAsync(AddPrescriptionRequest request);
    
    Task<PatientDetailsDto> GetPatientDetailsAsync(int idPatient);
}

public class DbService(IConfiguration config) : IDbService
{
    public async Task<string> AddPrescriptionAsync(AddPrescriptionRequest request)
    {
        var connectionString = config.GetConnectionString("Default");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            int patientId = request.Patient.IdPatient;

            var checkPatientCmd = new SqlCommand("SELECT COUNT(1) FROM Patient WHERE IdPatient = @IdPatient", connection, transaction);
            checkPatientCmd.Parameters.AddWithValue("@IdPatient", patientId);

            var exists = Convert.ToInt32(await checkPatientCmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var insertPatientCmd = new SqlCommand(
                    @"INSERT INTO Patient (IdPatient, FirstName, LastName, Birthdate) 
                      VALUES (@Id, @FirstName, @LastName, @Birthdate)", connection, transaction);

                insertPatientCmd.Parameters.AddWithValue("@Id", patientId);
                insertPatientCmd.Parameters.AddWithValue("@FirstName", request.Patient.FirstName);
                insertPatientCmd.Parameters.AddWithValue("@LastName", request.Patient.LastName);
                insertPatientCmd.Parameters.AddWithValue("@Birthdate", request.Patient.Birthdate);

                await insertPatientCmd.ExecuteNonQueryAsync();
            }
            
            if (request.Medicaments.Count > 10)
                throw new System.Exception("Max 10 medicaments allowed.");

            foreach (var med in request.Medicaments)
            {
                var medCmd = new SqlCommand("SELECT COUNT(1) FROM Medicament WHERE IdMedicament = @Id", connection, transaction);
                medCmd.Parameters.AddWithValue("@Id", med.IdMedicament);

                var existsMed = Convert.ToInt32(await medCmd.ExecuteScalarAsync()) > 0;
                if (!existsMed)
                    throw new System.Exception($"Medicament ID {med.IdMedicament} does not exist.");
            }

            if (request.DueDate < request.Date)
                throw new System.Exception("DueDate must be greater than or equal to Date.");
            
            var prescriptionCmd = new SqlCommand(
                @"INSERT INTO Prescriptions (Date, DueDate, IdPatient, IdDoctor)
                  VALUES (@Date, @DueDate, @IdPatient, @IdDoctor);
                  SELECT SCOPE_IDENTITY();", connection, transaction);

            prescriptionCmd.Parameters.AddWithValue("@Date", request.Date);
            prescriptionCmd.Parameters.AddWithValue("@DueDate", request.DueDate);
            prescriptionCmd.Parameters.AddWithValue("@IdPatient", patientId);
            prescriptionCmd.Parameters.AddWithValue("@IdDoctor", 1); 

            var prescriptionId = Convert.ToInt32(await prescriptionCmd.ExecuteScalarAsync());
            
            foreach (var med in request.Medicaments)
            {
                var pmCmd = new SqlCommand(
                    @"INSERT INTO PrescriptionMedicaments (IdPrescription, IdMedicament, Dose, Details)
                      VALUES (@PrescId, @MedId, @Dose, @Details)", connection, transaction);

                pmCmd.Parameters.AddWithValue("@PrescId", prescriptionId);
                pmCmd.Parameters.AddWithValue("@MedId", med.IdMedicament);
                pmCmd.Parameters.AddWithValue("@Dose", med.Dose);
                pmCmd.Parameters.AddWithValue("@Details", med.Description);

                await pmCmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return $"Prescription created with ID: {prescriptionId}";
        }
        catch (System.Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<PatientDetailsDto> GetPatientDetailsAsync(int idPatient)
    {
        var connectionString = config.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT p.IdPatient, p.FirstName,
                   pr.IdPrescription, pr.Date, pr.DueDate,
                   m.IdMedicament, m.Name, pm.Dose, pm.Details,
                   d.IdDoctor, d.FirstName AS DoctorFirstName
            FROM Patient p
            JOIN Prescriptions pr ON pr.IdPatient = p.IdPatient
            JOIN PrescriptionMedicaments pm ON pm.IdPrescription = pr.IdPrescription
            JOIN Medicament m ON m.IdMedicament = pm.IdMedicament
            JOIN Doctor d ON d.IdDoctor = pr.IdDoctor
            WHERE p.IdPatient = @Id
            ORDER BY pr.DueDate
        ", connection);

        command.Parameters.AddWithValue("@Id", idPatient);

        var reader = await command.ExecuteReaderAsync();

        PatientDetailsDto? patient = null;
        var prescriptionsMap = new Dictionary<int, PrescriptionDto>();

        while (await reader.ReadAsync())
        {
            if (patient == null)
            {
                patient = new PatientDetailsDto
                {
                    IdPatient = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                };
            }

            int idPrescription = reader.GetInt32(2);
            if (!prescriptionsMap.TryGetValue(idPrescription, out var prescription))
            {
                prescription = new PrescriptionDto
                {
                    IdPrescription = idPrescription,
                    Date = reader.GetDateTime(3),
                    DueDate = reader.GetDateTime(4),
                    Doctor = new DoctorDto
                    {
                        IdDoctor = reader.GetInt32(9),
                        FirstName = reader.GetString(10)
                    }
                };
                prescriptionsMap[idPrescription] = prescription;
                patient.Prescriptions.Add(prescription);
            }

            prescription.Medicaments.Add(new MedicamentDetailsDto
            {
                IdMedicament = reader.GetInt32(5),
                Name = reader.GetString(6),
                Dose = reader.GetInt32(7),
                Description = reader.GetString(8)
            });
        }

        return patient ?? throw new NotFoundException("Patient not found");
    }
}
