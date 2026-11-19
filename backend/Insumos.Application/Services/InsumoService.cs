using Insumos.Application.DTOs;
using Insumos.Domain.Entities;
using Insumos.Domain.Exceptions;
using Insumos.Domain.Interfaces;

namespace Insumos.Application.Services;

/// <summary>
/// Application use cases for managing warehouse supplies.
/// </summary>
public interface IInsumoService
{
    Task<IReadOnlyList<InsumoDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InsumoDto> CreateAsync(CreateInsumoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of <see cref="IInsumoService"/>.
/// </summary>
public sealed class InsumoService : IInsumoService
{
    private readonly IInsumoRepository _repository;

    public InsumoService(IInsumoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InsumoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<InsumoDto> CreateAsync(CreateInsumoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var insumo = Insumo.Create(
            request.Codigo,
            request.Nombre,
            request.Stock,
            request.UnidadMedida);

        if (await _repository.ExistsByCodigoAsync(insumo.Codigo, cancellationToken))
        {
            throw new ConflictException($"Ya existe un insumo registrado con el código '{insumo.Codigo}'.");
        }

        await _repository.AddAsync(insumo, cancellationToken);
        return MapToDto(insumo);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var insumo = await _repository.GetByIdAsync(id, cancellationToken);
        if (insumo is null)
        {
            throw new NotFoundException($"No se encontró el insumo con identificador '{id}'.");
        }

        await _repository.DeleteAsync(insumo, cancellationToken);
    }

    private static InsumoDto MapToDto(Insumo insumo) =>
        new(
            insumo.Id,
            insumo.Codigo,
            insumo.Nombre,
            insumo.Stock,
            insumo.UnidadMedida,
            insumo.CreatedAtUtc);
}
