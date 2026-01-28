using Medical.Models;
using Medical.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class Menu
    {
        private readonly PatientService _patients;
        private readonly DoctorService _doctors;
        private readonly AppointmentService _appointments;
        private readonly PrescriptionService _prescriptions;
        private readonly MedicalTestService _tests;

        public Menu(
            PatientService patients,
            DoctorService doctors,
            AppointmentService appointments,
            PrescriptionService prescriptions,
            MedicalTestService tests)
        {
            _patients = patients;
            _doctors = doctors;
            _appointments = appointments;
            _prescriptions = prescriptions;
            _tests = tests;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== MEDICAL APP ===");
                Console.WriteLine("1) Dodaj pacijenta");
                Console.WriteLine("2) Lista pacijenata (search)");
                Console.WriteLine("3) Detalji pacijenta (eager loading)");
                Console.WriteLine("4) Seed specijalizacije + dodaj doktora");
                Console.WriteLine("5) Lista doktora");
                Console.WriteLine("6) Zakazi pregled");
                Console.WriteLine("7) Pretrazi preglede (filter)");
                Console.WriteLine("8) Kreiraj recept + dodaj lijek");
                Console.WriteLine("9) Naruci pretragu (CT/MR/UZV...)");
                Console.WriteLine("0) Izlaz");

                var c = Console.ReadLine();

                try
                {
                    switch (c)
                    {
                        case "1": await AddPatient(); break;
                        case "2": await ListPatients(); break;
                        case "3": await PatientDetails(); break;
                        case "4": await AddDoctor(); break;
                        case "5": await ListDoctors(); break;
                        case "6": await AddAppointment(); break;
                        case "7": await SearchAppointments(); break;
                        case "8": await CreatePrescription(); break;
                        case "9": await OrderTest(); break;
                        case "0": return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GRESKA: {ex.Message}");
                }
            }
        }

        private bool TryReadLong(string prompt, out long result)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (long.TryParse(input, out result))
                return true;

            Console.WriteLine("Unesite ispravan broj!");
            return false;
        }

        private bool TryReadInt(string prompt, out int result)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out result))
                return true;

            Console.WriteLine("Unesite ispravan broj!");
            return false;
        }

        private bool TryReadDateTime(string prompt, out DateTime result)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (DateTime.TryParse(input, out result))
                return true;

            Console.WriteLine("Neispravan format datuma! Koristi yyyy-MM-dd HH:mm");
            return false;
        }

        private async Task AddPatient()
        {
            try
            {
                Console.Write("Ime: ");
                var fn = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(fn))
                {
                    Console.WriteLine("Ime ne smije biti prazno!");
                    return;
                }

                Console.Write("Prezime: ");
                var ln = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(ln))
                {
                    Console.WriteLine("Prezime ne smije biti prazno!");
                    return;
                }

                Console.Write("OIB (11 brojeva): ");
                var oib = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(oib) || oib.Length != 11 || !oib.All(char.IsDigit))
                {
                    Console.WriteLine("OIB mora imati tocno 11 brojeva!");
                    return;
                }

                Console.Write("Datum rodenja (dd.MM.yyyy ili ENTER za preskociti): ");
                var dobInput = Console.ReadLine()?.Trim();
                DateTime? dob = null;
                if (!string.IsNullOrWhiteSpace(dobInput))
                {
                    if (!DateOnly.TryParseExact(dobInput, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        Console.WriteLine("Neispravan format datuma! Koristi dd.MM.yyyy (npr. 15.03.1990)");
                        return;
                    }
                    dob = parsedDate.ToDateTime(TimeOnly.MinValue);
                }

                Console.Write("Spol (M/Z): ");
                var sexInput = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrWhiteSpace(sexInput) || (sexInput != "M" && sexInput != "Z"))
                {
                    Console.WriteLine("Spol mora biti M ili Z!");
                    return;
                }
                char sex = sexInput[0];

                Console.Write("Grad: ");
                var city = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(city))
                {
                    Console.WriteLine("Grad ne smije biti prazan!");
                    return;
                }

                Console.Write("Ulica: ");
                var street = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(street))
                {
                    Console.WriteLine("Ulica ne smije biti prazna!");
                    return;
                }

                Console.Write("Broj: ");
                var no = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(no))
                {
                    Console.WriteLine("Broj ne smije biti prazan!");
                    return;
                }

                var id = await _patients.CreateAsync(new Patient
                {
                    FirstName = fn,
                    LastName = ln,
                    Oib = oib,
                    DateOfBirth = dob,
                    Sex = sex,
                    City = city,
                    Street = street,
                    StreetNo = no
                });

                Console.WriteLine($"Pacijent uspjesno dodan! ID={id}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Greska pri spremanju u bazu:");
                Console.WriteLine($"   {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task ListPatients()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task PatientDetails()
        {
            try
            {
                if (!TryReadLong("Patient ID: ", out var id))
                    return;

                var p = await _patients.GetDetailsAsync(id);
                if (p == null)
                {
                    Console.WriteLine("Pacijent s tim ID-om ne postoji.");
                    return;
                }

                Console.WriteLine($"\n{p.FirstName} {p.LastName} | OIB: {p.Oib}");
                Console.WriteLine($"Adresa: {p.Street} {p.StreetNo}, {p.City}");

                if (p.Appointments.Any())
                {
                    Console.WriteLine("\nPregledi:");
                    foreach (var a in p.Appointments.OrderByDescending(x => x.ScheduledAt))
                        Console.WriteLine($"  - {a.ScheduledAt:dd.MM.yyyy HH:mm} | {a.Type} | Dr: {a.Doctor.LastName} ({a.Doctor.Specialty.Name})");
                }
                else
                {
                    Console.WriteLine("\nNema pregleda.");
                }

                if (p.Prescriptions.Any())
                {
                    Console.WriteLine("\nRecepti:");
                    foreach (var pr in p.Prescriptions.OrderByDescending(x => x.IssuedAt))
                    {
                        Console.WriteLine($"  - {pr.IssuedAt:dd.MM.yyyy HH:mm} | Dr: {pr.Doctor.LastName}");
                        foreach (var it in pr.Items)
                            Console.WriteLine($"     * {it.Medication.Name} {it.Medication.StrengthMg}mg | {it.Dosage} | {it.Days} dana");
                    }
                }
                else
                {
                    Console.WriteLine("\nNema recepta.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task AddDoctor()
        {
            try
            {
                await _doctors.SeedSpecialtiesIfEmptyAsync();

                var specs = await _doctors.GetSpecialtiesAsync();
                Console.WriteLine("Specijalizacije:");
                foreach (var s in specs)
                    Console.WriteLine($"{s.Id}) {s.Name}");

                if (!TryReadInt("Specialty ID: ", out var sid))
                    return;

                Console.Write("Ime: ");
                var fn = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(fn))
                {
                    Console.WriteLine("Ime ne smije biti prazno!");
                    return;
                }

                Console.Write("Prezime: ");
                var ln = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(ln))
                {
                    Console.WriteLine("Prezime ne smije biti prazno!");
                    return;
                }

                Console.Write("Licenca: ");
                var lic = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(lic))
                {
                    Console.WriteLine("Licenca ne smije biti prazna!");
                    return;
                }

                var id = await _doctors.CreateDoctorAsync(new Doctor
                {
                    FirstName = fn,
                    LastName = ln,
                    LicenseNo = lic,
                    SpecialtyId = sid
                });

                Console.WriteLine($"Doktor dodan. ID={id}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Greska pri spremanju u bazu:");
                Console.WriteLine($"   {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task ListDoctors()
        {
            try
            {
                var list = await _doctors.GetAllAsync();

                if (!list.Any())
                {
                    Console.WriteLine("Nema doktora.");
                    return;
                }

                foreach (var d in list)
                    Console.WriteLine($"{d.Id}: Dr {d.FirstName} {d.LastName} | {d.Specialty.Name} | Lic:{d.LicenseNo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task AddAppointment()
        {
            try
            {
                if (!TryReadLong("Patient ID: ", out var pid))
                    return;

                if (!TryReadLong("Doctor ID: ", out var did))
                    return;

                if (!TryReadDateTime("Datum i vrijeme (yyyy-MM-dd HH:mm): ", out var dt))
                    return;

                var when = new DateTimeOffset(dt, TimeSpan.Zero);

                Console.WriteLine("Tip pregleda:");
                Console.WriteLine("  1 = General");
                Console.WriteLine("  2 = Specialist");
                Console.WriteLine("  3 = Control");

                if (!TryReadInt("Odaberi tip: ", out var typeNum) || typeNum < 1 || typeNum > 3)
                {
                    Console.WriteLine("Tip mora biti 1, 2 ili 3!");
                    return;
                }

                var type = (AppointmentType)typeNum;

                var id = await _appointments.CreateAsync(new Appointment
                {
                    PatientId = pid,
                    DoctorId = did,
                    ScheduledAt = when,
                    Type = type
                });

                Console.WriteLine($"Pregled zakazan. ID={id}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Greska pri spremanju:");
                Console.WriteLine($"   {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task SearchAppointments()
        {
            try
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
                    Console.WriteLine($"{a.Id}: {a.ScheduledAt:dd.MM.yyyy HH:mm} | {a.Patient.LastName} | Dr {a.Doctor.LastName} | {a.Type}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task CreatePrescription()
        {
            try
            {
                if (!TryReadLong("Patient ID: ", out var pid))
                    return;

                if (!TryReadLong("Doctor ID: ", out var did))
                    return;

                var prId = await _prescriptions.CreatePrescriptionAsync(pid, did);
                Console.WriteLine($"Recept kreiran ID={prId}");

                Console.Write("Naziv lijeka: ");
                var name = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Naziv lijeka ne smije biti prazan!");
                    return;
                }

                Console.Write("Forma (ENTER za skip): ");
                var form = Console.ReadLine()?.Trim();

                Console.Write("Jacina mg (ENTER za skip): ");
                var smg = Console.ReadLine()?.Trim();
                decimal? strength = null;
                if (!string.IsNullOrWhiteSpace(smg) && decimal.TryParse(smg, out var str))
                    strength = str;

                var medId = await _prescriptions.EnsureMedicationAsync(
                    name,
                    string.IsNullOrWhiteSpace(form) ? null : form,
                    strength
                );

                Console.Write("Doziranje (npr. 3x dnevno): ");
                var dosage = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(dosage))
                {
                    Console.WriteLine("Doziranje ne smije biti prazno!");
                    return;
                }

                if (!TryReadInt("Koliko dana: ", out var days) || days <= 0)
                {
                    Console.WriteLine("Broj dana mora biti veci od 0!");
                    return;
                }

                await _prescriptions.AddItemAsync(prId, medId, dosage, days);
                Console.WriteLine("Lijek dodan na recept.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Greska pri spremanju:");
                Console.WriteLine($"   {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }

        private async Task OrderTest()
        {
            try
            {
                if (!TryReadLong("Appointment ID: ", out var aid))
                    return;

                Console.WriteLine("Tip pretrage:");
                Console.WriteLine("  1 = CT");
                Console.WriteLine("  2 = MR");
                Console.WriteLine("  3 = UZV");
                Console.WriteLine("  4 = RTG");
                Console.WriteLine("  5 = EKG");
                Console.WriteLine("  6 = Lab");

                if (!TryReadInt("Odaberi tip: ", out var typeNum) || typeNum < 1 || typeNum > 6)
                {
                    Console.WriteLine("Tip mora biti izmedu 1 i 6!");
                    return;
                }

                var tt = (TestType)typeNum;

                var id = await _tests.OrderAsync(aid, tt);
                Console.WriteLine($"Pretraga narucena. ID={id}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Greska pri spremanju:");
                Console.WriteLine($"   {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }
        }
    }
}