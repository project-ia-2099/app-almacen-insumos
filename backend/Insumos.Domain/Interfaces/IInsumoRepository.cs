using Insumos.Domain.Entities;

namespace Insumos.Domain.Interfaces;

/// <summary>
/// Persistence contract for <see cref="Insumo"/> aggregates.
/// </summary>
public interface IInsumoRepository
{
    Task<IReadOnlyList<Insumo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Insumo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task AddAsync(Insumo insumo, CancellationToken cancellationToken = default);
    Task DeleteAsync(Insumo insumo, CancellationToken cancellationToken = default);
}
