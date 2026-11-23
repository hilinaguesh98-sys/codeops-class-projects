namespace AddisEats.Api.Services;

public class SystemClock : IClock
{
    public SystemClock()
    {
        UtcNow = DateTime.UtcNow;
    }

    public DateTime UtcNow { get; }
}
