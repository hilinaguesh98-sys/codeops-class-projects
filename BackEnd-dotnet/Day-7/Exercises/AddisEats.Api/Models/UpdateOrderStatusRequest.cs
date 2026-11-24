using Microsoft.AspNetCore.Mvc;

namespace AddisEats.Api.Models;

public sealed record UpdateOrderStatusRequest(OrderStatus Status);
