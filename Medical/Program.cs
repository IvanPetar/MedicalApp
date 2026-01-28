using Medical.Data;
using Medical.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Medical.UI;
using Medical.Seed;

class Program
{
    static async Task Main()
    {
        //Connection string
        var connectionString =
            "Host=localhost;Port=5433;Username=admin;Password=SQL;Database=medicalApp;";

        //DbContext options
        var options = new DbContextOptionsBuilder<MedicalContext>()
            .UseNpgsql(connectionString)
            .Options;

        //Context
        using var context = new MedicalContext(options);

        //migrations
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Migration failed:");
            Console.WriteLine(ex.Message);
            throw;
        }

        await DbSeeder.SeedAsync(context);

        //services
        var patientService = new PatientService(context);
        var doctorService = new DoctorService(context);
        var appointmentService = new AppointmentService(context);
        var prescriptionService = new PrescriptionService(context);
        var medicalTestService = new MedicalTestService(context);

        var menu = new Menu(
            patientService,
            doctorService,
            appointmentService,
            prescriptionService,
            medicalTestService
        );

        await menu.RunAsync();
    }
}