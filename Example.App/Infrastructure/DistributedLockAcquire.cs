namespace Example.App.Infrastructure;

public interface IDistributedLockAcquirer
{
    Task<IAsyncDisposable> AcquireLock(
        string resource,
        TimeSpan timeout,
        CancellationToken ct);
}

public class DistributedLockAcquire : IDistributedLockAcquirer
{
    public Task<IAsyncDisposable> AcquireLock(
        string resource,
        TimeSpan timeout,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(resource))
            throw new ArgumentException("Resource cannot be empty", nameof(resource));

        if (timeout != timeout.Duration())
            throw new ArgumentException("Timeout cannot be negative", nameof(timeout));

        IAsyncDisposable @lock = new DistributedLock();
        return Task.FromResult(@lock);
    }
}

public class DistributedLock : IAsyncDisposable
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}