using Microsoft.Extensions.DependencyInjection;

namespace MauiMessenger.Client.Shared.Services;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddMessengerClientServices(
        this IServiceCollection services,
        string baseUrl)
    {
        services.AddHttpClient<ApiClient>(client => client.BaseAddress = new Uri(baseUrl));
        services.AddScoped<CurrentUserState>();
        return services;
    }
}
