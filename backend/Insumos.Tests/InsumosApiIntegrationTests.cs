using System.Net;
using System.Net.Http.Json;
using Insumos.Application.DTOs;
using Insumos.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Insumos.Tests;

public sealed class InsumosWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"InsumosTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=local_almacen;"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}

public sealed class InsumosApiIntegrationTests : IClassFixture<InsumosWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InsumosApiIntegrationTests(InsumosWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CrudFlow_CreateListAndDeleteInsumo_Succeeds()
    {
        var createRequest = new CreateInsumoRequest("ins-001", "Guantes de Nitrilo", 150, "Cajas");
        var createResponse = await _client.PostAsJsonAsync("/api/insumos", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<InsumoDto>();
        Assert.NotNull(created);
        Assert.Equal("INS-001", created.Codigo);
        Assert.Equal("Guantes de Nitrilo", created.Nombre);
        Assert.Equal(150, created.Stock);
        Assert.Equal("Cajas", created.UnidadMedida);

        var listResponse = await _client.GetAsync("/api/insumos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var items = await listResponse.Content.ReadFromJsonAsync<List<InsumoDto>>();
        Assert.NotNull(items);
        Assert.Contains(items, i => i.Id == created.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/insumos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var secondDeleteResponse = await _client.DeleteAsync($"/api/insumos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateInsumo_DuplicateCodigo_ReturnsConflict()
    {
        var request = new CreateInsumoRequest("DUP-100", "Casco de Seguridad", 25, "Unidades");
        var first = await _client.PostAsJsonAsync("/api/insumos", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var duplicate = await _client.PostAsJsonAsync("/api/insumos", request);
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [Theory]
    [InlineData("", "Mascarilla KN95", 10, "Cajas")]
    [InlineData("INS-002", "", 10, "Cajas")]
    [InlineData("INS-003", "Mascarilla KN95", -5, "Cajas")]
    [InlineData("INS-004", "Mascarilla KN95", 10, "")]
    public async Task CreateInsumo_InvalidBusinessInput_ReturnsBadRequest(
        string codigo,
        string nombre,
        decimal stock,
        string unidadMedida)
    {
        var request = new CreateInsumoRequest(codigo, nombre, stock, unidadMedida);
        var response = await _client.PostAsJsonAsync("/api/insumos", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
