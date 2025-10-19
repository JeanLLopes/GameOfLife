FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY src/GameOfLife.Api/GameOfLife.Api.csproj src/GameOfLife.Api/
COPY src/GameOfLife.Core/GameOfLife.Core.csproj src/GameOfLife.Core/
COPY src/GameOfLife.Infrastructure/GameOfLife.Infrastructure.csproj src/GameOfLife.Infrastructure/
RUN dotnet restore src/GameOfLife.Api/GameOfLife.Api.csproj

# Copy the rest of the source code and publish the application
COPY src/ ./src/
WORKDIR /src/GameOfLife.Api
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "GameOfLife.Api.dll"]