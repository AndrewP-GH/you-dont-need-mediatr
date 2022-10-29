using System.Net;
using Polly;
using Polly.Retry;

namespace Example.App.Infrastructure;

public class RetriesFactory
{
    public AsyncRetryPolicy DefaultStoragePolicy => Policy
        .Handle<CosmosException>(e => e.StatusCode == HttpStatusCode.PreconditionFailed)
        .WaitAndRetryAsync(5, i => TimeSpan.FromMilliseconds(i * i * 20));
}

class CosmosException : Exception
{
    public HttpStatusCode StatusCode { get; }
}