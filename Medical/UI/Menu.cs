using Medical.UI;
using System;
using System.Threading.Tasks;

namespace Medical.UI
{
    public class Menu
    {
        private readonly PatientMenu _patients;
        private readonly DoctorMenu _doctors;
        private readonly AppointmentMenu _appointments;
        private readonly PrescriptionMenu _prescriptions;
        private readonly MedicalTestMenu _medicalTests;

        public Menu(
            PatientMenu patients,
            DoctorMenu doctors,
            AppointmentMenu appointments,
            PrescriptionMenu prescriptions,
            MedicalTestMenu medicalTests)
        {
            _patients = patients;
            _doctors = doctors;
            _appointments = appointments;
            _prescriptions = prescriptions;
            _medicalTests = medicalTests;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== MEDICAL APP ===");
                Console.WriteLine("1) Pacijenti");
                Console.WriteLine("2) Doktori");
                Console.WriteLine("3) Pregledi");
                Console.WriteLine("4) Recepti");
                Console.WriteLine("5) Specijalisticke pretrage");
                Console.WriteLine("0) Izlaz");

                switch (Console.ReadLine())
                {
                    case "1":
                        await _patients.RunAsync();
                        break;

                    case "2":
                        await _doctors.RunAsync();
                        break;

                    case "3":
                        await _appointments.RunAsync();
                        break;

                    case "4":
                        await _prescriptions.RunAsync();
                        break;

                    case "5":
                        await _medicalTests.RunAsync();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Nepoznata opcija.");
                        break;
                }
            }
        }
    }
}
