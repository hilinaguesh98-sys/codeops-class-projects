using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IOrderService
{
    Order? GetById(int id);
    Result<Order> Place(CreateOrderRequest request);
    Order UpdateStatus(int id, OrderStatus status);
}
