FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/GameOfLife.Api/*.csproj src/GameOfLife.Api/
COPY src/GameOfLife.Core/*.csproj src/GameOfLife.Core/
COPY src/GameOfLife.Infrastructure/*.csproj src/GameOfLife.Infrastructure/
RUN dotnet restore

COPY . .
WORKDIR "/src/src/GameOfLife.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER app
RUN mkdir -p /app/data
ENV ConnectionStrings__DefaultConnection "Data Source=/app/data/gameoflife.db"

ENTRYPOINT ["dotnet", "GameOfLife.Api.dll"]