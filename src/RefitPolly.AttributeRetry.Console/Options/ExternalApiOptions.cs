using RefitPolly.AttributeRetry.Console.Resilience.Retry;

namespace RefitPolly.AttributeRetry.Console.Options;

public class ExternalApiOptions
{
    public static string Session => "ExternalApi";
    public required string UrlBase { get; set; }
    public RetryOptions RetryOptions { get; set; } = new();
}

