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
    public enum AppointmentType
    {
        General = 1,
        Specialist = 2,
        Control = 3
    }

    [Table("appointments")]
    [Index(nameof(ScheduledAt))]
    public class Appointment
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

        [Column("scheduled_at")]
        public DateTimeOffset ScheduledAt { get; set; }

        [Column("type")]
        public AppointmentType Type { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<MedicalTest> Tests { get; set; } = new List<MedicalTest>();
    }
}
