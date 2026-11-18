using AddisEats.Api.Models;

namespace AddisEats.Api.Services.menuitemservices;

public interface IMenuItemServices
{
    List<Menuitem> GetAll();
    Menuitem? GetById(int id);
}