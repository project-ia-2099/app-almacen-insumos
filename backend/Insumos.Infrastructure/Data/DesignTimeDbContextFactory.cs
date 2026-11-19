using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Insumos.Infrastructure.Data;

/// <summary>
/// Design-time factory for generating EF Core migrations without a live MySQL connection.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySQL(
            "Server=localhost;Database=local_almacen;",
            mysql => mysql.MigrationsHistoryTable("__EFMigrationsHistory_insumos"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
