namespace WebApplication5.DTO;

public class PatientDetailsDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
}