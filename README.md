# Conway's Game of Life API

This is a RESTful API implementation to simulate "Conway's Game of Life", as part of a coding challenge.

![Recording+2025-10-19+215818](https://github.com/user-attachments/assets/0063ac90-5a06-4830-8d7d-bf23ae391b8e)

## Features

- Upload an initial board state and get an ID.
- Query next, nth, and final (stable) board states.
- Board states are persisted using SQL Server.
- API built with .NET 7, following Clean Architecture.
- Integrated Swagger documentation.
- Supports running locally or via Docker Compose.

## Endpoints

1. **POST /api/boards**: Upload a new board (`bool[][]` JSON).
2. **GET /api/boards/{id}/next**: Get the next simulation state.
3. **GET /api/boards/{id}/states/{x}**: Get the state after `x` generations.
4. **GET /api/boards/{id}/final**: Get the final (stable) state.
5. **Persistence**: Board states survive API restarts.

## Architecture

- **GameOfLife.Core**: Business logic and entities.
- **GameOfLife.Infrastructure**: EF Core persistence, SQL Server.
- **GameOfLife.Api**: ASP.NET Core API, DI, error handling.
- **GameOfLife.Core.Tests**: Unit tests for simulation logic.

## Running Locally

### Prerequisites

- .NET 7 SDK
- SQL Server (local or Docker)

### Steps

1. Clone the repository.
2. Set your SQL Server connection string in `appsettings.json`.
3. Apply EF Core migrations:
    ```bash
    dotnet tool install --global dotnet-ef
    cd src/GameOfLife.Infrastructure
    dotnet ef database update --startup-project ../GameOfLife.Api
    cd ../..
    ```
4. Run the API:
    ```bash
    dotnet run --project src/GameOfLife.Api
    ```
5. Access Swagger at `http://localhost:<port>/swagger`.

## Running with Docker Compose

1. Build and start containers:
    ```bash
    docker-compose up --build
    ```
2. API available at `http://localhost:8080/swagger`.

## Testing

- Integration tests use an in-memory database and are located in `tests/GameOfLife.Api.IntegrationTests`.
- Unit tests for business logic are in `GameOfLife.Core.Tests`.

## Notes

- Board state is stored as JSON in the database for simplicity.
- For large boards, further optimizations may be needed.
