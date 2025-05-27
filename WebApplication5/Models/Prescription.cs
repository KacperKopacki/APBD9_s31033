using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models;

public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [ForeignKey("Patient")]
    public int IdPatient { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    
    [ForeignKey("Doctor")]
    public int IdDoctor { get; set; }
    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = null!;
}