# Conway's Game of Life API

This is a RESTful API implementation to simulate "Conway's Game of Life", as part of a coding challenge.

## Problem Description

The API allows uploading an initial board (grid) state and subsequently querying the next simulation states according to the Game of Life rules.

### Functional Requirements

1.  **[POST] /api/boards**: Uploads a new board state (JSON `bool[][]`) and returns an ID.
2.  **[GET] /api/boards/{id}/next**: Returns the next simulation state for a board.
3.  **[GET] /api/boards/{id}/states/{x}**: Returns the board state after `x` simulations.
4.  **[GET] /api/boards/{id}/final**: Returns the final (stable) state of the board. Returns an error if it does not stabilize after `X` attempts.
5.  **Persistence**: The state of the boards is maintained even if the API restarts.

## Solution Overview and Architecture Decisions

The solution is built using .NET 7 and follows **Clean Architecture** principles to ensure modularity, testability, and separation of concerns (SoC).

* **`GameOfLife.Core`**: Contains pure business logic (Game of Life rules) and entities (`Board`). No infrastructure dependencies (web or database).
* **`GameOfLife.Infrastructure`**: Implements persistence using Entity Framework Core and a SQL Server database. Implements the `IBoardRepository` interface defined in Core.
* **`GameOfLife.Api`**: Exposes functionality via Controllers (ASP.NET Core). Responsible for Dependency Injection, error handling, and routing.
* **`GameOfLife.Core.Tests`**: Unit tests for business logic, ensuring correctness of simulation rules.

## Assumptions and Trade-offs

* **State Persistence**: To persist the `bool[][]` in the database, an EF Core `ValueConverter` is used to serialize the array to a JSON string. This is simple and effective, but less "queryable" than normalizing into `(Cell, X, Y)` tables.
* **Database**: The project now uses **SQL Server** for persistence. The connection string can be configured for your environment.
* **Performance**: The simulation algorithm is $O(r * c)$ per generation. For very large boards, optimizations (like "sparse matrix" or "hashlife") would be necessary, but are out of scope for this challenge.

## How to Run Locally

### Prerequisites
* .NET 7.0 SDK
* SQL Server (local or Docker)

### Steps

1.  Clone the repository.
2.  Open a terminal in the solution root folder.
3.  **Update the database (EF Core migrations):**
    ```bash
    # Install the EF Core global tool if you don't have it
    dotnet tool install --global dotnet-ef

    # Navigate to the infrastructure project
    cd src/GameOfLife.Infrastructure

    # Apply migrations (ensure your connection string is set for SQL Server)
    dotnet ef database update --startup-project ../GameOfLife.Api

    # Go back to the root
    cd ../..
    ```
4.  **Run the API:**
    ```bash
    dotnet run --project src/GameOfLife.Api
    ```
5.  The API will be available at `http://localhost:<port>`.
6.  Access `http://localhost:<port>/swagger` to view the API documentation and test endpoints.

---

### Run with Docker and Docker Compose

1.  Build and start the containers:
    ```bash
    docker-compose up --build
    ```
2.  The API will be available at `http://localhost:8080/swagger`.

**Note:**  
- The `docker-compose.yml` file configures both the API and a SQL Server instance.
- The connection string is set to connect to the SQL Server container by default.
- The default SQL Server password is set in the compose file; change it as needed for your environment.