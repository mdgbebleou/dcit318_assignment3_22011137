using System;
using System.Collections.Generic;
using System.Linq;

// =============================
// 1. Generic Repository<T> Class
// =============================
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(items);
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// =============================
// 2. Patient Class
// =============================
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient [Id={Id}, Name={Name}, Age={Age}, Gender={Gender}]";
    }
}

// =============================
// 3. Prescription Class
// =============================
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription [Id={Id}, Medication={MedicationName}, Date={DateIssued:yyyy-MM-dd}, PatientId={PatientId}]";
    }
}

// =============================
// 4. HealthSystemApp - Orchestrates the system
// =============================
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Johnson", 34, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Smith", 52, "Male"));
        _patientRepo.Add(new Patient(3, "Carol White", 29, "Female"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Lisinopril", DateTime.Now.AddDays(-8)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Sertraline", DateTime.Now.AddDays(-12)));
        _prescriptionRepo.Add(new Prescription(105, 2, "Atorvastatin", DateTime.Now.AddDays(-3)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n--- All Patients ---");
        var patients = _patientRepo.GetAll();
        if (!patients.Any())
        {
            Console.WriteLine("No patients found.");
            return;
        }

        foreach (var patient in patients)
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine($"\n--- Prescriptions for Patient ID: {patientId} ---");

        if (_prescriptionMap.TryGetValue(patientId, out List<Prescription> prescriptions))
        {
            if (prescriptions.Any())
            {
                foreach (var p in prescriptions)
                {
                    Console.WriteLine(p);
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found.");
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

// =============================
// 5. Main Application Entry Point
// =============================
class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(1);
        app.PrintPrescriptionsForPatient(2);
    }
}