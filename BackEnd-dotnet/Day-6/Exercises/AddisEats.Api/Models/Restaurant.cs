namespace AddisEats.Api.Models;

public sealed class Restaurant
{
    public Restaurant(
        int id,
        string name,
        string area,
        string address,
        decimal deliveryFeeEtb,
        decimal? commissionRate = null)
    {
        Id = id;
        Name = name;
        Area = area;
        Address = address;
        DeliveryFeeEtb = deliveryFeeEtb;
        CommissionRate = commissionRate;
    }

    public int Id { get; }
    public string Name { get; }
    public string Area { get; }
    public string Address { get; }
    public decimal DeliveryFeeEtb { get; }
    public decimal? CommissionRate { get; }
}
