using AddisEats.Api.Models;
using AddisEats.Api.Services.menuitemservices;

namespace AddisEats.Api.Services.menuitemservice;

public class MenuItemServices : IMenuItemServices
{
    private readonly List<Menuitem> menuitems = new()
    {
        new Menuitem { Id = 1, Name = "blabla", Catagory = "maincourse", Price = 1234 },
        new Menuitem { Id = 2, Name = "Cheeseburger", Catagory = "maincourse", Price = 1234 },
        new Menuitem { Id = 3, Name = "blabla", Catagory = "maincourse", Price = 1234 }
    };

    public List<Menuitem> GetAll()
    {
        return menuitems;
    }

    public Menuitem? GetById(int id)
    {
        return menuitems.FirstOrDefault(x => x.Id == id);
    }
}