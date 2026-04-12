using Microsoft.Extensions.DependencyInjection;

namespace MauiMessenger.Client.Shared.Services;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddMessengerClientServices(
        this IServiceCollection services,
        string baseUrl)
    {
        services.AddHttpClient<IApiClient, ApiClient>(client => client.BaseAddress = new Uri(baseUrl));
        services.AddScoped<IAuthSessionClient, DefaultAuthSessionClient>();
        services.AddScoped<CurrentUserState>();
        return services;
    }
}
