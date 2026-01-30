using Medical.Models;
using Medical.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class PrescriptionMenu
    {
        private readonly PrescriptionService _prescriptions;

        public PrescriptionMenu(PrescriptionService prescriptions)
        {
            _prescriptions = prescriptions;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n--- RECEPTI ---");
                Console.WriteLine("1) Kreiraj recept");
                Console.WriteLine("2) Dodaj lijek na recept");
                Console.WriteLine("3) Lista recepata po pacijentu");
                Console.WriteLine("4) Obrisi stavku recepta");
                Console.WriteLine("5) Obrisi recept");
                Console.WriteLine("0) Nazad");

                switch (Console.ReadLine())
                {
                    case "1": await Safe(CreatePrescription); break;
                    case "2": await Safe(AddItem); break;
                    case "3": await Safe(ListByPatient); break;
                    case "4": await Safe(DeleteItem); break;
                    case "5": await Safe(DeletePrescription); break;
                    case "0": return;
                }
            }
        }

      
        private async Task CreatePrescription()
        {
            if (!TryReadLong("Patient ID: ", out var pid)) return;
            if (!TryReadLong("Doctor ID: ", out var did)) return;

            var id = await _prescriptions.CreatePrescriptionAsync(pid, did);
            Console.WriteLine($"Recept kreiran. ID={id}");
        }

        private async Task AddItem()
        {
            if (!TryReadLong("Prescription ID: ", out var prId)) return;

            Console.Write("Naziv lijeka: ");
            var name = ReadRequired();

            Console.Write("Forma (ENTER za skip): ");
            var form = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(form)) form = null;

            Console.Write("Jacina mg (ENTER za skip): ");
            var smg = Console.ReadLine()?.Trim();
            decimal? strength = null;
            if (!string.IsNullOrWhiteSpace(smg) && decimal.TryParse(smg, out var s))
                strength = s;

            var medId = await _prescriptions.EnsureMedicationAsync(
                name,
                form,
                strength
            );

            Console.Write("Doziranje (npr. 3x dnevno): ");
            var dosage = ReadRequired();

            if (!TryReadInt("Koliko dana: ", out var days) || days <= 0)
            {
                Console.WriteLine("Broj dana mora biti veci od 0!");
                return;
            }

            await _prescriptions.AddItemAsync(prId, medId, dosage, days);
            Console.WriteLine("Lijek dodan na recept.");
        }

        private async Task ListByPatient()
        {
            if (!TryReadLong("Patient ID: ", out var pid)) return;

            var list = await _prescriptions.GetByPatientAsync(pid);
            if (!list.Any())
            {
                Console.WriteLine("Pacijent nema recepata.");
                return;
            }

            foreach (var pr in list)
            {
                Console.WriteLine(
                    $"\nRecept {pr.Id} | {pr.IssuedAt:dd.MM.yyyy HH:mm} | " +
                    $"Dr {pr.Doctor.LastName}"
                );

                foreach (var it in pr.Items)
                {
                    Console.WriteLine(
                        $"  Stavka {it.Id}: {it.Medication.Name} " +
                        $"{it.Medication.StrengthMg}mg | {it.Dosage} | {it.Days} dana"
                    );
                }
            }
        }

        private async Task DeleteItem()
        {
            if (!TryReadLong("Item ID: ", out var id)) return;

            Console.Write("Potvrda brisanja (DA/NE): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "DA")
            {
                Console.WriteLine("Brisanje prekinuto.");
                return;
            }

            await _prescriptions.DeleteItemAsync(id);
            Console.WriteLine("Stavka recepta obrisana.");
        }

        private async Task DeletePrescription()
        {
            if (!TryReadLong("Prescription ID: ", out var id)) return;

            Console.Write("Potvrda brisanja (DA/NE): ");
            if (Console.ReadLine()?.Trim().ToUpper() != "DA")
            {
                Console.WriteLine("Brisanje prekinuto.");
                return;
            }

            await _prescriptions.DeletePrescriptionAsync(id);
            Console.WriteLine("Recept obrisan.");
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

        private static string ReadRequired()
        {
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception("Polje ne smije biti prazno!");
            return s;
        }
    }
}
