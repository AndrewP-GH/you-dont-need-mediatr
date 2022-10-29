using Example.App.Infrastructure;
using Example.App.Logging;
using Example.App.Metrics;
using Example.App.Native;
using Example.App.Tracing;
using Example.Web.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

[ApiController]
[Route("native")]
public class NativeController : ControllerBase
{
    private readonly CalculationUnit _unit;
    private readonly IDistributedLockAcquirer _lockAcquirer;
    private readonly IScope _scope;

    public NativeController(CalculationUnit unit, IDistributedLockAcquirer lockAcquirer)
    {
        _unit = unit;
        _lockAcquirer = lockAcquirer;
    }

    public record CalculateInput(int? target);

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input, CancellationToken ct)
    {
        var target = ValidateNullable.GetOrThrow(ctx => ctx.Get(input.target));
        using (_scope.WithScope(input))
        using (Elapsed.WithMeter<MediatRController.CalculateInput>())
        using (Trace.WithTrace<MediatRController.CalculateInput>())
        {
            await using var _ = await _lockAcquirer.AcquireLock("calculate", TimeSpan.FromMinutes(1), ct);
            return await _unit.DoCalculate(target);
        }
    }
}