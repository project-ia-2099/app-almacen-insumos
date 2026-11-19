using Insumos.Domain.Entities;
using Insumos.Domain.Interfaces;
using Insumos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Insumos.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Insumo"/>.
/// </summary>
public sealed class InsumoRepository : IInsumoRepository
{
    private readonly AppDbContext _dbContext;

    public InsumoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Insumo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insumos
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Insumo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insumos
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insumos
            .AsNoTracking()
            .AnyAsync(i => i.Codigo == codigo, cancellationToken);
    }

    public async Task AddAsync(Insumo insumo, CancellationToken cancellationToken = default)
    {
        await _dbContext.Insumos.AddAsync(insumo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Insumo insumo, CancellationToken cancellationToken = default)
    {
        _dbContext.Insumos.Remove(insumo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
