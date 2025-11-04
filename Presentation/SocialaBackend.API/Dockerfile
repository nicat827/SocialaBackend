# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/SocialaBackend.API/SocialaBackend.API.csproj", "Presentation/SocialaBackend.API/"]
COPY ["Domain/SocialaBackend.Application/SocialaBackend.Application.csproj", "Domain/SocialaBackend.Application/"]
COPY ["Domain/SocialaBackend.Domain/SocialaBackend.Domain.csproj", "Domain/SocialaBackend.Domain/"]
COPY ["Infrastructure/SocialaBackend.Infrastructure/SocialaBackend.Infrastructure.csproj", "Infrastructure/SocialaBackend.Infrastructure/"]
COPY ["Infrastructure/SocialaBackend.Persistence/SocialaBackend.Persistence.csproj", "Infrastructure/SocialaBackend.Persistence/"]
RUN dotnet restore "./Presentation/SocialaBackend.API/SocialaBackend.API.csproj"
COPY . .
WORKDIR "/src/Presentation/SocialaBackend.API"
RUN dotnet build "./SocialaBackend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SocialaBackend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialaBackend.API.dll"]