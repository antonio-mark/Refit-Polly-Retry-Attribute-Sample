using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Refit;
using RefitPolly.AttributeRetry.Console.Interfaces;
using RefitPolly.AttributeRetry.Console.Options;
using RefitPolly.AttributeRetry.Console.Resilience.Retry;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddOptions<ExternalApiOptions>()
        .BindConfiguration(ExternalApiOptions.Session)
        .ValidateOnStart();

services.AddRefitClient<IHttpBinApi>()
        .ConfigureHttpClient((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ExternalApiOptions>>().Value;
            client.BaseAddress = new Uri(options.UrlBase);
        })
        .AddResilienceHandler("RefitRetryPipeline", (builder, context) =>
        {
            var options = context.ServiceProvider.GetRequiredService<IOptions<ExternalApiOptions>>().Value;
            builder.AddRetry(HttpRetryStrategyFactory.Create(options.RetryOptions));
        });

var provider = services.BuildServiceProvider();

var client = provider.GetRequiredService<IHttpBinApi>();

Console.WriteLine("Calling endpoint with retry enabled that returns HTTP 500...\n");

var responseWithRetry = await client.Get500WithRetry();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"Final status: {responseWithRetry.StatusCode}");
Console.ResetColor();

Console.WriteLine("\nDone.");

Console.WriteLine("\nCalling endpoint with retry disabled that returns HTTP 500...\n");

var response = await client.Get500();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"Final status: {response.StatusCode}");
Console.ResetColor();

Console.WriteLine("\nDone.");

Console.ReadLine();