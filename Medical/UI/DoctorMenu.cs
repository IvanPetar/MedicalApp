using Medical.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class DoctorMenu
    {
        private readonly DoctorService _doctors;

        public DoctorMenu(DoctorService doctors)
        {
            _doctors = doctors;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- DOKTORI ---");
                Console.WriteLine("1) Lista doktora");
                Console.WriteLine("0) Nazad");

                switch (Console.ReadLine())
                {
                    case "1":
                        await List();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Nepoznata opcija.");
                        break;
                }
            }
        }

        private async Task List()
        {
            try
            {
                var list = await _doctors.GetAllAsync();

                if (!list.Any())
                {
                    Console.WriteLine("Nema doktora.");
                    return;
                }

                Console.WriteLine("\nID | Ime i prezime | Specijalizacija | Licenca");
                Console.WriteLine("------------------------------------------------");

                foreach (var d in list)
                {
                    Console.WriteLine(
                        $"{d.Id} | Dr {d.FirstName} {d.LastName} | " +
                        $"{d.Specialty.Name} | {d.LicenseNo}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }
    }
}
