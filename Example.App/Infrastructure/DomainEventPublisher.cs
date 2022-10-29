using JetBrains.Annotations;
using MediatR;

namespace Example.App.Infrastructure;

public interface IDomainEventPublisher
{
    Task Notify(IEnumerable<EntityBase> entities);
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IMediator _mediator;

    public DomainEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Notify(IEnumerable<EntityBase> entities)
    {
        foreach (var entity in entities)
        {
            var events = entity.GetChanges.ToArray();
            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent);
            }
            entity.Commit();
        }
    }
}