using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medical.Models
{
    [Table("patients")]
    [Index(nameof(Oib), IsUnique = true)]
    public class Patient
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required, Column("first_name"), StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, Column("last_name"), StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required, Column("oib"), StringLength(11, MinimumLength = 11)]
        public string Oib { get; set; } = null!; // UNIQUE

        [Column("date_of_birth", TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Required, Column("sex")]
        public char Sex { get; set; }

        [Required, Column("city"), StringLength(80)]
        public string City { get; set; } = null!;

        [Required, Column("street"), StringLength(120)]
        public string Street { get; set; } = null!;

        [Required, Column("street_no"), StringLength(10)]
        public string StreetNo { get; set; } = null!;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigations
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
