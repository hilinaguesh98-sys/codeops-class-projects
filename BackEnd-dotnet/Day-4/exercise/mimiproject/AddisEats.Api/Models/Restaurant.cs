using AddisEats.OrderBook.Models;

namespace AddisEats.Api.Models;

public sealed class Restaurant
{
    public int Id { get; }

    public string Name { get; }

    public string Area { get; }

    public string Address { get; }

    public string OwnerPhone { get; }

    public decimal DeliveryFeeEtb { get; }

    public decimal CommissionRate { get; }

    public IReadOnlyList<MenuItem> MenuItems { get; }

    public Restaurant(
        int id,
        string name,
        string area,
        string address,
        string ownerPhone,
        decimal deliveryFeeEtb,
        decimal commissionRate,
        IEnumerable<MenuItem> menuItems)
    {
        Id = id;
        Name = name;
        Area = area;
        Address = address;
        OwnerPhone = ownerPhone;
        DeliveryFeeEtb = deliveryFeeEtb;
        CommissionRate = commissionRate;
        MenuItems = menuItems.ToArray();
    }
}
