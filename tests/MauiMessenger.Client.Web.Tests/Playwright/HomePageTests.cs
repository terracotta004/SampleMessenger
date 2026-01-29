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
        //Arrange
        var browser = headless ? _fixture.Browser : _fixture.HeadedBrowser ?? _fixture.Browser;
        var page = await browser.NewPageAsync();
        try
        {
            await page.GotoAsync(new Uri(_fixture.BaseUrl, "counter").ToString());
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Act & Assert
            var status = page.GetByTestId("counter-status");
            var button = page.GetByTestId("counter-increment");

            await Assertions.Expect(status).ToHaveTextAsync("Current count: 0");
            await Assertions.Expect(button).ToContainTextAsync("Click me");

            //Act
            await button.ClickAsync();

            await Assertions.Expect(status).ToHaveTextAsync("Current count: 1");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}