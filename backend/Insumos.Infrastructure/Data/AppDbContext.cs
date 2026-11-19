using Insumos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Insumos.Infrastructure.Data;

/// <summary>
/// Entity Framework Core context for the Almacén Insumos module.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Insumo> Insumos => Set<Insumo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.ToTable("insumos_items");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Codigo)
                .IsUnique();

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Stock)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(e => e.UnidadMedida)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.CreatedAtUtc)
                .IsRequired();
        });
    }
}
