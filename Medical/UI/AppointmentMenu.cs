using Medical.Models;
using Medical.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class AppointmentMenu
    {
        private readonly AppointmentService _appointments;

        public AppointmentMenu(AppointmentService appointments)
        {
            _appointments = appointments;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- PREGLEDI ---");
                Console.WriteLine("1) Zakazi pregled");
                Console.WriteLine("2) Pretrazi preglede");
                Console.WriteLine("3) Azuriraj pregled");
                Console.WriteLine("4) Obrisi pregled");
                Console.WriteLine("5) Dodaj dijagnozu");
                Console.WriteLine("0) Nazad");

                switch (Console.ReadLine())
                {
                    case "1": await Safe(Create); break;
                    case "2": await Safe(Search); break;
                    case "3": await Safe(Update); break;
                    case "4": await Safe(Delete); break;
                    case "5": await Safe(AddDiagnosis); break;
                    case "0": return;
                }
            }
        }

        private async Task Create()
        {
            if (!TryReadLong("Patient ID: ", out var pid)) return;
            if (!TryReadLong("Doctor ID: ", out var did)) return;
            if (!TryReadDateTime("Datum i vrijeme (yyyy-MM-dd HH:mm): ", out var dt)) return;

            Console.WriteLine("Tip pregleda:");
            Console.WriteLine("  1 = General");
            Console.WriteLine("  2 = Specialist");
            Console.WriteLine("  3 = Control");

            if (!TryReadInt("Odaberi tip: ", out var t) || t < 1 || t > 3)
            {
                Console.WriteLine("Neispravan tip pregleda.");
                return;
            }

            var id = await _appointments.CreateAsync(new Appointment
            {
                PatientId = pid,
                DoctorId = did,
                ScheduledAt = new DateTimeOffset(dt, TimeSpan.Zero),
                Type = (AppointmentType)t
            });

            Console.WriteLine($"Pregled zakazan. ID={id}");
        }

        private async Task Search()
        {
            Console.Write("Patient ID (ENTER skip): ");
            var ps = Console.ReadLine();
            long? pid = string.IsNullOrWhiteSpace(ps) ? null : long.Parse(ps);

            Console.Write("Doctor ID (ENTER skip): ");
            var ds = Console.ReadLine();
            long? did = string.IsNullOrWhiteSpace(ds) ? null : long.Parse(ds);

            var list = await _appointments.SearchAsync(pid, did);

            if (!list.Any())
            {
                Console.WriteLine("Nema pregleda.");
                return;
            }

            foreach (var a in list)
            {
                Console.WriteLine(
                    $"{a.Id}: {a.ScheduledAt:dd.MM.yyyy HH:mm} | " +
                    $"{a.Patient.LastName} | Dr {a.Doctor.LastName} | {a.Type}"
                );
            }
        }

        private async Task Update()
        {
            if (!TryReadLong("Appointment ID: ", out var id)) return;
            if (!TryReadDateTime("Novi datum i vrijeme (yyyy-MM-dd HH:mm): ", out var dt)) return;

            Console.WriteLine("Novi tip pregleda:");
            Console.WriteLine("  1 = General");
            Console.WriteLine("  2 = Specialist");
            Console.WriteLine("  3 = Control");

            if (!TryReadInt("Tip: ", out var t) || t < 1 || t > 3)
            {
                Console.WriteLine("Neispravan tip.");
                return;
            }

            await _appointments.UpdateAsync(
                id,
                new DateTimeOffset(dt, TimeSpan.Zero),
                (AppointmentType)t
            );

            Console.WriteLine("Pregled azuriran.");
        }

        private async Task Delete()
        {
            if (!TryReadLong("Appointment ID: ", out var id)) return;

            Console.Write("Potvrda brisanja (DA/NE): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "DA")
            {
                Console.WriteLine("Brisanje prekinuto.");
                return;
            }

            await _appointments.DeleteAsync(id);
            Console.WriteLine("Pregled obrisan (otkazan).");
        }

     
        private async Task AddDiagnosis()
        {
            if (!TryReadLong("Appointment ID: ", out var aid)) return;

            Console.Write("Sifra dijagnoze: ");
            var code = ReadRequired();

            Console.Write("Opis: ");
            var desc = ReadRequired();

            await _appointments.AddDiagnosisAsync(aid, code, desc);
            Console.WriteLine("Dijagnoza dodana.");
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

        private static string ReadRequired()
        {
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception("Polje ne smije biti prazno!");
            return s;
        }
    }
}
