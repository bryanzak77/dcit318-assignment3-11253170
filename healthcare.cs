using System;
using System.Collections.Generic;
using System.Linq;

// a. Generic Repository
public class Repository<T> where T : class
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        T? itemToRemove = _items.FirstOrDefault(predicate);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
            return true;
        }
        return false;
    }
}

// b. Patient Class
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
        return $"Id: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// c. Prescription Class
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
        return $"Id: {Id}, Medication: {MedicationName}, Issued: {DateIssued:d}";
    }
}

// g. HealthSystemApp class
public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
    private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add 2-3 Patient objects
        _patientRepo.Add(new Patient(1, "Ama Addo", 45, "Female"));
        _patientRepo.Add(new Patient(2, "Kwaku Asamoah", 32, "Male"));
        _patientRepo.Add(new Patient(3, "John Asante", 67, "Male"));

        // Add 4-5 Prescription objects
        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(102, 2, "Ibuprofen", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(103, 1, "Lisinopril", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Atorvastatin", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(105, 2, "Metformin", DateTime.Now.AddDays(-7)));
    }

    public void BuildPrescriptionMap()
    {
        var allPrescriptions = _prescriptionRepo.GetAll();
        foreach (var prescription in allPrescriptions)
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("--- All Patients ---");
        var patients = _patientRepo.GetAll();
        foreach (var patient in patients)
        {
            Console.WriteLine(patient);
        }
        Console.WriteLine("--------------------");
        Console.WriteLine();
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        _prescriptionMap.TryGetValue(patientId, out var prescriptions);
        return prescriptions ?? new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient != null)
        {
            Console.WriteLine($"--- Prescriptions for Patient: {patient.Name} (Id: {patientId}) ---");
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Any())
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"Patient with Id {patientId} not found.");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        int patientIdToSearch = 1;
        app.PrintPrescriptionsForPatient(patientIdToSearch);

        patientIdToSearch = 2;
        app.PrintPrescriptionsForPatient(patientIdToSearch);

        patientIdToSearch = 99; // Non-existent patient
        app.PrintPrescriptionsForPatient(patientIdToSearch);
    }
}