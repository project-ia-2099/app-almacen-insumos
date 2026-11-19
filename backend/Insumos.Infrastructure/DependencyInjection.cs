using Insumos.Application.Services;
using Insumos.Domain.Interfaces;
using Insumos.Infrastructure.Data;
using Insumos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Insumos.Infrastructure;

/// <summary>
/// Infrastructure dependency injection and MySQL database bootstrapper.
/// </summary>
public static class DependencyInjection
{
    private const string AreaName = "almacen";
    private const string MigrationsHistoryTableName = "__EFMigrationsHistory_insumos";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string environmentName)
    {
        var baseConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(baseConnectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection no está configurado. Configure la variable de entorno o user-secrets.");
        }

        var databaseName = ResolveDatabaseName(environmentName);
        var builder = new MySqlConnectionStringBuilder(baseConnectionString)
        {
            Database = databaseName
        };

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(
                builder.ConnectionString,
                mysql => mysql.MigrationsHistoryTable(MigrationsHistoryTableName)));

        services.AddScoped<IInsumoRepository, InsumoRepository>();
        services.AddScoped<IInsumoService, InsumoService>();

        return services;
    }

    /// <summary>
    /// Ensures the area MySQL database exists when relational and applies EF Core migrations.
    /// </summary>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        IConfiguration configuration,
        string environmentName)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!dbContext.Database.IsRelational())
        {
            return;
        }

        var baseConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(baseConnectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection no está configurado.");
        }

        var databaseName = ResolveDatabaseName(environmentName);
        var serverBuilder = new MySqlConnectionStringBuilder(baseConnectionString)
        {
            Database = string.Empty
        };

        await using (var connection = new MySqlConnection(serverBuilder.ConnectionString))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText =
                $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            await command.ExecuteNonQueryAsync();
        }

        await dbContext.Database.MigrateAsync();
    }

    public static string ResolveDatabaseName(string environmentName)
    {
        return environmentName switch
        {
            "Development" => $"local_{AreaName}",
            "QA" => $"qa_{AreaName}",
            "Production" => $"prod_{AreaName}",
            _ => throw new InvalidOperationException(
                $"Entorno '{environmentName}' no reconocido para resolver la base de datos de área '{AreaName}'.")
        };
    }
}
