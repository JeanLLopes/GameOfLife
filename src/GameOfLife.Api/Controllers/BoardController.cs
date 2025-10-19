using GameOfLife.Api.Extensions;
using GameOfLife.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Controllers;

[ApiController]
[Route("api/boards")]
[Produces("application/json")]
public class BoardController : ControllerBase
{
    private readonly IBoardRepository _repo;
    private readonly IGameOfLifeService _service;
    private readonly IOptions<GameOfLifeSettings> _settings;

    public BoardController(
        IBoardRepository repo,
        IGameOfLifeService service,
        IOptions<GameOfLifeSettings> settings)
    {
        _repo = repo;
        _service = service;
        _settings = settings;
    }

    [HttpPost]
    public async Task<IActionResult> UploadBoard([FromBody] bool[][] initialState)
    {
        if (initialState == null || initialState.Length == 0 || initialState[0].Length == 0)
            return BadRequest("Initial state cannot be empty.");

        var board = await _repo.CreateAsync(initialState);
        return CreatedAtAction(nameof(GetNextState), new { id = board.Id }, board);
    }

    [HttpGet("{id:guid}/next")]
    public async Task<IActionResult> GetNextState(Guid id)
    {
        var board = await _repo.GetByIdAsync(id);
        if (board == null)
            return NotFound($"Board with ID {id} not found.");

        var nextBoard = _service.CalculateNextState(board);
        await _repo.UpdateAsync(nextBoard);

        return Ok(nextBoard);
    }

    [HttpGet("{id:guid}/states/{count:int}")]
    public async Task<IActionResult> GetXStatesAway(Guid id, int count)
    {
        if (count <= 0)
            return BadRequest("Number of states must be positive.");

        var board = await _repo.GetByIdAsync(id);
        if (board == null)
            return NotFound($"Board with ID {id} not found.");

        for (int i = 0; i < count; i++)
        {
            if (board.IsStable) break;
            board = _service.CalculateNextState(board);
        }

        await _repo.UpdateAsync(board);
        return Ok(board);
    }

    [HttpGet("{id:guid}/final")]
    public async Task<IActionResult> GetFinalState(Guid id)
    {
        var maxAttempts = _settings.Value.MaxSimulationAttempts;
        var board = await _repo.GetByIdAsync(id);
        if (board == null)
            return NotFound($"Board with ID {id} not found.");

        for (int i = 0; i < maxAttempts; i++)
        {
            if (board.IsStable)
            {
                await _repo.UpdateAsync(board);
                return Ok(board);
            }
            board = _service.CalculateNextState(board);
        }

        return BadRequest($"Board did not reach a stable state after {maxAttempts} attempts.");
    }
}
