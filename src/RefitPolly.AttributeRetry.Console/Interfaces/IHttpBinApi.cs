using Refit;
using RefitPolly.AttributeRetry.Console.Resilience.Retry;

namespace RefitPolly.AttributeRetry.Console.Interfaces;

public interface IHttpBinApi
{
    [Get("/status/500")]
    [Retry]
    Task<HttpResponseMessage> Get500WithRetry();

    [Get("/status/500")]
    Task<HttpResponseMessage> Get500();
}
