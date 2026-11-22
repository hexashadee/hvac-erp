# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY NullSupportERP.slnx ./

COPY src/NullSupportERP.Domain/NullSupportERP.Domain.csproj \
    src/NullSupportERP.Domain/

COPY src/NullSupportERP.Application/NullSupportERP.Application.csproj \
    src/NullSupportERP.Application/

COPY src/NullSupportERP.Infrastructure/NullSupportERP.Infrastructure.csproj \
    src/NullSupportERP.Infrastructure/

COPY src/NullSupportERP.Api/NullSupportERP.Api.csproj \
    src/NullSupportERP.Api/

RUN dotnet restore \
    src/NullSupportERP.Api/NullSupportERP.Api.csproj

COPY src/ src/

RUN dotnet publish \
    src/NullSupportERP.Api/NullSupportERP.Api.csproj \
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

ENTRYPOINT ["dotnet", "NullSupportERP.Api.dll"]
