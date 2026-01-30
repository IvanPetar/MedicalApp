using Medical.Models;
using Medical.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class MedicalTestMenu
    {
        private readonly MedicalTestService _tests;

        public MedicalTestMenu(MedicalTestService tests)
        {
            _tests = tests;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- SPECIJALISTIČKE PRETRAGE ---");
                Console.WriteLine("1) Naruci pretragu");
                Console.WriteLine("2) Lista pretraga");
                Console.WriteLine("3) Azuriraj pretragu");
                Console.WriteLine("4) Obrisi pretragu");
                Console.WriteLine("0) Nazad");

                switch (Console.ReadLine())
                {
                    case "1": await Safe(Create); break;
                    case "2": await Safe(List); break;
                    case "3": await Safe(Update); break;
                    case "4": await Safe(Delete); break;
                    case "0": return;
                }
            }
        }
        private async Task Create()
        {
            if (!TryReadLong("Patient ID: ", out var pid)) return;
            if (!TryReadLong("Doctor ID (specijalist): ", out var did)) return;
            if (!TryReadDateTime("Datum i vrijeme (yyyy-MM-dd HH:mm): ", out var dt)) return;

            Console.WriteLine("Tip pretrage:");
            Console.WriteLine("1 = CT");
            Console.WriteLine("2 = MR");
            Console.WriteLine("3 = UZV");
            Console.WriteLine("4 = RTG");
            Console.WriteLine("5 = EKG");
            Console.WriteLine("6 = Lab");

            if (!TryReadInt("Odaberi tip: ", out var t) || t < 1 || t > 6)
            {
                Console.WriteLine("Neispravan tip.");
                return;
            }

            var id = await _tests.CreateAsync(new MedicalTest
            {
                PatientId = pid,
                DoctorId = did,
                Type = (TestType)t,
                ScheduledAt = new DateTimeOffset(dt, TimeSpan.Zero)
            });

            Console.WriteLine($"Pretraga narucena. ID={id}");
        }

        private async Task List()
        {
            var list = await _tests.GetAllAsync();
            if (!list.Any())
            {
                Console.WriteLine("Nema pretraga.");
                return;
            }

            foreach (var t in list)
            {
                Console.WriteLine(
                    $"{t.Id}: {t.Type} | {t.ScheduledAt:dd.MM.yyyy HH:mm} | " +
                    $"{t.Patient.LastName} | Dr {t.Doctor.LastName} | {t.Status}"
                );
            }
        }

        private async Task Update()
        {
            if (!TryReadLong("Test ID: ", out var id)) return;
            if (!TryReadDateTime("Novi termin (yyyy-MM-dd HH:mm): ", out var dt)) return;

            Console.WriteLine("Status:");
            Console.WriteLine("1 = Scheduled");
            Console.WriteLine("2 = Done");
            Console.WriteLine("3 = Cancelled");

            if (!TryReadInt("Odaberi status: ", out var s) || s < 1 || s > 3)
            {
                Console.WriteLine("Neispravan status.");
                return;
            }

            Console.Write("Rezultat (ENTER za preskociti): ");
            var result = Console.ReadLine();

            await _tests.UpdateAsync(
                id,
                new DateTimeOffset(dt, TimeSpan.Zero),
                (TestStatus)s,
                string.IsNullOrWhiteSpace(result) ? null : result
            );

            Console.WriteLine("Pretraga azurirana.");
        }

        private async Task Delete()
        {
            if (!TryReadLong("Test ID: ", out var id)) return;

            Console.Write("Potvrda brisanja (DA/NE): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "DA")
            {
                Console.WriteLine("Brisanje prekinuto.");
                return;
            }

            await _tests.DeleteAsync(id);
            Console.WriteLine("Pretraga obrisana.");
        }

        private static async Task Safe(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GRESKA: {ex.Message}");
            }
        }

    
        private static bool TryReadLong(string prompt, out long result)
        {
            Console.Write(prompt);
            return long.TryParse(Console.ReadLine(), out result);
        }

        private static bool TryReadInt(string prompt, out int result)
        {
            Console.Write(prompt);
            return int.TryParse(Console.ReadLine(), out result);
        }

        private static bool TryReadDateTime(string prompt, out DateTime result)
        {
            Console.Write(prompt);
            return DateTime.TryParse(Console.ReadLine(), out result);
        }
    }
}
