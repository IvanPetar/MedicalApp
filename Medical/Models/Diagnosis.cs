using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Models
{
    [Table("diagnoses")]
    public class Diagnosis
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("appointment_id")]
        public long AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;

        [Required, Column("code"), StringLength(20)]
        public string Code { get; set; } = null!; //ICD code

        [Required, Column("description"), StringLength(500)]
        public string Description { get; set; } = null!;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
