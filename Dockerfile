# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first (layer caching for dependencies)
COPY Asisya.Products.sln ./
COPY src/Asisya.Products.Domain/Asisya.Products.Domain.csproj             src/Asisya.Products.Domain/
COPY src/Asisya.Products.Application/Asisya.Products.Application.csproj   src/Asisya.Products.Application/
COPY src/Asisya.Products.Infrastructure/Asisya.Products.Infrastructure.csproj src/Asisya.Products.Infrastructure/
COPY src/Asisya.Products.API/Asisya.Products.API.csproj                   src/Asisya.Products.API/
COPY tests/Asisya.Products.Tests.Unit/Asisya.Products.Tests.Unit.csproj   tests/Asisya.Products.Tests.Unit/

# Restore dependencies
RUN dotnet restore src/Asisya.Products.API/Asisya.Products.API.csproj

# Copy all source code
COPY src/ src/

# Build & publish
RUN dotnet publish src/Asisya.Products.API/Asisya.Products.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Asisya.Products.API.dll"]
