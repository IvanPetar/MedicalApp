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
    [Table("specialties")]
    [Index(nameof(Name), IsUnique = true)]
    public class Specialty
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, Column("name"), StringLength(80)]
        public string Name { get; set; } = null!;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
