using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Data
{
    public class MedicalContextFactory
         : IDesignTimeDbContextFactory<MedicalContext>
    {
        public MedicalContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedicalContext>();

            optionsBuilder.UseNpgsql(
                 "Host=localhost;Port=5433;Username=admin;Password=SQL;Database=medicalApp;"
            );

            return new MedicalContext(optionsBuilder.Options);
        }
    }
}
