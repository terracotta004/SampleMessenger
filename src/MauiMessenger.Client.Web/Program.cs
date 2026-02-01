using MauiMessenger.Client.Web.Components;
using MauiMessenger.Client.Web.Services;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var app = Program.BuildApp(args);
app.Run();

public partial class Program
{
    public static WebApplication BuildApp(
        string[] args,
        string? environmentName = null,
        string? contentRootPath = null,
        Action<WebApplicationBuilder>? configureBuilder = null,
        Action<WebApplication>? configureApp = null)
    {
        var options = new WebApplicationOptions
        {
            Args = args,
            EnvironmentName = environmentName ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            ApplicationName = typeof(Program).Assembly.GetName().Name,
            ContentRootPath = contentRootPath
        };

        var builder = WebApplication.CreateBuilder(options);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddHttpClient<ApiClient>(client =>
        {
            var baseUrl = builder.Configuration.GetValue<string>("Api:BaseUrl") ?? "http://localhost:5010";
            client.BaseAddress = new Uri(baseUrl);
        });
        builder.Services.AddScoped<CurrentUserState>();

        if (builder.Environment.IsEnvironment("Testing"))
        {
            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
        }

        configureBuilder?.Invoke(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        configureApp?.Invoke(app);

        return app;
    }
}
