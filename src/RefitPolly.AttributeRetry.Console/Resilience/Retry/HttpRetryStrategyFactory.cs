using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace RefitPolly.AttributeRetry.Console.Resilience.Retry;

public static class HttpRetryStrategyFactory
{
    private static readonly HttpRequestOptionsKey<RestMethodInfo> RestMethodKey =
        new(HttpRequestMessageOptions.RestMethodInfo);

    public static HttpRetryStrategyOptions Create(RetryOptions retryOptions)
    {
        return new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = retryOptions.MaxRetryAttempts,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(retryOptions.BaseDelaySeconds),

            // Only for testing retries
            OnRetry = args =>
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine(
                    $"Retry #{args.AttemptNumber} " +
                    $"Status: {args.Outcome.Result?.StatusCode}"
                );
                System.Console.ResetColor();
                return default;
            },

            ShouldHandle = args =>
            {
                var request = args.Context.GetRequestMessage();

                if (request is null)
                {
                    System.Console.WriteLine($"No Retry: AttemptNumber = {args.AttemptNumber}");
                    return ValueTask.FromResult(false);
                }

                if (!request.Options.TryGetValue(RestMethodKey, out var restMethod))
                {
                    System.Console.WriteLine($"No Retry: AttemptNumber = {args.AttemptNumber}");
                    return ValueTask.FromResult(false);
                }

                if (!restMethod.MethodInfo.IsDefined(typeof(RetryAttribute), false))
                {
                    System.Console.WriteLine($"No Retry: AttemptNumber = {args.AttemptNumber}");
                    return ValueTask.FromResult(false);
                }

                return ValueTask.FromResult(HttpClientResiliencePredicates.IsTransient(args.Outcome));
            }
        };
    }
}