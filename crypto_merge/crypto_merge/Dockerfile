# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["crypto_merge/crypto_merge.csproj", "crypto_merge/"]
COPY ["BusLogic/BusLogic.csproj", "BusLogic/"]
COPY ["InternetDbContext/InternetDatabase.csproj", "InternetDbContext/"]
COPY ["crypto_merge.Tg.Bot/crypto_merge.Tg.Bot.csproj", "crypto_merge.Tg.Bot/"]
RUN dotnet nuget add source "https://pkgs.dev.azure.com/tgbots/Telegram.Bot/_packaging/release/nuget/v3/index.json" -n Telegram.Bot
RUN dotnet restore "./crypto_merge/crypto_merge.csproj"
COPY . .
WORKDIR "/src/crypto_merge"
RUN dotnet build "./crypto_merge.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./crypto_merge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "crypto_merge.dll"]