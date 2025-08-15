using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// =============================
// 1. Immutable InventoryItem Record (implements IInventoryEntity)
// =============================
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// =============================
// 2. Marker Interface: IInventoryEntity
// =============================
public interface IInventoryEntity
{
    int Id { get; }
}

// =============================
// 3. Generic InventoryLogger<T>
// =============================
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
        return new List<T>(_log); // Return a copy
    }

    // Save all items to file in JSON format
    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);

            using (var writer = new StreamWriter(_filePath))
            {
                writer.Write(json);
            }

            Console.WriteLine($"✅ Data saved to {_filePath}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access denied: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"❌ Directory not found: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ File I/O error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error while saving: {ex.Message}");
        }
    }

    // Load items from JSON file
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"⚠️ File not found: {_filePath}. Starting with empty log.");
                return;
            }

            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("⚠️ File is empty. No data to load.");
                    return;
                }

                var loadedItems = JsonSerializer.Deserialize<List<T>>(json);
                if (loadedItems != null)
                {
                    _log.Clear();
                    _log.AddRange(loadedItems);
                    Console.WriteLine($"✅ Loaded {_log.Count} items from {_filePath}");
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access denied: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ File I/O error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"❌ JSON format error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error while loading: {ex.Message}");
        }
    }
}

// =============================
// 4. InventoryApp - Integration Layer
// =============================
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;
    private const string DataFile = "inventory.json";

    public InventoryApp()
    {
        _logger = new InventoryLogger<InventoryItem>(DataFile);
    }

    // Seed sample data
    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-10)));
        _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(3, "Keyboard", 25, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(4, "Monitor", 8, DateTime.Now.AddDays(-1)));
        _logger.Add(new InventoryItem(5, "USB Cable", 100, DateTime.Now));

        Console.WriteLine($"✅ Seeded {_logger.GetAll().Count} sample items.");
    }

    // Save data to file
    public void SaveData()
    {
        _logger.SaveToFile();
    }

    // Load data from file
    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    // Print all items
    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("📭 No items to display.");
            return;
        }

        Console.WriteLine("\n--- Inventory Items ---");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd HH:mm}");
        }
        Console.WriteLine($"Total Items: {items.Count}");
    }
}

// =============================
// 5. Main Application
// =============================
class Program
{
    static void Main()
    {
        var app = new InventoryApp();

        Console.WriteLine("📦 Inventory Management System\n");

        // 1. Seed data
        app.SeedSampleData();

        // 2. Save to file
        app.SaveData();

        // 3. Simulate new session: clear memory
        Console.WriteLine("\n🧹 Simulating new session... (memory cleared)\n");
        // Note: In real app, process would exit and restart

        // 4. Reload from file
        app.LoadData();

        // 5. Print loaded data
        app.PrintAllItems();

        Console.WriteLine("\n✅ Inventory system demo complete.");
    }
}