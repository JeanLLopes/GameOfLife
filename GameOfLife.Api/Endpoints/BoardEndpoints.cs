using GameOfLife.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Endpoints;

public static class BoardEndpoints
{
    public class GameOfLifeSettings
    {
        public int MaxSimulationAttempts { get; set; } = 1000;
    }

    public static void MapBoardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/boards");

        group.MapPost("/", async (bool[][] initialState, IBoardRepository repo) =>
        {
            if (initialState == null || initialState.Length == 0 || initialState[0].Length == 0)
            {
                return Results.BadRequest("O estado inicial não pode ser vazio.");
            }

            var board = await repo.CreateAsync(initialState);
            return Results.Created($"/api/boards/{board.Id}", board);
        })
        .WithName("UploadBoard")
        .WithTags("GameOfLife");

        group.MapGet("/{id:guid}/next", async (Guid id, IBoardRepository repo, IGameOfLifeService service) =>
        {
            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado.");

            var nextBoard = service.CalculateNextState(board);
            await repo.UpdateAsync(nextBoard);

            return Results.Ok(nextBoard);
        })
        .WithName("GetNextState")
        .WithTags("GameOfLife");

        group.MapGet("/{id:guid}/states/{count:int}", async (Guid id, int count, IBoardRepository repo, IGameOfLifeService service) =>
        {
            if (count <= 0) return Results.BadRequest("O número de estados deve ser positivo.");

            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado.");

            for (int i = 0; i < count; i++)
            {
                if (board.IsStable) break;
                board = service.CalculateNextState(board);
            }

            await repo.UpdateAsync(board); 
            return Results.Ok(board);
        })
        .WithName("GetXStatesAway")
        .WithTags("GameOfLife");

        group.MapGet("/{id:guid}/final", async (
            Guid id,
            IBoardRepository repo,
            IGameOfLifeService service,
            IOptions<GameOfLifeSettings> settings) =>
        {
            var maxAttempts = settings.Value.MaxSimulationAttempts; 
            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado.");

            for (int i = 0; i < maxAttempts; i++)
            {
                if (board.IsStable)
                {
                    await repo.UpdateAsync(board);
                    return Results.Ok(board);
                }
                board = service.CalculateNextState(board);
            }

            return Results.BadRequest($"O board não atingiu um estado estável após {maxAttempts} tentativas.");
        })
        .WithName("GetFinalState")
        .WithTags("GameOfLife");
    }
}