using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IOrderService
{
    Order? GetById(int id);
    Order Create(CreateOrderRequest request);
    Order UpdateStatus(int id, OrderStatus status);
}
