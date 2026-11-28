using AddisEats.Api.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AddisEats.Api.OpenApi;

internal sealed class OrderStatusSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(OrderStatus))
        {
            schema.Example = new OpenApiString(nameof(OrderStatus.Pending));
        }

        if (context.Type == typeof(OrderDto) ||
            context.Type == typeof(UpdateOrderStatusRequest))
        {
            if (schema.Properties.TryGetValue("status", out var statusProperty))
            {
                schema.Properties["status"] = new OpenApiSchema
                {
                    Type = "string",
                    Enum =
                    [
                        new OpenApiString(nameof(OrderStatus.Pending)),
                        new OpenApiString(nameof(OrderStatus.Accepted)),
                        new OpenApiString(nameof(OrderStatus.Completed)),
                        new OpenApiString(nameof(OrderStatus.Cancelled))
                    ],
                    Example = new OpenApiString(nameof(OrderStatus.Pending)),
                    Description = statusProperty.Description,
                    Nullable = statusProperty.Nullable
                };
            }
        }
    }
}
