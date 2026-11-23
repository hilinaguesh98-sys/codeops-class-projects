namespace AddisEats.OrderBook.Models;

public sealed record OrderLine
{
    public MenuItem MenuItem { get; }

    public int Quantity { get; }

    public OrderLine(MenuItem menuItem, int quantity)
    {
        if (menuItem is null)
        {
            throw new ArgumentException("An order line must include a menu item.", nameof(menuItem));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Order line quantity must be greater than zero.", nameof(quantity));
        }

        MenuItem = menuItem;
        Quantity = quantity;
    }
}
