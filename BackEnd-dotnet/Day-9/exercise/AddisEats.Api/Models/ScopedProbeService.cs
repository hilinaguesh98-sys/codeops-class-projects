namespace AddisEats.Api.Models;

public sealed class ScopedProbeService
{
    public Guid ScopeId { get; } = Guid.NewGuid();
}
