# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY HvacERP.slnx ./

COPY src/HvacERP.Domain/HvacERP.Domain.csproj \
    src/HvacERP.Domain/

COPY src/HvacERP.Application/HvacERP.Application.csproj \
    src/HvacERP.Application/

COPY src/HvacERP.Infrastructure/HvacERP.Infrastructure.csproj \
    src/HvacERP.Infrastructure/

COPY src/HvacERP.Api/HvacERP.Api.csproj \
    src/HvacERP.Api/

RUN dotnet restore \
    src/HvacERP.Api/HvacERP.Api.csproj

COPY src/ src/

RUN dotnet publish \
    src/HvacERP.Api/HvacERP.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Production runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "HvacERP.Api.dll"]
