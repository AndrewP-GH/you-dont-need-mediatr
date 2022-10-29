using MediatR;

namespace Example.App.Infrastructure;

public abstract class EntityBase
{
    private readonly List<DomainEventBase> _changes = new();
    public int Id { get; init; }

    public IReadOnlyCollection<DomainEventBase> GetChanges => _changes.AsReadOnly();
    protected void Add(DomainEventBase domainEvent) => _changes.Add(domainEvent);
    public void Commit()
        => _changes.Clear();
}

public abstract class DomainEventBase : INotification
{
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
}