using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace MauiMessenger.Client.Shared.Services;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddMessengerClientServices(
        this IServiceCollection services,
        string baseUrl)
    {
        services.AddScoped<CookieContainer>();
        services.AddHttpClient<IApiClient, ApiClient>(client => client.BaseAddress = new Uri(baseUrl))
            .ConfigurePrimaryHttpMessageHandler(serviceProvider => new HttpClientHandler
            {
                CookieContainer = serviceProvider.GetRequiredService<CookieContainer>(),
                UseCookies = true
            });
        services.AddScoped<IAuthSessionClient, DefaultAuthSessionClient>();
        services.AddScoped<IRealtimeMessageClient, SignalRRealtimeMessageClient>();
        services.AddScoped<CurrentUserState>();
        return services;
    }
}
