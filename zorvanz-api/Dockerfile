# ─── 1) BUILD ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos sólo el .csproj y restauramos
COPY ["zorvanz-api.csproj", "./"]
RUN dotnet restore "./zorvanz-api.csproj"

# Copiamos el resto del código
COPY . ./

# Publicamos (incluye build + publish)
RUN dotnet publish "zorvanz-api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ─── 2) RUNTIME ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Configura Kestrel para escuchar en 0.0.0.0:8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Traemos artefactos de publish
COPY --from=build /app/publish .

# Script para ejecutar migraciones y luego la aplicación
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
