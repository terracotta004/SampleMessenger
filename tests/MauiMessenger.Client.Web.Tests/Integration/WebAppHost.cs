using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MauiMessenger.Client.Web.Tests.Integration;

public sealed class WebAppHost : IAsyncLifetime
{
    private WebApplication? _app;

    public Uri BaseAddress { get; private set; } = new("http://localhost");

    public async Task InitializeAsync()
    {
        _app = Program.BuildApp(
            Array.Empty<string>(),
            environmentName: "Testing",
            contentRootPath: GetContentRoot(),
            configureBuilder: builder =>
            {
                builder.WebHost.UseKestrel();
                builder.WebHost.UseSetting(WebHostDefaults.ServerUrlsKey, "http://127.0.0.1:0");
            });

        await _app.StartAsync();

        var server = _app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var address = addresses?.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidOperationException("Failed to determine the test server address.");
        }

        BaseAddress = new Uri(address);
    }

    private static string GetContentRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/MauiMessenger.Client.Web"));
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}
