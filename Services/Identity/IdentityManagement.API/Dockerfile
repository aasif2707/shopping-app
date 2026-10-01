# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_HTTP_PORTS=80
ENV ASPNETCORE_HTTPS_PORTS=443
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Identity/IdentityManagement.API/IdentityManagement.API.csproj", "Services/Identity/IdentityManagement.API/"]
COPY ["Services/Identity/IdentityManagement.Infrastructure/IdentityManagement.Infrastructure.csproj", "Services/Identity/IdentityManagement.Infrastructure/"]
COPY ["Services/Identity/IdentityManagement.Application/IdentityManagement.Application.csproj", "Services/Identity/IdentityManagement.Application/"]
COPY ["Services/Identity/IdentityManagement.Domain/IdentityManagement.Core.csproj", "Services/Identity/IdentityManagement.Domain/"]
COPY ["Shared/Shared.Logging.Serilog/Shared.Logging.Serilog.csproj", "Shared/Shared.Logging.Serilog/"]
RUN dotnet restore "./Services/Identity/IdentityManagement.API/IdentityManagement.API.csproj"
COPY . .
WORKDIR "/src/Services/Identity/IdentityManagement.API"
RUN dotnet build "./IdentityManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityManagement.API.dll"]