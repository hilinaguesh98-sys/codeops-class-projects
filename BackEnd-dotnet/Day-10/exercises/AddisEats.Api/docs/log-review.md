# Logging review

## One-request volume comparison

For one successful `POST /api/v1/orders` request, the prior effective
`Information` threshold emitted one `OrderService` event: the order-created
event. With `AddisEats.Api` set to `Debug`, the same request emitted two
`OrderService` events: the placement `Debug` event and the created `Information`
event. That is one additional service event, or a 100% increase for this
request. `Microsoft` and `Microsoft.EntityFrameworkCore` are both filtered at
`Warning`.

## Privacy review

The available runtime sample included `RestaurantId`, `OrderId`, and a validated
`CorrelationId`. It contained no name, phone number, delivery address,
authentication token, credential, or other person-specific identifier that
could be used to act as a person. The correlation ID is trace metadata, not an
authorization credential. Order request bodies are not logged.

No historical or one-hour log file was supplied or collected for this task, so
this is a review of the controlled request sample, not a claim that a full hour
of production output was audited. The initial local HTTP-only startup also
emitted an HTTPS-port warning; it contained no user data.
