# AddisEats.Api

Run the API with `dotnet run --launch-profile http`.

Swagger UI: http://localhost:5119/swagger

## Exercise 4: IClock lifetime observations

Two consecutive GET requests to `/api/v1/health` returned:

| Lifetime | Request 1 (`utcTime`, `secondUtcTime`) | Request 2 (`utcTime`, `secondUtcTime`) |
| --- | --- | --- |
| Transient | `19:05:19.7028301Z`, `19:05:19.7033348Z` | `19:05:20.1108750Z`, `19:05:20.1108850Z` |
| Scoped | `19:06:32.0343005Z`, `19:06:32.0343005Z` | `19:06:32.5169785Z`, `19:06:32.5169785Z` |
| Singleton | `19:07:51.8593401Z`, `19:07:51.8593401Z` | `19:07:51.8593401Z`, `19:07:51.8593401Z` |

Transient creates a new clock for every resolution, so the two injected values differed within each request and across requests; scoped reuses one clock instance within a request, so the pair matched but changed on the next request. Singleton reuses one clock instance for the application lifetime, so its value remained unchanged across both requests.

## Exercise 5: payment provider switch

The application now registers `CbeBirrProvider` instead of `TeleBirrProvider`; `GET /api/v1/payments/provider` returned `{"provider":"CBE Birr"}`. The switch changed one DI registration line, and no controller changed.

## Exercise 6: delivery options

`DeliveryOptions` is bound from the `Delivery` section in `appsettings.json`. `GET /api/v1/delivery/options` returns `freeAboveEtb: 500` and `defaultFeeEtb: 50`.

## Exercise 7: missing registration

Expected exception message when the `IPaymentProvider` scoped registration is removed (not runtime-captured):

> Unable to resolve service for type 'AddisEats.Api.Services.IPaymentProvider' while attempting to activate 'AddisEats.Api.Controllers.PaymentProviderController'.

The message says dependency injection could not resolve `IPaymentProvider` while constructing `PaymentProviderController`.

## Exercise 8: authentication middleware order

The protected endpoint is `GET /api/v1/protected` with `X-Exercise-Auth: enabled`. Expected observation if `UseAuthorization` precedes `UseAuthentication`: the request is unauthorized even with the header (not runtime-verified).

Authentication must run first to populate `HttpContext.User` before authorization evaluates the protected endpoint.
