using Insumos.Application.DTOs;
using Insumos.Application.Services;
using Insumos.Domain.Exceptions;
using Insumos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.EnvironmentName);

var app = builder.Build();

app.UseCors();

await app.Services.InitializeDatabaseAsync(app.Configuration, app.Environment.EnvironmentName);

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "app-almacen-insumos-api"
}));

var insumosGroup = app.MapGroup("/api/insumos");

insumosGroup.MapGet("/", async (IInsumoService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

insumosGroup.MapPost("/", async (CreateInsumoRequest request, IInsumoService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(request, ct);
        return Results.Created($"/api/insumos/{created.Id}", created);
    }
    catch (ConflictException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (DomainException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

insumosGroup.MapDelete("/{id:guid}", async (Guid id, IInsumoService service, CancellationToken ct) =>
{
    try
    {
        await service.DeleteAsync(id, ct);
        return Results.NoContent();
    }
    catch (NotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
});

app.Run();

public partial class Program;
