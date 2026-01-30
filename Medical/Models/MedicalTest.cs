using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public enum TestStatus
    {
        Scheduled = 1,
        Done = 2,
        Cancelled = 3
    }

    [Table("medical_tests")]
    public class MedicalTest
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }


        [Column("patient_id")]
        public long PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;


        [Column("doctor_id")]
        public long DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [Column("type")]
        public TestType Type { get; set; }

        [Column("scheduled_at")]
        public DateTimeOffset ScheduledAt { get; set; }

        [Column("status")]
        public TestStatus Status { get; set; } = TestStatus.Scheduled;

        [Column("result")]
        public string? Result { get; set; }
    }
}
