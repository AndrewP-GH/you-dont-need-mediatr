using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Example.Web.Middleware;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DataEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public DataEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var platform = GetPlatform(request.Headers);
        var deviceId = GetDeviceId(request.Headers);
        // Initialize the AsyncLocal's value ahead of time
        var requestInfo = RequestInfo.Current;
        requestInfo.Set(platform, deviceId);

        await _next(context);
    }

    private string GetDeviceId(IHeaderDictionary header) =>
        header.ContainsKey("deviceId")
            ? header["deviceId"].ToString()
            : string.Empty;

    private string GetPlatform(IHeaderDictionary header) =>
        header.ContainsKey("platform")
            ? header["platform"].ToString()
            : string.Empty;

    public sealed class RequestInfo
    {
        private static readonly AsyncLocal<RequestInfo> AsyncLocal = new();
        public static RequestInfo Current => AsyncLocal.Value ??= new();

        [MemberNotNullWhen(false, nameof(Platform), nameof(DeviceId))]
        public bool IsEmpty { get; private set; } = true;

        public string? Platform { get; private set; }
        public string? DeviceId { get; private set; }

        public void Set(string platform, string deviceId)
        {
            IsEmpty = false;
            Platform = platform;
            DeviceId = deviceId;
        }
    }
}