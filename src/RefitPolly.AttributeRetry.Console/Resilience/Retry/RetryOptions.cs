namespace RefitPolly.AttributeRetry.Console.Resilience.Retry;

public sealed class RetryOptions
{
    public int MaxRetryAttempts { get; init; } = 3;
    public int BaseDelaySeconds { get; init; } = 2;
}


