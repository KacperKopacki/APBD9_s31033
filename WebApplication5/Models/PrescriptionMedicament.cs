using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Models;

[PrimaryKey(nameof(IdPrescription), nameof(IdMedicament))]
public class PrescriptionMedicament
{
    public int IdPrescription { get; set; }
    public int IdMedicament { get; set; }

    public virtual Prescription Prescription { get; set; } = null!;
    public virtual Medicament Medicament { get; set; } = null!;

    public int Dose { get; set; }

    [MaxLength(100)]
    public string? Details { get; set; }
}