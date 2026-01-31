using Medical.Models;
using Medical.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class PatientMenu
    {
        private readonly PatientService _patients;

        public PatientMenu(PatientService patients)
        {
            _patients = patients;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- PACIJENTI ---");
                Console.WriteLine("1) Dodaj pacijenta");
                Console.WriteLine("2) Lista pacijenata (search)");
                Console.WriteLine("3) Detalji pacijenta");
                Console.WriteLine("4) Azuriraj adresu pacijenta");
                Console.WriteLine("5) Obrisi pacijenta");
                Console.WriteLine("0) Nazad");

                switch (Console.ReadLine())
                {
                    case "1": await AddPatient(); break;
                    case "2": await ListPatients(); break;
                    case "3": await PatientDetails(); break;
                    case "4": await UpdateAddress(); break;
                    case "5": await DeletePatient(); break;
                    case "0": return;
                }
            }
        }

        private async Task AddPatient()
        {
            try
            {
                Console.Write("Ime: ");
                var fn = ReadRequired();

                Console.Write("Prezime: ");
                var ln = ReadRequired();

                Console.Write("OIB (11 brojeva): ");
                var oib = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(oib) || oib.Length != 11 || !oib.All(char.IsDigit))
                {
                    Console.WriteLine("OIB mora imati tocno 11 brojeva!");
                    return;
                }

                Console.Write("Datum rodenja (dd.MM.yyyy ili ENTER za preskociti): ");
                DateTime? dob = null;
                var dobInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(dobInput))
                {
                    if (!DateOnly.TryParseExact(dobInput, "dd.MM.yyyy", null,
                        System.Globalization.DateTimeStyles.None, out var parsed))
                    {
                        Console.WriteLine("Neispravan datum!");
                        return;
                    }
                    dob = parsed.ToDateTime(TimeOnly.MinValue);
                }

                Console.Write("Spol (M/Z): ");
                var sexInput = Console.ReadLine()?.Trim().ToUpper();
                if (sexInput != "M" && sexInput != "Z")
                {
                    Console.WriteLine("Spol mora biti M ili Z!");
                    return;
                }

                Console.Write("Grad: ");
                var city = ReadRequired();

                Console.Write("Ulica: ");
                var street = ReadRequired();

                Console.Write("Broj: ");
                var no = ReadRequired();

                var id = await _patients.CreateAsync(new Patient
                {
                    FirstName = fn,
                    LastName = ln,
                    Oib = oib,
                    DateOfBirth = dob,
                    Sex = sexInput[0],
                    City = city,
                    Street = street,
                    StreetNo = no
                });

                Console.WriteLine($"Pacijent uspjesno dodan! ID={id}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }
        }
        private async Task ListPatients()
        {
            Console.Write("Search (ime/prezime/oib) ili ENTER: ");
            var s = Console.ReadLine();

            var list = await _patients.GetAllAsync(s);
            if (!list.Any())
            {
                Console.WriteLine("Nema pacijenata.");
                return;
            }

            foreach (var p in list)
                Console.WriteLine($"{p.Id}: {p.FirstName} {p.LastName} ({p.Oib})");
        }

        private async Task PatientDetails()
        {
            if (!TryReadLong("Patient ID: ", out var id))
                return;

            var p = await _patients.GetDetailsAsync(id);
            if (p == null)
            {
                Console.WriteLine("Pacijent ne postoji.");
                return;
            }

            Console.WriteLine($"\n{p.FirstName} {p.LastName} | OIB: {p.Oib}");
            Console.WriteLine($"Adresa: {p.Street} {p.StreetNo}, {p.City}");

            if (p.Appointments.Any())
            {
                Console.WriteLine("\nPregledi:");
                foreach (var a in p.Appointments.OrderByDescending(x => x.ScheduledAt))
                    Console.WriteLine($" - {a.ScheduledAt:dd.MM.yyyy HH:mm} | {a.Type} | Dr {a.Doctor.LastName ?? "Nepoznat"}");
            }

            if (p.Prescriptions.Any())
            {
                Console.WriteLine("\nRecepti:");
                foreach (var pr in p.Prescriptions.OrderByDescending(x => x.IssuedAt))
                {
                    Console.WriteLine($" - {pr.IssuedAt:dd.MM.yyyy HH:mm} | Dr {pr.Doctor.LastName}");
                    foreach (var it in pr.Items)
                        Console.WriteLine($"    * {it.Medication.Name} {it.Medication.StrengthMg}mg | {it.Dosage}");
                }
            }
            if (p.MedicalTests.Any())
            {
                Console.WriteLine("\nSpecijalisticke Pretrage:");
                foreach (var t in p.MedicalTests.OrderByDescending(x => x.ScheduledAt))
                {
                    Console.WriteLine(
                        $" - {t.ScheduledAt:dd.MM.yyyy HH:mm} | {t.Type} | Status: {t.Status} | Dr {t.Doctor.LastName}"
                    );

                    if (!string.IsNullOrWhiteSpace(t.Result))
                        Console.WriteLine($"    Rezultat: {t.Result}");
                }
            }
        }
        private async Task UpdateAddress()
        {
            if (!TryReadLong("Patient ID: ", out var id))
                return;

            Console.Write("Novi grad: ");
            var city = ReadRequired();

            Console.Write("Nova ulica: ");
            var street = ReadRequired();

            Console.Write("Novi broj: ");
            var no = ReadRequired();

            await _patients.UpdateAddressAsync(id, city, street, no);
            Console.WriteLine("Adresa azurirana.");
        }
        private async Task DeletePatient()
        {
            if (!TryReadLong("Patient ID: ", out var id))
                return;

            Console.Write("Potvrda brisanja (DA/NE): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "DA")
            {
                Console.WriteLine("Brisanje prekinuto.");
                return;
            }

            await _patients.DeleteAsync(id);
            Console.WriteLine("Pacijent obrisan.");
        }
        private static bool TryReadLong(string prompt, out long result)
        {
            Console.Write(prompt);
            return long.TryParse(Console.ReadLine(), out result);
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
