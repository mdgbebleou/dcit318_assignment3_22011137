using System;
using System.Collections.Generic;
using System.Linq;

// =============================
// 1. Marker Interface: IInventoryItem
// =============================
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// =============================
// 2. ElectronicItem Class
// =============================
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
        return $"[ID={Id}] {Name} | Brand: {Brand} | Qty: {Quantity} | Warranty: {WarrantyMonths} months";
    }
}

// =============================
// 3. GroceryItem Class
// =============================
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
        return $"[ID={Id}] {Name} | Qty: {Quantity} | Expires: {ExpiryDate:yyyy-MM-dd}";
    }
}

// =============================
// 4. Custom Exceptions
// =============================
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

// =============================
// 5. Generic Inventory Repository
// =============================
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists: {item.Name}");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} was not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Cannot remove: Item with ID {id} not found.");
        }
    }

    public List<T> GetAllItems()
    {
        return _items.Values.ToList();
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }

        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        _items[id] = _items[id] with { Quantity = newQuantity };
    }
}

// =============================
// 6. WareHouseManager Class
// =============================
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    // Seed initial data
    public void SeedData()
    {
        // Electronics
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "Tablet", 15, "Apple", 18));

        // Groceries
        _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(3)));
        _groceries.AddItem(new GroceryItem(103, "Eggs", 100, DateTime.Now.AddDays(14)));
    }

    // Generic method to print all items in any repository
    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine("\n--- All Items ---");
        var items = repo.GetAllItems();
        if (!items.Any())
        {
            Console.WriteLine("No items in inventory.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    // Increase stock (generic)
    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($" Stock increased: {quantity} added to ID {id}. New quantity: {item.Quantity + quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error increasing stock: {ex.Message}");
        }
    }

    // Remove item by ID (generic)
    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($" Item ID {id} removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error removing item: {ex.Message}");
        }
    }
}

// =============================
// 7. Main Application
// =============================
class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        // Print all items
        manager.PrintAllItems(manager._groceries);
        manager.PrintAllItems(manager._electronics);

        // Test error scenarios with try-catch
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("🧪 Testing Error Scenarios");
        Console.WriteLine(new string('=', 50));

        // 1. Add a duplicate item
        Console.WriteLine("\n Attempting to add duplicate item (ID 1)...");
        try
        {
            manager._electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 24));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Caught: {ex.Message}");
        }

        // 2. Remove a non-existent item
        Console.WriteLine("\n Attempting to remove non-existent item (ID 999)...");
        manager.RemoveItemById(manager._groceries, 999);

        // 3. Update with invalid quantity
        Console.WriteLine("\n Attempting to update quantity to -5 for ID 101...");
        try
        {
            manager._groceries.UpdateQuantity(101, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Caught: {ex.Message}");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Caught: {ex.Message}");
        }

        Console.WriteLine("\n✅ All tests completed.");
    }
}