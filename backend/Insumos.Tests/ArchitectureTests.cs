using System.Reflection;
using Insumos.Application.Services;
using Insumos.Domain.Entities;

namespace Insumos.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnOtherApplicationLayersOrPersistence()
    {
        Assembly domainAssembly = typeof(Insumo).Assembly;
        var referencedNames = domainAssembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        Assert.DoesNotContain(referencedNames, name => name is not null && name.StartsWith("Insumos.Application", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.StartsWith("Insumos.Infrastructure", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.StartsWith("Insumos.Api", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.Contains("EntityFrameworkCore", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.Contains("MySql", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Application_ShouldOnlyDependOnDomainAmongProjectLayers()
    {
        Assembly applicationAssembly = typeof(InsumoService).Assembly;
        var referencedNames = applicationAssembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        Assert.Contains("Insumos.Domain", referencedNames);
        Assert.DoesNotContain(referencedNames, name => name is not null && name.StartsWith("Insumos.Infrastructure", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.StartsWith("Insumos.Api", StringComparison.Ordinal));
        Assert.DoesNotContain(referencedNames, name => name is not null && name.Contains("EntityFrameworkCore", StringComparison.Ordinal));
    }
}
