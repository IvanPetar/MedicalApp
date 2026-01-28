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
                var cardiology = new Specialty
                {
                    Name = "Cardiology"
                };

                context.Specialties.Add(cardiology);
                await context.SaveChangesAsync();
            }

            if (!await context.Doctors.AnyAsync())
            {
                var specialty = await context.Specialties.FirstAsync();

                context.Doctors.Add(new Doctor
                {
                    FirstName = "Ivan",
                    LastName = "Ivić",
                    LicenseNo = "DOC-001",
                    SpecialtyId = specialty.Id
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
