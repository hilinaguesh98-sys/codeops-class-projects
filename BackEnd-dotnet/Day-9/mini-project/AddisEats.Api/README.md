# AddisEats API Design

## Design rules

- Paths use lowercase, plural, hyphenated nouns; `/menu-items` is plural and the hyphenated form is used instead of `menu`.
- `{id:int}` segments are identifiers. `area`, `areas`, `maxFeeEtb`, `page`, `limit`, `category`, and pagination parameters are filters and appear in the query string.
- Nesting stops at two levels: collection -> identifier -> child collection.
- There are no named action verbs in paths. Standard HTTP verbs express operations:
  - `GET` retrieves or searches.
  - `POST` creates a member of a collection.
  - `PATCH` partially updates an existing resource.
- `200 OK` returns an existing or updated representation. `201 Created` returns a newly created representation plus `Location`. `204 No Content` confirms a successful update that intentionally has no response body.
- Bad input, malformed requests, missing resources, conflicts, authentication failures, and authorization failures use `4xx`. `5xx` is reserved for unexpected server failures; bad input never produces `500`.
- Every error response uses RFC 7807 `ProblemDetails` with `type`, `title`, `status`, `detail`, `instance`, and an application `code` extension.
- Unknown query parameters are rejected consistently with `400 UNKNOWN_QUERY_PARAMETER`.
- `limit` accepts only `1` through `100`; values outside that range return `400 INVALID_LIMIT`.

## Complete route table

| Method | Path | Purpose | Success | Failure codes |
|---|---|---|---|---|
| GET | `/api/v1/restaurants` | Search restaurants using query filters | `200` with restaurant array | `400 INVALID_PAGE`, `400 INVALID_LIMIT`, `400 UNKNOWN_QUERY_PARAMETER`, `400 INVALID_MODEL` |
| GET | `/api/v1/restaurants/{id:int}` | Retrieve one restaurant | `200` with `RestaurantDto` | `400 UNKNOWN_QUERY_PARAMETER`, `404 RESTAURANT_NOT_FOUND` |
| GET | `/api/v1/restaurants/{id:int}/menu-items` | Retrieve menu items for one restaurant | `200` with menu-item array | `400 UNKNOWN_QUERY_PARAMETER`, `404 RESTAURANT_NOT_FOUND` |
| GET | `/api/v1/menu-items` | Search menu items by category/page | `200` with menu-item array | `400 INVALID_PAGE`, `400 UNKNOWN_QUERY_PARAMETER`, `400 INVALID_MODEL` |
| GET | `/api/v1/menu-items/{id:int}` | Retrieve one menu item | `200` with `MenuItem` | `400 UNKNOWN_QUERY_PARAMETER`, `404 MENU_ITEM_NOT_FOUND` |
| POST | `/api/v1/menu-items` | Create a menu item | `201` with menu item and `Location` | `400 INVALID_PRICE`, `400 INVALID_MODEL`, `409 MENU_ITEM_CONFLICT`, `400 UNKNOWN_QUERY_PARAMETER` |
| GET | `/api/v1/orders/{id:int}` | Retrieve one order | `200` with `OrderDto` | `400 UNKNOWN_QUERY_PARAMETER`, `404 ORDER_NOT_FOUND` |
| POST | `/api/v1/orders` | Create an order | `201` with `OrderDto` and `Location` | `400 INVALID_TOTAL`, `400 INVALID_MODEL`, `400 UNKNOWN_QUERY_PARAMETER` |
| PATCH | `/api/v1/orders/{id:int}` | Partially update order status | `204` with no body | `400 INVALID_MODEL`, `400 UNKNOWN_QUERY_PARAMETER`, `404 ORDER_NOT_FOUND`, `409 ORDER_ALREADY_COMPLETED` |

## Error catalogue

| Code | Status | Message |
|---|---|---|
| `INVALID_MODEL` | `400` | The request body or parameters are invalid. |
| `UNKNOWN_QUERY_PARAMETER` | `400` | Unknown query parameter(s): `<names>`. |
| `INVALID_PAGE` | `400` | Page must be greater than zero. |
| `INVALID_LIMIT` | `400` | Limit must be between 1 and 100. |
| `INVALID_PRICE` | `400` | Menu item price must be greater than zero. |
| `RESTAURANT_NOT_FOUND` | `404` | Restaurant `<id>` was not found. |
| `MENU_ITEM_NOT_FOUND` | `404` | Menu item `<id>` was not found. |
| `ORDER_NOT_FOUND` | `404` | Order `<id>` was not found. |
| `MENU_ITEM_CONFLICT` | `409` | A menu item with that ID already exists. |
| `ORDER_ALREADY_COMPLETED` | `409` | An already completed order cannot be cancelled. |

Authentication or authorization are not currently implemented, but their reserved boundary is `401 UNAUTHENTICATED` and `403 FORBIDDEN`.

## Day 8 validation design

