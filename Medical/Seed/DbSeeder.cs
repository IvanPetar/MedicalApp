using Medical.Data;
using Medical.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(MedicalContext context)
        {
            await context.Database.EnsureCreatedAsync();


            if (!await context.Specialties.AnyAsync())
            {
                var specialties = new[]
                {
                    new Specialty { Name = "Opća medicina" },
                    new Specialty { Name = "Kardiologija" },
                    new Specialty { Name = "Radiologija" }
                };

                context.Specialties.AddRange(specialties);
                await context.SaveChangesAsync();
            }

            if (!await context.Doctors.AnyAsync())
            {
                var general = await context.Specialties
                    .FirstAsync(s => s.Name == "Opća medicina");

                var cardio = await context.Specialties
                    .FirstAsync(s => s.Name == "Kardiologija");

                var radio = await context.Specialties
                    .FirstAsync(s => s.Name == "Radiologija");

                var doctors = new[]
                {
                    new Doctor
                    {
                        FirstName = "Ivan",
                        LastName = "Ivić",
                        LicenseNo = "DOC-001",
                        SpecialtyId = general.Id
                    },
                    new Doctor
                    {
                        FirstName = "Marko",
                        LastName = "Marić",
                        LicenseNo = "DOC-002",
                        SpecialtyId = cardio.Id
                    },
                    new Doctor
                    {
                        FirstName = "Ana",
                        LastName = "Anić",
                        LicenseNo = "DOC-003",
                        SpecialtyId = radio.Id
                    }
                };

                context.Doctors.AddRange(doctors);
                await context.SaveChangesAsync();
            }
        }
    }
}
