using MauiMessenger.Client.Shared.Services;
using Microsoft.Extensions.Logging;

namespace MauiMessenger.Client.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMessengerClientServices(GetApiBaseUrl());

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static string GetApiBaseUrl()
    {
#if ANDROID
        return "http://10.0.2.2:5010";
#else
        return "http://localhost:5010";
#endif
    }
}

