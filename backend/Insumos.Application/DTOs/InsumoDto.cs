namespace Insumos.Application.DTOs;

/// <summary>
/// Read model for an insumo item.
/// </summary>
public sealed record InsumoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    decimal Stock,
    string UnidadMedida,
    DateTime CreatedAtUtc);

/// <summary>
/// Input model for registering a new insumo.
/// </summary>
public sealed record CreateInsumoRequest(
    string Codigo,
    string Nombre,
    decimal Stock,
    string UnidadMedida);
