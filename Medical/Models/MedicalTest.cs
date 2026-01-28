using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Models
{
    public enum TestType
    {
        CT = 1,
        MR = 2,
        UZV = 3,
        RTG = 4,
        EKG = 5,
        Lab = 6
    }

    [Table("medical_tests")]
    public class MedicalTest
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("appointment_id")]
        public long AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;

        [Column("type")]
        public TestType Type { get; set; }

        [Column("ordered_at")]
        public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("result")]
        public string? Result { get; set; }
    }
}
