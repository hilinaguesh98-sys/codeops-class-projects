# AddisEats

AddisEats is a beginner-friendly food ordering example. `AddisEats.OrderBook` contains the shared menu model, order calculations, area/category reports, pagination, and a console demonstration. `AddisEats.Api` exposes the shared menu model through an ASP.NET Core controller and an in-memory menu service.

## Projects and technologies

- **AddisEats.OrderBook** — .NET 8 console project containing the domain models and order-book reporting.
- **AddisEats.Api** — .NET 8 ASP.NET Core Web API with controller routing, dependency injection, configuration options, and Swagger.
- Both projects use the same `MenuItem` model from the OrderBook project; the API does not duplicate it.

## Build and run

From this directory:

```powershell
dotnet build AddisEats.sln
dotnet run --project AddisEats.OrderBook\AddisEats.OrderBook.csproj
dotnet run --project AddisEats.Api\AddisEats.Api.csproj --launch-profile http
```

With the API running, open Swagger at <http://localhost:5106/swagger>.

The API uses the `Menu:DefaultPageSize` setting in `AddisEats.Api\appsettings.json` (default `5`). GET requests return all matching items unless `page` is supplied; for example, `/api/v1/menu?category=Stew&page=1`.

## Menu API

| Method and route | Success | Failure |
| --- | --- | --- |
| `GET /api/v1/menu?category={category}` | `200 OK`; omit `category` to get all items | `400 Bad Request` for an invalid page number |
| `GET /api/v1/menu/{id}` | `200 OK` with the item | `404 Not Found` if the ID does not exist |
| `POST /api/v1/menu` | `201 Created` with a `Location` header for the new item | `400 Bad Request` for a non-positive price; `409 Conflict` for a duplicate ID |

Example request body for POST:

```json
{
  "id": 7,
  "name": "Kitfo Special",
  "category": "Main",
  "price": 285.00
}
```

In Swagger, use **Try it out** on the POST endpoint to create an item. Set `price` to `0` to see the `400` response, and request a missing ID such as `/api/v1/menu/999` to see `404`.
