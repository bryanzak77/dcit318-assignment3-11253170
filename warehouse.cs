using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
    // a. Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem Class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"[Electronic] Id: {Id}, Name: {Name}, Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";
        }
    }

    // c. GroceryItem Class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"[Grocery] Id: {Id}, Name: {Name}, Qty: {Quantity}, Expires: {ExpiryDate:d}";
        }
    }

    // e. Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot remove. Item with ID {id} not found.");
            }
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot update. Item with ID {id} not found.");
            }
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }
            _items[id].Quantity = newQuantity;
        }
    }

    // f. WareHouseManager Class
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 50, "Dell", 12));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 120, "Samsung", 24));
            _groceries.AddItem(new GroceryItem(101, "Milk", 200, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 150, DateTime.Now.AddDays(3)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Increased stock for item ID {id}. New quantity: {item.Quantity + quantity}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Successfully removed item with ID {id}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void Run()
        {
            Console.WriteLine("--- Seeding Data ---");
            SeedData();
            Console.WriteLine();

            Console.WriteLine("--- All Grocery Items ---");
            PrintAllItems(_groceries);
            Console.WriteLine();

            Console.WriteLine("--- All Electronic Items ---");
            PrintAllItems(_electronics);
            Console.WriteLine();

            Console.WriteLine("--- Demonstration of Exception Handling ---");

            try
            {
                Console.WriteLine("Attempting to add a duplicate grocery item (ID 101)...");
                _groceries.AddItem(new GroceryItem(101, "Yogurt", 50, DateTime.Now.AddDays(10)));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
            }

            Console.WriteLine();

            try
            {
                Console.WriteLine("Attempting to remove a non-existent electronic item (ID 99)...");
                _electronics.RemoveItem(99);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
            }

            Console.WriteLine();

            try
            {
                Console.WriteLine("Attempting to update grocery item ID 102 with an invalid quantity (-10)...");
                _groceries.UpdateQuantity(102, -10);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
            }

            Console.WriteLine();

            Console.WriteLine("--- Final State of Inventory ---");
            Console.WriteLine("Groceries:");
            PrintAllItems(_groceries);
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new WareHouseManager();
            app.Run();
        }
    }
}