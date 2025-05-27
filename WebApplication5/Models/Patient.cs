using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models;

[Table("Patient")]
public class Patient
{
    [Key]
    public int IdPatient { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    public DateTime Birthdate { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = null!;
}