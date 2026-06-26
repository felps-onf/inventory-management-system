using System.Globalization;

var products = new List<(string Name, decimal Price, int Stock)>();

RunSelfTests();

while (true)
{
    Console.Clear();
    Console.WriteLine("=== Inventory Management System ===");
    Console.WriteLine("1. View all products");
    Console.WriteLine("2. Add a product");
    Console.WriteLine("3. Update stock");
    Console.WriteLine("4. Remove a product");
    Console.WriteLine("5. Run built-in validation tests");
    Console.WriteLine("6. Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ShowProducts(products);
            Pause();
            break;
        case "2":
            AddProduct(products);
            Pause();
            break;
        case "3":
            UpdateStock(products);
            Pause();
            break;
        case "4":
            RemoveProduct(products);
            Pause();
            break;
        case "5":
            RunSelfTests();
            Pause();
            break;
        case "6":
            Console.WriteLine("Goodbye!");
            return;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            Pause();
            break;
    }
}

static void ShowProducts(List<(string Name, decimal Price, int Stock)> products)
{
    Console.WriteLine();
    Console.WriteLine("Current inventory:");
    if (products.Count == 0)
    {
        Console.WriteLine("No products available.");
        return;
    }

    foreach (var product in products)
    {
        Console.WriteLine($"- {product.Name}: Price {product.Price.ToString("C", CultureInfo.CurrentCulture)}, Stock {product.Stock}");
    }
}

static void AddProduct(List<(string Name, decimal Price, int Stock)> products)
{
    Console.Write("Product name: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Price: ");
    if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
    {
        Console.WriteLine("Please enter a valid non-negative price.");
        return;
    }

    Console.Write("Initial stock quantity: ");
    if (!int.TryParse(Console.ReadLine(), out var stock) || stock < 0)
    {
        Console.WriteLine("Please enter a valid non-negative stock quantity.");
        return;
    }

    if (!AddProductToList(products, name, price, stock))
    {
        Console.WriteLine("Product name already exists or the values are invalid.");
        return;
    }

    Console.WriteLine($"Added {name} to inventory.");
}

static void UpdateStock(List<(string Name, decimal Price, int Stock)> products)
{
    Console.Write("Product name: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Quantity change (+ for restock, - for sale): ");
    if (!int.TryParse(Console.ReadLine(), out var change))
    {
        Console.WriteLine("Please enter a valid integer.");
        return;
    }

    if (!UpdateStockInList(products, name, change))
    {
        Console.WriteLine("Product not found or stock would become negative.");
        return;
    }

    Console.WriteLine("Stock updated successfully.");
}

static void RemoveProduct(List<(string Name, decimal Price, int Stock)> products)
{
    Console.Write("Product name: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    if (!RemoveProductFromList(products, name))
    {
        Console.WriteLine("Product not found.");
        return;
    }

    Console.WriteLine($"Removed {name} from inventory.");
}

static bool AddProductToList(List<(string Name, decimal Price, int Stock)> products, string name, decimal price, int stock)
{
    if (string.IsNullOrWhiteSpace(name) || price < 0 || stock < 0)
    {
        return false;
    }

    if (products.Any(product => product.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
    {
        return false;
    }

    products.Add((name, price, stock));
    return true;
}

static bool UpdateStockInList(List<(string Name, decimal Price, int Stock)> products, string name, int change)
{
    for (var i = 0; i < products.Count; i++)
    {
        if (!products[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        var newStock = products[i].Stock + change;
        if (newStock < 0)
        {
            return false;
        }

        products[i] = (products[i].Name, products[i].Price, newStock);
        return true;
    }

    return false;
}

static bool RemoveProductFromList(List<(string Name, decimal Price, int Stock)> products, string name)
{
    for (var i = 0; i < products.Count; i++)
    {
        if (products[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            products.RemoveAt(i);
            return true;
        }
    }

    return false;
}

static void RunSelfTests()
{
    Console.WriteLine("Running built-in validation tests...");

    var testProducts = new List<(string Name, decimal Price, int Stock)>();

    Assert(AddProductToList(testProducts, "Keyboard", 49.99m, 5), "Add valid product");
    Assert(!AddProductToList(testProducts, "Keyboard", 29.99m, 2), "Reject duplicate product");
    Assert(!AddProductToList(testProducts, "", 10m, 1), "Reject empty name");
    Assert(!AddProductToList(testProducts, "Mouse", -1m, 3), "Reject negative price");
    Assert(!AddProductToList(testProducts, "Mouse", 10m, -1), "Reject negative stock");
    Assert(UpdateStockInList(testProducts, "Keyboard", -2), "Decrease stock");
    Assert(!UpdateStockInList(testProducts, "Keyboard", -10), "Reject stock below zero");
    Assert(RemoveProductFromList(testProducts, "Keyboard"), "Remove existing product");
    Assert(testProducts.Count == 0, "Inventory becomes empty after removal");

    Console.WriteLine("All built-in validation tests passed.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException($"Test failed: {message}");
    }
}

static void Pause()
{
    Console.WriteLine();
    Console.WriteLine("Press Enter to continue...");
    Console.ReadLine();
}
