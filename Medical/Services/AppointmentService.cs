using Medical.Data;
using Medical.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Services
{
    public class AppointmentService
    {
        private readonly MedicalContext _db;
        public AppointmentService(MedicalContext db) => _db = db;

        public async Task<long> CreateAsync(Appointment a)
        {
            _db.Appointments.Add(a);
            await _db.SaveChangesAsync();
            return a.Id;
        }

        public Task<List<Appointment>> SearchAsync(
            long? patientId = null,
            long? doctorId = null,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null)
        {
            var q = _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .AsQueryable();

            if (patientId.HasValue)
                q = q.Where(x => x.PatientId == patientId);

            if (doctorId.HasValue)
                q = q.Where(x => x.DoctorId == doctorId);

            if (from.HasValue)
                q = q.Where(x => x.ScheduledAt >= from.Value);

            if (to.HasValue)
                q = q.Where(x => x.ScheduledAt <= to.Value);

            return q
                .OrderByDescending(x => x.ScheduledAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(long id, DateTimeOffset when, AppointmentType type)
        {
            var a = await _db.Appointments.FindAsync(id)
                ?? throw new Exception("Pregled ne postoji");

            a.ScheduledAt = when;
            a.Type = type;

            await _db.SaveChangesAsync();
        }

    
        public async Task DeleteAsync(long id)
        {
            var a = await _db.Appointments.FindAsync(id)
                ?? throw new Exception("Pregled ne postoji");

            _db.Appointments.Remove(a);
            await _db.SaveChangesAsync();
        }

        public async Task AddDiagnosisAsync(long appointmentId, string code, string desc)
        {
            _db.Diagnoses.Add(new Diagnosis
            {
                AppointmentId = appointmentId,
                Code = code,
                Description = desc
            });

            await _db.SaveChangesAsync();
        }
    }
}
