namespace AddisEats.Api.Models;

public class Menuitem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Catagory { get; set; } = string.Empty;
    public int Price { get; set; }
    public decimal pricewithvat => Price * 1.15m;
}