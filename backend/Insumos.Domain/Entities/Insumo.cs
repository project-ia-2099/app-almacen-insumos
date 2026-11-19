using Insumos.Domain.Exceptions;

namespace Insumos.Domain.Entities;

/// <summary>
/// Represents a warehouse supply item (insumo).
/// </summary>
public sealed class Insumo
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public decimal Stock { get; private set; }
    public string UnidadMedida { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    // Required by EF Core
    private Insumo()
    {
    }

    private Insumo(Guid id, string codigo, string nombre, decimal stock, string unidadMedida, DateTime createdAtUtc)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Stock = stock;
        UnidadMedida = unidadMedida;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Creates a validated <see cref="Insumo"/> instance enforcing all business invariants.
    /// </summary>
    public static Insumo Create(string codigo, string nombre, decimal stock, string unidadMedida)
    {
        var normalizedCodigo = NormalizeRequired(codigo, nameof(Codigo), 50).ToUpperInvariant();
        var normalizedNombre = NormalizeRequired(nombre, nameof(Nombre), 150);
        var normalizedUnidad = NormalizeRequired(unidadMedida, nameof(UnidadMedida), 30);

        if (stock < 0)
        {
            throw new DomainException("El stock no puede ser negativo.");
        }

        return new Insumo(
            Guid.NewGuid(),
            normalizedCodigo,
            normalizedNombre,
            stock,
            normalizedUnidad,
            DateTime.UtcNow);
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"El campo {fieldName} es obligatorio.");
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new DomainException($"El campo {fieldName} no puede superar los {maxLength} caracteres.");
        }

        return trimmed;
    }
}
