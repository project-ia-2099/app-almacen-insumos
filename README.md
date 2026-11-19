# Gestión de Insumos - Área Almacén (`app-almacen-insumos`)

Aplicación web corporativa sin login para el área de **Almacén**, que permite **registrar, listar y eliminar insumos** (`Codigo`, `Nombre`, `Stock`, `UnidadMedida`).

---

## Arquitectura y Stack Tecnológico

- **Backend (`backend/`):** `.NET 10` (`net10.0`) Minimal APIs en **Clean Architecture** de 4 capas:
  - `Insumos.Domain`: Entidad `Insumo`, excepciones de dominio e interfaz `IInsumoRepository`.
  - `Insumos.Application`: DTOs (`InsumoDto`, `CreateInsumoRequest`) y casos de uso (`InsumoService`).
  - `Insumos.Infrastructure`: `AppDbContext`, `InsumoRepository`, migraciones EF Core (`__EFMigrationsHistory_insumos`) y tabla `insumos_items` sobre MySQL (`local_almacen` / `qa_almacen` / `prod_almacen`).
  - `Insumos.Api`: Minimal APIs (`GET /health`, `GET /api/insumos`, `POST /api/insumos`, `DELETE /api/insumos/{id}`).
  - `Insumos.Tests`: Pruebas de arquitectura (`ArchitectureTests`) e integración (`InsumosApiIntegrationTests`) con `xUnit`.
- **Frontend (`frontend/`):** React + TypeScript (Vite) sobre **Node.js 26 LTS**, diseño Minimalista Pastel, iconografía con `@fortawesome/fontawesome-free` y pruebas con `Vitest` + `@testing-library/react`.

---

## Ejecución y Pruebas Locales

### Backend
```bash
dotnet test backend/ --configuration Release
dotnet run --project backend/Insumos.Api
```
*(Requiere `ConnectionStrings__DefaultConnection` configurado hacia MySQL local).*

### Frontend
```bash
cd frontend
npm install
npm test
npm run build
```
*(Configurar `VITE_API_URL` apuntando a la URL de la API).*
