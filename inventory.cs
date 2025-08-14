using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventorySystem
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_log);
                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    sw.WriteLine(jsonString);
                }
                Console.WriteLine($"Data successfully saved to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File {_filePath} not found. Starting with an empty log.");
                _log = new List<T>();
                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(_filePath))
                {
                    var jsonString = sr.ReadToEnd();
                    _log = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
                }
                Console.WriteLine($"Data successfully loaded from {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading: {ex.Message}");
                _log = new List<T>();
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("inventory.json");

        public void SeedSampleData()
        {
            Console.WriteLine("Seeding sample data...");
            _logger.Add(new InventoryItem(1, "Laptop", 15, DateTime.Now.AddDays(-30)));
            _logger.Add(new InventoryItem(2, "Mouse", 150, DateTime.Now.AddDays(-10)));
            _logger.Add(new InventoryItem(3, "Keyboard", 75, DateTime.Now.AddDays(-5)));
            _logger.Add(new InventoryItem(4, "Monitor", 30, DateTime.Now.AddDays(-25)));
        }

        public void SaveData()
        {
            Console.WriteLine("\nSaving current inventory to file...");
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            Console.WriteLine("\nLoading inventory data from file...");
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            Console.WriteLine("\n--- Current Inventory ---");
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("No items found in the inventory.");
            }
            else
            {
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }
            }
            Console.WriteLine("--------------------------");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new InventoryApp();

            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine("\n--- Simulating new session ---");
            var newAppInstance = new InventoryApp();

            newAppInstance.LoadData();
            newAppInstance.PrintAllItems();
        }
    }
}