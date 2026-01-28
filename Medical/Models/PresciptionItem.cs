using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Models
{
    [Table("prescription_items")]
    public class PrescriptionItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("prescription_id")]
        public long PrescriptionId { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; } = null!;

        [Column("medication_id")]
        public long MedicationId { get; set; }

        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; } = null!;

        [Required, Column("dosage"), StringLength(100)]
        public string Dosage { get; set; } = null!; // "3x daily"

        [Column("days")]
        public int Days { get; set; } // INT
    }
}
