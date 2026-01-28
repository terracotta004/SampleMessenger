using Microsoft.Playwright;

namespace MauiMessenger.Client.Web.Tests.Playwright;

[Collection("WebApp collection")]
public class HomePageTests
{
    private readonly WebAppFixture _fixture;

    public HomePageTests(WebAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HomePage_Loads(bool headless)
    {
        var browser = headless ? _fixture.Browser : _fixture.HeadedBrowser ?? _fixture.Browser;
        var page = await browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl.ToString());
            var title = await page.TextContentAsync("h1");
            Assert.Equal("Hello, world!", title);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Counter_Increments(bool headless)
    {
        var browser = headless ? _fixture.Browser : _fixture.HeadedBrowser ?? _fixture.Browser;
        var page = await browser.NewPageAsync();
        try
        {
            await page.GotoAsync(new Uri(_fixture.BaseUrl, "counter").ToString());
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            var status = page.Locator("[data-testid='counter-status']");
            var button = page.Locator("[data-testid='counter-increment']");

            await Assertions.Expect(status).ToHaveTextAsync("Current count: 0");
            await page.EvaluateAsync(
                "selector => document.querySelector(selector)?.click()",
                "[data-testid='counter-increment']");
            await Assertions.Expect(status).ToHaveTextAsync("Current count: 1");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
