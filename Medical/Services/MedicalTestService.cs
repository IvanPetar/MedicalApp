using Medical.Data;
using Medical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Services
{
    public class MedicalTestService
    {
        private readonly MedicalContext _db;
        public MedicalTestService(MedicalContext db) => _db = db;

        public async Task<long> OrderAsync(long appointmentId, TestType type)
        {
            var t = new MedicalTest { AppointmentId = appointmentId, Type = type };
            _db.MedicalTests.Add(t);
            await _db.SaveChangesAsync();
            return t.Id;
        }
    }
}
