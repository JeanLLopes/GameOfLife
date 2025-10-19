FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY src/GameOfLife.Api/GameOfLife.Api.csproj GameOfLife.Api/
COPY src/GameOfLife.Core/GameOfLife.Core.csproj GameOfLife.Core/
COPY src/GameOfLife.Infrastructure/GameOfLife.Infrastructure.csproj GameOfLife.Infrastructure/
COPY src/ ./

WORKDIR /src/GameOfLife.Api

RUN dotnet restore --configfile ../nuget.config --disable-parallel /p:RestoreNoFallbackFolder=true \
    && dotnet publish -c Release -o /app --no-restore --configfile ../nuget.config /p:RestoreNoFallbackFolder=true

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "GameOfLife.Api.dll"]
