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
    private readonly RetriesFactory _retriesFactory;
    private readonly IScope _scope;

    public NativeController(CalculationUnit unit, RetriesFactory retriesFactory)
    {
        _unit = unit;
        _retriesFactory = retriesFactory;
    }

    public record CalculateInput(int? target);

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input)
    {
        var target = ValidateNullable.GetOrThrow(ctx => ctx.Get(input.target));
        using (_scope.WithScope(input))
        using (Elapsed.WithMeter<MediatRController.CalculateInput>())
        using (Trace.WithTrace<MediatRController.CalculateInput>())
        {
            var retryPolicy = _retriesFactory.DefaultStoragePolicy;
            return await retryPolicy.ExecuteAsync(async () => await _unit.DoCalculate(target));
        }
    }
}