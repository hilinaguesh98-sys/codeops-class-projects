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
