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
            if (initialState == null || initialState.Length == 0 || initialState[0].Length == 0) // [cite: 33]
            {
                return Results.BadRequest("O estado inicial não pode ser vazio.");
            }

            var board = await repo.CreateAsync(initialState);
            // Retorna o ID e o objeto criado
            return Results.Created($"/api/boards/{board.Id}", board);
        })
        .WithName("UploadBoard")
        .WithTags("GameOfLife");

        group.MapGet("/{id:guid}/next", async (Guid id, IBoardRepository repo, IGameOfLifeService service) =>
        {
            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado."); // [cite: 32]

            var nextBoard = service.CalculateNextState(board);
            await repo.UpdateAsync(nextBoard); // Salva o novo estado

            return Results.Ok(nextBoard);
        })
        .WithName("GetNextState")
        .WithTags("GameOfLife");

        group.MapGet("/{id:guid}/states/{count:int}", async (Guid id, int count, IBoardRepository repo, IGameOfLifeService service) =>
        {
            if (count <= 0) return Results.BadRequest("O número de estados deve ser positivo."); // [cite: 33, 40]

            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado.");

            for (int i = 0; i < count; i++)
            {
                if (board.IsStable) break; // [cite: 53] Otimização
                board = service.CalculateNextState(board);
            }

            await repo.UpdateAsync(board); // Salva o estado final da simulação
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
            var maxAttempts = settings.Value.MaxSimulationAttempts; // [cite: 23]
            var board = await repo.GetByIdAsync(id);
            if (board == null) return Results.NotFound($"Board com ID {id} não encontrado.");

            for (int i = 0; i < maxAttempts; i++)
            {
                if (board.IsStable)
                {
                    await repo.UpdateAsync(board); // Salva o estado final
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