FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi/QimErp.Proxy.Mobile.WebApi.csproj src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi/
COPY src/Modules/Mobile/QimErp.Proxy.Mobile.Shared/QimErp.Proxy.Mobile.Shared.csproj src/Modules/Mobile/QimErp.Proxy.Mobile.Shared/

RUN dotnet restore src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi/QimErp.Proxy.Mobile.WebApi.csproj --source https://api.nuget.org/v3/index.json

COPY . .
RUN dotnet publish src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi/QimErp.Proxy.Mobile.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build-env /app/publish .

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
RUN chown -R appuser:appgroup /app
USER appuser

HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 CMD curl -f http://localhost:8080/ready || exit 1

ENTRYPOINT ["dotnet", "QimErp.Proxy.Mobile.WebApi.dll"]
