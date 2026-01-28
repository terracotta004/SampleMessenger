using Microsoft.Playwright;
using MauiMessenger.Client.Web.Tests.Integration;

namespace MauiMessenger.Client.Web.Tests.Playwright;

public sealed class WebAppFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;

    public WebAppHost Host { get; } = new();
    public IBrowser Browser { get; private set; } = default!;
    public IBrowser? HeadedBrowser { get; private set; }
    public Uri BaseUrl => Host.BaseAddress;

    public async Task InitializeAsync()
    {
        await Host.InitializeAsync();
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        HeadedBrowser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
    }

    public async Task DisposeAsync()
    {
        if (HeadedBrowser is not null)
        {
            await HeadedBrowser.CloseAsync();
        }

        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        _playwright?.Dispose();
        await Host.DisposeAsync();
    }
}
