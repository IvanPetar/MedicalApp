using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medical.Models;
using Microsoft.EntityFrameworkCore;

namespace Medical.Data
{
    public class MedicalContext : DbContext
    {
        public MedicalContext(DbContextOptions<MedicalContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
        public DbSet<Medication> Medications => Set<Medication>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<MedicalTest> MedicalTests => Set<MedicalTest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Constraints
            modelBuilder.Entity<Patient>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Diagnosis>()
                .Property(d => d.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Prescription>()
                .Property(p => p.IssuedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<MedicalTest>()
                .Property(t => t.OrderedAt)
                .HasDefaultValueSql("now()");

            // UNIQUE for Doctor.LicenseNo
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNo)
                .IsUnique();

            // N:N (PrescriptionItem) - forbid duplicates of (PrescriptionId, MedicationId)
            modelBuilder.Entity<PrescriptionItem>()
                .HasIndex(x => new { x.PrescriptionId, x.MedicationId })
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
