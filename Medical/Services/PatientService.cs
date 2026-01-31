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
    public class PatientService
    {
        private readonly MedicalContext _db;
        public PatientService(MedicalContext db) => _db = db;

        public async Task<long> CreateAsync(Patient p)
        {
            _db.Patients.Add(p);
            await _db.SaveChangesAsync();
            return p.Id;
        }

        public Task<List<Patient>> GetAllAsync(string? search = null)
        {
            var q = _db.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x =>
                    x.FirstName.ToLower().Contains(search.ToLower()) ||
                    x.LastName.ToLower().Contains(search.ToLower()) ||
                    x.Oib.Contains(search));

            return q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
        }

        public Task<Patient?> GetDetailsAsync(long id)
            => _db.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.Specialty)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Items)
                        .ThenInclude(i => i.Medication)
                .Include(m => m.MedicalTests)
                    .ThenInclude(t => t.Doctor)
                        .ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task UpdateAddressAsync(long id, string city, string street, string no)
        {
            var p = await _db.Patients.FindAsync(id) ?? throw new Exception("Patient not found");
            p.City = city;
            p.Street = street;
            p.StreetNo = no;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var p = await _db.Patients.FindAsync(id) ?? throw new Exception("Patient not found");
            _db.Patients.Remove(p);
            await _db.SaveChangesAsync();
        }
    }
}
