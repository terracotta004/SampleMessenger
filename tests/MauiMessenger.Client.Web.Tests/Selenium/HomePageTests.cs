using MauiMessenger.Client.Web.Tests.Integration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MauiMessenger.Client.Web.Tests.Selenium;

[Collection("WebApp collection")]
public class HomePageTests
{
    private readonly WebAppHost _host;

    public HomePageTests(WebAppHost host)
    {
        _host = host;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void HomePage_Loads(bool headless)
    {
        using var driver = CreateDriver(headless);

        driver.Navigate().GoToUrl(_host.BaseAddress.ToString());

        var heading = driver.FindElement(By.TagName("h1")).Text;
        Assert.Equal("Hello, world!", heading);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Counter_Increments(bool headless)
    {
        using var driver = CreateDriver(headless);

        driver.Navigate().GoToUrl(new Uri(_host.BaseAddress, "counter").ToString());

        var statusSelector = "[data-testid='counter-status']";
        var buttonSelector = "[data-testid='counter-increment']";
        var status = By.CssSelector(statusSelector);
        var button = By.CssSelector(buttonSelector);

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(status).Text == "Current count: 0");

        wait.Until(d =>
        {
            var element = d.FindElement(button);
            if (!element.Displayed || !element.Enabled)
            {
                return false;
            }

            ClickWithFallback(d, buttonSelector);
            return d.FindElement(status).Text == "Current count: 1";
        });
    }

    private static void ClickWithFallback(IWebDriver driver, string selector)
    {
        try
        {
            driver.FindElement(By.CssSelector(selector)).Click();
        }
        catch (Exception ex) when (ex is StaleElementReferenceException or ElementClickInterceptedException or WebDriverException)
        {
            var executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("document.querySelector(arguments[0])?.click();", selector);
        }
    }

    private static IWebDriver CreateDriver(bool headless)
    {
        var options = new ChromeOptions();
        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        var chromiumPath = Environment.GetEnvironmentVariable("CHROMIUM_BIN");
        if (string.IsNullOrWhiteSpace(chromiumPath))
        {
            if (File.Exists("/usr/bin/chromium"))
            {
                chromiumPath = "/usr/bin/chromium";
            }
            else if (File.Exists("/usr/bin/chromium-browser"))
            {
                chromiumPath = "/usr/bin/chromium-browser";
            }
        }

        if (string.IsNullOrWhiteSpace(chromiumPath))
        {
            throw new InvalidOperationException(
                "Chromium binary not found. Set CHROMIUM_BIN or install chromium at /usr/bin/chromium.");
        }

        options.BinaryLocation = chromiumPath;

        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        var driverPath = Environment.GetEnvironmentVariable("CHROMEDRIVER_BIN");
        if (string.IsNullOrWhiteSpace(driverPath))
        {
            if (File.Exists("/usr/bin/chromedriver"))
            {
                driverPath = "/usr/bin/chromedriver";
            }
            else if (File.Exists("/usr/lib/chromium-browser/chromedriver"))
            {
                driverPath = "/usr/lib/chromium-browser/chromedriver";
            }
        }

        if (string.IsNullOrWhiteSpace(driverPath))
        {
            throw new InvalidOperationException(
                "Chromedriver not found. Set CHROMEDRIVER_BIN or install chromium-driver.");
        }

        var service = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(driverPath)!, Path.GetFileName(driverPath));
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;

        return new ChromeDriver(service, options);
    }
}
