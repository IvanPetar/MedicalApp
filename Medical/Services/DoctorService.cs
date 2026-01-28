using Medical.Data;
using Medical.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Services
{
    public class DoctorService
    {
        private readonly MedicalContext _db;
        public DoctorService(MedicalContext db) => _db = db;

        public async Task SeedSpecialtiesIfEmptyAsync()
        {
            if (await _db.Specialties.AnyAsync()) return;

            _db.Specialties.AddRange(
                new Specialty { Name = "Opća medicina" },
                new Specialty { Name = "Kardiologija" },
                new Specialty { Name = "Radiologija" },
                new Specialty { Name = "Ortopedija" }
            );

            await _db.SaveChangesAsync();
        }

        public async Task<long> CreateDoctorAsync(Doctor d)
        {
            _db.Doctors.Add(d);
            await _db.SaveChangesAsync();
            return d.Id;
        }

        public Task<List<Specialty>> GetSpecialtiesAsync()
            => _db.Specialties.OrderBy(x => x.Name).ToListAsync();

        public Task<List<Doctor>> GetAllAsync()
            => _db.Doctors.Include(d => d.Specialty)
                          .OrderBy(d => d.LastName)
                          .ToListAsync();
    }
}
