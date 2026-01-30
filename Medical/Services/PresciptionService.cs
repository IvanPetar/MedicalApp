using Medical.Data;
using Medical.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Services
{
    public class PrescriptionService
    {
        private readonly MedicalContext _db;
        public PrescriptionService(MedicalContext db) => _db = db;

        
        public async Task<long> EnsureMedicationAsync(string name, string? form, decimal? strengthMg)
        {
            var existing = await _db.Medications
                .FirstOrDefaultAsync(m => m.Name == name);

            if (existing != null)
                return existing.Id;

            var med = new Medication
            {
                Name = name,
                Form = form,
                StrengthMg = strengthMg
            };

            _db.Medications.Add(med);
            await _db.SaveChangesAsync();
            return med.Id;
        }

        public async Task<long> CreatePrescriptionAsync(long patientId, long doctorId)
        {
            var p = new Prescription
            {
                PatientId = patientId,
                DoctorId = doctorId
            };

            _db.Prescriptions.Add(p);
            await _db.SaveChangesAsync();
            return p.Id;
        }

        public async Task AddItemAsync(long prescriptionId, long medicationId, string dosage, int days)
        {
            _db.PrescriptionItems.Add(new PrescriptionItem
            {
                PrescriptionId = prescriptionId,
                MedicationId = medicationId,
                Dosage = dosage,
                Days = days
            });

            await _db.SaveChangesAsync();
        }

        public Task<List<Prescription>> GetByPatientAsync(long patientId)
            => _db.Prescriptions
                .Include(p => p.Doctor).ThenInclude(d => d.Specialty)
                .Include(p => p.Items).ThenInclude(i => i.Medication)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.IssuedAt)
                .ToListAsync();

   
        public async Task DeleteItemAsync(long itemId)
        {
            var item = await _db.PrescriptionItems.FindAsync(itemId)
                ?? throw new Exception("Stavka recepta ne postoji");

            _db.PrescriptionItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePrescriptionAsync(long prescriptionId)
        {
            var p = await _db.Prescriptions.FindAsync(prescriptionId)
                ?? throw new Exception("Recept ne postoji");

            _db.Prescriptions.Remove(p);
            await _db.SaveChangesAsync();
        }
    }
}
