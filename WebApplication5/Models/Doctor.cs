using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models;

[Table("Doctor")]
public class Doctor
{
    [Key]
    public int IdDoctor { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(50)]
    public string LastName { get; set; }
    [MaxLength(50)]
    public string Email { get; set; }
    
    public virtual ICollection<Prescription> Prescriptions { get; set; } = null!;
}