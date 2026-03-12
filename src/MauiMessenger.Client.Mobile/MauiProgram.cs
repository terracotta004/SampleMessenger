using MauiMessenger.Client.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiMessenger.Client.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var apiBaseUrl = GetApiBaseUrl();
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Api:BaseUrl"] = apiBaseUrl
        });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMessengerClientServices(apiBaseUrl);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static string GetApiBaseUrl()
    {
#if ANDROID
        return "http://192.168.1.158:5010";
#else
        return "http://localhost:5010";
#endif
    }
}
