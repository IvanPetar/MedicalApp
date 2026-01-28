using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medical.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required, Column("first_name"), StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, Column("last_name"), StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required, Column("license_no"), StringLength(30)]
        public string LicenseNo { get; set; } = null!;

        [Column("specialty_id")]
        public int SpecialtyId { get; set; }

        [ForeignKey(nameof(SpecialtyId))]
        public Specialty Specialty { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
