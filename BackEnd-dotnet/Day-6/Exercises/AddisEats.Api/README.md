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
