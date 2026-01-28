using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Models
{
    [Table("medications")]
    [Index(nameof(Name), IsUnique = true)]
    public class Medication
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required, Column("name"), StringLength(120)]
        public string Name { get; set; } = null!;

        [Column("form"), StringLength(40)]
        public string? Form { get; set; } // tablet, syrup...

        [Column("strength_mg")]
        public decimal? StrengthMg { get; set; } // DECIMAL

        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
