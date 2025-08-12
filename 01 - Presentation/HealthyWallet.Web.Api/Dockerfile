FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HealthyWallet.Web.Api/HealthyWallet.Web.Api.csproj", "HealthyWallet.Web.Api/"]
RUN dotnet restore "HealthyWallet.Web.Api/HealthyWallet.Web.Api.csproj"
COPY . .
WORKDIR "/src/HealthyWallet.Web.Api"
RUN dotnet build "./HealthyWallet.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HealthyWallet.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthyWallet.Web.Api.dll"]
