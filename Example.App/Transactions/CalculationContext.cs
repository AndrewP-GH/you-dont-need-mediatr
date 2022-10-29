using Example.App.Infrastructure;

namespace Example.App.Transactions;

public record CalculationContext(int Target, IEnumerable<EntityBase> Entities = default);