Every request type uses data annotations with custom messages. Request types cover restaurant search, menu-item creation/search, order creation/status updates, and query values. `CreateOrderRequest` and `CreateMenuItemRequest` use `IValidatableObject` for cross-field rules.

`EthiopianMobileAttribute` accepts `09xxxxxxxx`, `07xxxxxxxx`, `+2519xxxxxxxx`, and `+2517xxxxxxxx`. `EtbAmountAttribute` rejects zero, negative values, and values with more than two decimal places.

No request type accepts a monetary order total. The order service resolves every `MenuItemId`, multiplies it by server-stored `Price` and requested quantity, and computes the order total. An order with a 95 ETB server total is rejected with `422 MINIMUM_ORDER_TOTAL`.

`Result<T>` represents failures from restaurant lookup/search, menu lookup/search/create, and order get/place/status operations. Catalogue codes must originate from real requests:

| Code | Status | Real trigger |
|---|---|---|
| `INVALID_MODEL` | `400` | Invalid/missing annotation fields |
| `UNKNOWN_QUERY_PARAMETER` | `400` | Unrecognized query key |
| `INVALID_PAGE` | `400` | `page=0` or negative |
| `INVALID_LIMIT` | `400` | `limit=0` or `limit>100` |
| `INVALID_PRICE` | `400` | Zero or negative menu price |
| `RESTAURANT_NOT_FOUND` | `404` | Unknown restaurant ID |
| `MENU_ITEM_NOT_FOUND` | `404` | Unknown menu item ID |
| `ORDER_NOT_FOUND` | `404` | Unknown order ID |
| `MENU_ITEM_CONFLICT` | `409` | Duplicate menu item ID |
| `ORDER_ALREADY_COMPLETED` | `409` | Cancelling a completed order |
| `MINIMUM_ORDER_TOTAL` | `422` | Server-computed total below 100 ETB |

`ExceptionMiddleware` is the first middleware. It logs the exception and trace ID, returns `500 ProblemDetails` with `INTERNAL_SERVER_ERROR` and `traceId`, and never includes a stack trace. `POST /api/v1/failures/exception` verifies this beyond the order endpoint.

## Day 9 middleware pipeline

### Canonical pipeline

```text
ExceptionMiddleware
CorrelationIdMiddleware
RequestTimingMiddleware
Development Swagger
HttpsRedirection
Cors
// UseRateLimiter (future)
// UseAuthentication (future)
UseAuthorization
Map("/admin", ApiKeyMiddleware branch)
MapControllers
```

Line-by-line reason:

1. `ExceptionMiddleware` is first so exceptions from every later middleware and endpoint are converted to `500 ProblemDetails` with a trace ID and no stack trace.
2. `CorrelationIdMiddleware` follows exception handling so it can reject neither exceptions nor headers; it accepts `X-Correlation-Id` or generates an ID, sets `TraceIdentifier`, and returns the header on every response.
3. `RequestTimingMiddleware` follows correlation middleware so both the timing log and downstream responses share the correlation ID. It logs method, path, status, elapsed milliseconds, and correlation ID; elapsed time above `1000ms` is `Warning`, otherwise `Information`.
4. Swagger middleware is kept inside Development and comes after global diagnostics/correlation/timing.
5. HTTPS redirection comes after Swagger and before CORS/authorization.
6. CORS runs before authentication/authorization/routing endpoints so browser preflight and origin handling can complete.
7. `UseRateLimiter` is present as a comment in the canonical future position after CORS and before authentication.
8. `UseAuthentication` is present as a comment after rate limiting and before authorization.
9. `UseAuthorization` remains active after authentication's documented position. It runs after CORS and before endpoint branches.
10. `Map("/admin", ...)` is the API-key branch and is registered before `MapControllers`.
11. `MapControllers` is final and executes normal MVC endpoints.

### Correlation and timing verification

Sending `X-Correlation-Id: corridor-test-123` returned:

```text
X-Correlation-Id: corridor-test-123
```

The log showed the same ID in both the audit filter and timing middleware:

```text
Audit action=...RestaurantsController.GetById (AddisEats.Api) correlationId=corridor-test-123 arguments={"id":1}
Request completed correlationId=corridor-test-123 method=GET path=/api/v1/restaurants/1 status=200 elapsedMs=148
```

### Admin Map branch

`GET /admin` without `X-Admin-Key` short-circuits with `401 UNAUTHENTICATED` and never calls the branch endpoint.
`GET /admin` with `X-Admin-Key: admin-secret` passes through and returns `200`:

```json
{"branch":"admin","correlationId":"8e38a8dbe3e54ffebad9e9f4ac1f52fa"}
```

### Action filter

`AuditActionFilter` is an MVC action filter. It logs the action name and `ActionArguments` after model binding, including the correlation ID. Middleware cannot perform this action-specific logging because MVC action selection and argument binding have not yet occurred at most middleware boundaries.

No middleware constructor accepts a scoped service. Request timing and correlation constructors receive only singleton/convention dependencies (`RequestDelegate`, logger), keeping middleware lifetime and scope behavior safe.
