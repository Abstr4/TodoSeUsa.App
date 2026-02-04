namespace TodoSeUsa.Infrastructure.Hosting;

public static class SingleInstanceGuard
{
    private static Mutex? _mutex;

    public static bool TryAcquire(string name)
    {
        _mutex = new Mutex(true, name, out var isFirstInstance);
        return isFirstInstance;
    }
}