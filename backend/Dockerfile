FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Insumos.Domain/Insumos.Domain.csproj Insumos.Domain/
COPY Insumos.Application/Insumos.Application.csproj Insumos.Application/
COPY Insumos.Infrastructure/Insumos.Infrastructure.csproj Insumos.Infrastructure/
COPY Insumos.Api/Insumos.Api.csproj Insumos.Api/
RUN dotnet restore Insumos.Api/Insumos.Api.csproj

COPY . .
RUN dotnet publish Insumos.Api/Insumos.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Insumos.Api.dll"]
