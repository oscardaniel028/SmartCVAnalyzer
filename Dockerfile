# ─── Etapa 1: Build ───────────────────────────────────────────────────────────
# Usamos la imagen del SDK para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos los archivos .csproj primero para aprovechar el cache de Docker
# Si los .csproj no cambian, Docker no vuelve a restaurar los paquetes
COPY ["src/SmartCVAnalyzer.API/SmartCVAnalyzer.API.csproj", "src/SmartCVAnalyzer.API/"]
COPY ["src/SmartCVAnalyzer.Application/SmartCVAnalyzer.Application.csproj", "src/SmartCVAnalyzer.Application/"]
COPY ["src/SmartCVAnalyzer.Domain/SmartCVAnalyzer.Domain.csproj", "src/SmartCVAnalyzer.Domain/"]
COPY ["src/SmartCVAnalyzer.Infrastructure/SmartCVAnalyzer.Infrastructure.csproj", "src/SmartCVAnalyzer.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "src/SmartCVAnalyzer.API/SmartCVAnalyzer.API.csproj"

# Copiar el resto del código fuente
COPY . .

# Compilar y publicar en modo Release
RUN dotnet publish "src/SmartCVAnalyzer.API/SmartCVAnalyzer.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Etapa 2: Runtime ─────────────────────────────────────────────────────────
# Usamos la imagen de runtime (más liviana, sin SDK)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear usuario no-root por seguridad
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copiar solo los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Exponer el puerto HTTP
EXPOSE 8080

# Variables de entorno base
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SmartCVAnalyzer.API.dll"]