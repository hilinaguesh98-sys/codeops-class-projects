using AddisEats.Api.Models;

namespace AddisEats.Api.Services;

public interface IOrderService
{
    Result<OrderDto> GetById(int id);
    Result<OrderDto> Place(CreateOrderRequest request);
    Result<OrderDto> UpdateStatus(int id, UpdateOrderStatusRequest request);
}
