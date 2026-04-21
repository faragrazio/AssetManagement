# Stage 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia i file .csproj e ripristina i pacchetti NuGet
COPY src/AssetManagement.Domain/AssetManagement.Domain.csproj src/AssetManagement.Domain/
COPY src/AssetManagement.Application/AssetManagement.Application.csproj src/AssetManagement.Application/
COPY src/AssetManagement.Infrastructure/AssetManagement.Infrastructure.csproj src/AssetManagement.Infrastructure/
COPY src/AssetManagement.API/AssetManagement.API.csproj src/AssetManagement.API/

RUN dotnet restore src/AssetManagement.API/AssetManagement.API.csproj

# Copia tutto il codice sorgente e compila
COPY . .
RUN dotnet publish src/AssetManagement.API/AssetManagement.API.csproj \
    -c Release -o /app/publish --no-restore

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copia solo l'output del publish — non l'intero SDK
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AssetManagement.API.dll"]