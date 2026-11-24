# AddisEats API routing and serialization notes

## Int route constraints
`/api/v1/restaurants/abc` matches no route segment and returns `404`, while an unconstrained route may accept it and return `400` for model binding.

## Route parameter mismatch
The route names the parameter `restaurantId`, while `GetOne(int id)` expects `id`. This is recorded as a mismatch exercise: it is not a compile error because route tokens are template strings and C# parameter names are independent of them; ASP.NET Core, however, reports it as a runtime binding/error condition rather than a compile-time issue.

## Areas query
`?areas=Bole&areas=Piassa` binds to `RestaurantQuery.Areas` and filters across both areas. With no `areas` values, `Areas.Count == 0` and the filter is skipped, so every restaurant is returned.

## Ambiguous route startup exception
When two route templates were deliberately made identical (`{id:int}` and `{id:int}`), ASP.NET Core stopped startup with:

```text
AmbiguousMatchException: The supplied route is ambiguous for the endpoints:
RestaurantController.GetById
RestaurantController.GetOther
```

Fix 1: add constraints to distinguish the templates, for example `{id:int}` and `{slug:alpha}`. Fix 2: use distinct literal paths, for example `featured/{id:int}` and `restaurants/{id:int}`.

## JSON options
- Camel-case response property names.
- `null` properties omitted.
- Enums serialized by name because `JsonStringEnumConverter` is configured.

## Verified response
Before JSON configuration, a restaurant response used CLR-style property names and might include null values. After configuration:

```json
{"id":1,"name":"Bole Injera House","area":"Bole","address":"Bole Road","deliveryFeeEtb":35,"commissionRate":0.10}
```

The same DTO serializes a nullable commission omission when it is `null`:

```json
{"id":2,"name":"Lomi Kitchen","area":"Bole","address":"Atlas Road","deliveryFeeEtb":28}
```

The order enum is serialized as `"Pending"` rather than `0`.

## Complete route table

"Identifier" means a path value identifies one resource. "Filter" means a path value narrows a collection; filters belong in the query string, not the path.

| Method | Path | Segment classification | Purpose | Success | Failure codes |
|---|---|---|---|---|---|
| GET | `/api/v1/restaurants` | Collection resource; `area`, `areas`, `maxFeeEtb`, `page`, `limit` are query filters | Search restaurants | `200` | `400` invalid filters; `400` unknown query parameter |
| GET | `/api/v1/restaurants/{id:int}` | `{id:int}` = identifier | Get one restaurant | `200` | `404` restaurant not found; `400` route constraint or unknown query parameter |
| GET | `/api/v1/restaurants/{id:int}/menu` | `{id:int}` = identifier | Get a restaurant menu | `200` | `404` restaurant not found; `400` unknown query parameter |
| GET | `/api/v1/orders/{id:int}` | `{id:int}` = identifier | Get one order | `200` | `404` order not found; `400` unknown query parameter |
| POST | `/api/v1/orders` | No path identifier or filter | Create an order | `201` | `400` invalid/negative total or malformed body; `401` unauthenticated; `403` not permitted |
| PATCH | `/api/v1/orders/{id:int}` | `{id:int}` = identifier; status is the request body | Update order status | `200` | `400` invalid status/body or unknown query parameter; `404` order not found; `409` illegal delivered-order transition; `401`; `403` |

Changed in this pass: the route parameter mismatch was fixed from `restaurantId` to `id`; POST/PATCH values are resource operations; and all collection narrowing remains in query parameters. Status is not a path filter, so it was moved to the JSON body.

## Route-verbs and rewrites

No path currently contains an HTTP action verb such as `get`, `create`, `update`, `cancel`, or `delete`. The existing path words `restaurants`, `orders`, and `menu` are nouns. The only operation-name decision was status updating:

1. `PATCH /api/v1/orders/{id:int}` — chosen: directly updates the order resource.
2. `PATCH /api/v1/orders/{id:int}/status` — alternative: duplicates resource hierarchy in a `/status` sub-resource.
3. `POST /api/v1/orders/{id:int}/cancel` — alternative: an action-style RPC path.

Chosen because it expresses a partial update of the order and avoids a verb or action sub-resource. `/status` is still a valid sub-resource in a dedicated workflow, but it adds a hierarchy when a single mutable field is enough.

## Error catalogue

