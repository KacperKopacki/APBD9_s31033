using Microsoft.EntityFrameworkCore;
using WebApplication5.Models;

namespace WebApplication5.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
        
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var doctor = new Doctor
        {
            IdDoctor = 1,
            FirstName = "Gregory",
            LastName = "House",
            Email = "house@hospital.com"
        };

        var patient = new Patient
        {
            IdPatient = 1,
            FirstName = "Lisa",
            LastName = "Cuddy",
            Birthdate = new DateTime(1980, 1, 1)
        };

        var medicament = new Medicament
        {
            IdMedicament = 1,
            Name = "Ibuprofen",
            Description = "Painkiller",
            Type = "Tablet"
        };

        var prescription = new Prescription
        {
            IdPrescription = 1,
            Date = new DateTime(2024, 5, 27),
            DueDate = new DateTime(2024, 6, 5),
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient
        };

        var prescriptionMedicament = new PrescriptionMedicament
        {
            IdPrescription = prescription.IdPrescription,
            IdMedicament = medicament.IdMedicament,
            Dose = 2,
            Details = "Twice a day"
        };

        modelBuilder.Entity<Doctor>().HasData(doctor);
        modelBuilder.Entity<Patient>().HasData(patient);
        modelBuilder.Entity<Medicament>().HasData(medicament);
        modelBuilder.Entity<Prescription>().HasData(prescription);
        modelBuilder.Entity<PrescriptionMedicament>().HasData(prescriptionMedicament);
    }
}