using AddisEats.OrderBook.Models;
using AddisEats.OrderBook.Services;

var shiro = new MenuItem(1, "Shiro", "Stew", 120.50m);
var doroWot = new MenuItem(2, "Doro Wot", "Stew", 250.00m);
var chechebsa = new MenuItem(3, "Chechebsa", "Breakfast", 95.00m);
var tibs = new MenuItem(4, "Tibs", "Main", 320.00m);

var orderBook = new OrderBook();
var firstOrder = new Order("Mekdes", "Bole", new[]
{
    new OrderLine(shiro, 2),
    new OrderLine(chechebsa, 1),
    new OrderLine(tibs, 1)
});

var orders = new[]
{
    firstOrder,
    new Order("Dawit", "Bole", new[] { new OrderLine(doroWot, 1), new OrderLine(shiro, 1) }),
    new Order("Sara", "Kazanchis", new[] { new OrderLine(tibs, 2) }),
    new Order("Abel", "Bole", new[] { new OrderLine(chechebsa, 3) }),
    new Order("Hana", "Kazanchis", new[] { new OrderLine(shiro, 1) }),
    new Order("Yonas", "Bole", new[] { new OrderLine(doroWot, 2) })
};

orderBook.Add(orders[0]);
orderBook.Add(orders[1]);
orderBook.Add(orders[2]);
orderBook.Add(orders[3]);
orderBook.Add(orders[4]);
orderBook.Add(orders[5]);

Console.WriteLine($"Customer: {firstOrder.Customer}");
Console.WriteLine($"Area: {firstOrder.Area}");
Console.WriteLine("Items:");
Console.WriteLine(string.Join(
    Environment.NewLine,
    firstOrder.Lines.Select(line =>
        $"- {line.MenuItem.Name}: {line.Quantity} x {line.MenuItem.Price:0.00} ETB")));
Console.WriteLine($"Total: {firstOrder.Total:0.00} ETB");

Console.WriteLine();
Console.WriteLine("Bole orders by total:");
Console.WriteLine(string.Join(
    Environment.NewLine,
    orderBook.GetOrdersForArea("Bole").Select(order =>
        $"- {order.Customer}: {order.Total:0.00} ETB")));
Console.WriteLine($"Revenue: {orderBook.GetTotalRevenue():0.00} ETB");
Console.WriteLine($"Busiest category: {orderBook.GetBusiestCategory()}");

var pageOne = orderBook.GetPage(1);
var pageTwo = orderBook.GetPage(2);
Console.WriteLine($"Page 1: {string.Join(", ", pageOne.Select(order => order.Customer))}");
Console.WriteLine($"Page 2: {string.Join(", ", pageTwo.Select(order => order.Customer))}");

if (new OrderBook().GetBusiestCategory() is not null)
{
    throw new InvalidOperationException("An empty order book should not have a busiest category.");
}

if (pageOne.Select(order => order.Customer).Intersect(pageTwo.Select(order => order.Customer)).Any())
{
    throw new InvalidOperationException("Pagination must not repeat orders across pages.");
}

if (pageOne.Count != 5 || pageTwo.Count != 1)
{
    throw new InvalidOperationException("The order book should return five orders per page.");
}

var invalidPriceRejected = false;
try
{
    _ = new MenuItem(99, "Invalid", "Test", 0m);
}
catch (ArgumentException)
{
    invalidPriceRejected = true;
}

var invalidQuantityRejected = false;
try
{
    _ = new OrderLine(shiro, 0);
}
catch (ArgumentException)
{
    invalidQuantityRejected = true;
}

if (!invalidPriceRejected || !invalidQuantityRejected)
{
    throw new InvalidOperationException("Menu price and order quantity validation failed.");
}

Console.WriteLine("Validation and pagination checks passed.");
