using Medical.Data;
using Medical.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Services
{
    public class MedicalTestService
    {
        private readonly MedicalContext _db;
        public MedicalTestService(MedicalContext db) => _db = db;


        public async Task<long> CreateAsync(MedicalTest t)
        {
            _db.MedicalTests.Add(t);
            await _db.SaveChangesAsync();
            return t.Id;
        }

  
        public Task<List<MedicalTest>> GetAllAsync()
            => _db.MedicalTests
                  .Include(t => t.Patient)
                  .Include(t => t.Doctor)
                  .OrderBy(t => t.ScheduledAt)
                  .ToListAsync();

  
        public async Task UpdateAsync(
            long id,
            DateTimeOffset scheduledAt,
            TestStatus status,
            string? result = null)
        {
            var t = await _db.MedicalTests.FindAsync(id)
                ?? throw new Exception("Pretraga ne postoji");

            t.ScheduledAt = scheduledAt;
            t.Status = status;
            t.Result = result;

            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(long id)
        {
            var t = await _db.MedicalTests.FindAsync(id)
                ?? throw new Exception("Pretraga ne postoji");

            _db.MedicalTests.Remove(t);
            await _db.SaveChangesAsync();
        }
    }
}
