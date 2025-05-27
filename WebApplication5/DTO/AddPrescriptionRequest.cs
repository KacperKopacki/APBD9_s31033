namespace WebApplication5.DTO;

public class AddPrescriptionRequest
{
    public PatientDto Patient { get; set; } = null!;
    public List<MedicamentDto> Medicaments { get; set; } = new();
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}