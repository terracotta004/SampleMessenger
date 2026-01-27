using System.Data.Common;
using System.Reflection;
using MauiMessenger.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MauiMessenger.Api.Tests.Integration;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            RemoveProviderServices(services, "Npgsql.EntityFrameworkCore.PostgreSQL");
            RemoveProviderServices(services, "Npgsql");

            _connection ??= new SqliteConnection("DataSource=:memory:");
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    private static void RemoveProviderServices(IServiceCollection services, string providerAssemblyName)
    {
        var descriptors = services
            .Where(descriptor =>
                IsFromAssembly(descriptor.ServiceType.Assembly, providerAssemblyName)
                || IsFromAssembly(descriptor.ImplementationType?.Assembly, providerAssemblyName)
                || IsFromAssembly(descriptor.ImplementationInstance?.GetType().Assembly, providerAssemblyName)
                || IsFromAssembly(descriptor.ImplementationFactory?.Method.ReturnType.Assembly, providerAssemblyName))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    private static bool IsFromAssembly(Assembly? assembly, string assemblyName)
    {
        if (assembly is null)
        {
            return false;
        }

        var name = assembly.GetName().Name ?? string.Empty;
        return string.Equals(name, assemblyName, StringComparison.Ordinal)
            || name.StartsWith(assemblyName, StringComparison.Ordinal);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