| Status | Code | Message |
|---|---|---|
| `400 Bad Request` | `INVALID_FILTER` | One or more restaurant filters are invalid. |
| `400 Bad Request` | `UNKNOWN_QUERY_PARAMETER` | Unknown query parameter(s): `{names}`. |
| `400 Bad Request` | `INVALID_TOTAL` | The ETB total must not be negative. |
| `400 Bad Request` | `INVALID_STATUS` | The supplied order status is invalid. |
| `400 Bad Request` | `MALFORMED_BODY` | The request body is missing or malformed. |
| `404 Not Found` | `RESTAURANT_NOT_FOUND` | Restaurant `{id}` was not found. |
| `404 Not Found` | `ORDER_NOT_FOUND` | Order `{id}` was not found. |
| `409 Conflict` | `ORDER_ALREADY_DELIVERED` | A delivered order cannot be cancelled. |
| `401 Unauthorized` | `UNAUTHENTICATED` | A valid access token is required. |
| `403 Forbidden` | `FORBIDDEN` | The authenticated identity is not permitted to perform this action. |

## Scenario decisions

- **Search returns no rows:** `200 OK`; an empty collection is a successful query, not a missing resource.
- **Cancel an order already delivered:** `409 Conflict`; the order exists, but its current state disallows cancellation.
- **Request has a negative ETB total:** `400 Bad Request`; the submitted representation is invalid.
- **Valid courier token attempts to edit a menu:** `403 Forbidden`; authentication succeeds, but the courier role lacks authorization.

## Rewritten bad handout routes

The twelve handout routes below follow the usual noun/verb anti-pattern exercise; each original route has been rewritten below by replacing actions with resources and HTTP methods.

| Original route | Rewritten route | Broken naming rule |
|---|---|---|
| `GET /getRestaurants` | `GET /api/v1/restaurants` | Avoid verbs in URI paths; use the HTTP method. |
| `GET /restaurant/{id}` | `GET /api/v1/restaurants/{id:int}` | Use plural collection names and an integer identifier constraint. |
| `POST /createRestaurant` | `POST /api/v1/restaurants` | Avoid action verbs; collection POST creates a resource. |
| `PUT /updateRestaurant/{id}` | `PATCH /api/v1/restaurants/{id:int}` | Avoid verbs; use PATCH for partial updates. |
| `GET /deleteRestaurant/{id}` | `DELETE /api/v1/restaurants/{id:int}` | Never use GET for state-changing actions. |
| `GET /restaurants/search` | `GET /api/v1/restaurants?area=Bole` | Put filters in the query string, not a pseudo-resource. |
| `GET /getMenu/{restaurantId}` | `GET /api/v1/restaurants/{restaurantId:int}/menu` | Avoid verbs and use a nested resource for menu. |
| `POST /placeOrder` | `POST /api/v1/orders` | Avoid action verbs; POST to the orders collection. |
| `GET /getOrders` | `GET /api/v1/orders` | Avoid verbs and use the plural collection. |
| `GET /getOrder/{id}` | `GET /api/v1/orders/{id:int}` | Avoid verbs and identify the resource by ID. |
| `POST /cancelOrder/{id}` | `PATCH /api/v1/orders/{id:int}` | Model status changes as resource updates, not cancellation actions. |
| `POST /updateOrderStatus/{id}` | `PATCH /api/v1/orders/{id:int}` | Avoid verbs; send the new status in the body. |

## REST, GraphQL, and gRPC choices

- **Public mobile API:** Choose REST. It is cache-friendly and broadly compatible with mobile networking stacks, with stable HTTP semantics and easy independent client evolution.
- **Internal service-to-service call:** Choose gRPC. It provides strongly typed contracts and efficient binary transport, while HTTP/2 streaming suits high-volume internal communication.
- **Dashboard needing six resources:** Choose GraphQL. One operation can request exactly the needed fields from all six resources, avoiding under-fetching and multiple REST round trips. REST remains viable if the dashboard can tolerate endpoints and caching per resource.

## Unknown query parameters

The API rejects unknown query parameters with `400 Bad Request` and code `UNKNOWN_QUERY_PARAMETER`. Search filters are limited to `area`, `areas`, `maxFeeEtb`, `page`, and `limit`; routes also reject any unexpected query string for one-resource and write endpoints.
