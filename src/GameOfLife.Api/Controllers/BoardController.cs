using GameOfLife.Api.Extensions;
using GameOfLife.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Controllers;

/// <summary>
/// Controller for managing Conway's Game of Life board states and operations.
/// </summary>
[ApiController]
[Route("api/boards")]
[Produces("application/json")]
public class BoardController : ControllerBase
{
    private readonly IBoardRepository _repo;
    private readonly IGameOfLifeService _service;
    private readonly IOptions<GameOfLifeSettings> _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardController"/> class.
    /// </summary>
    /// <param name="repo">The board repository for persistence operations.</param>
    /// <param name="service">The Game of Life service for board state calculations.</param>
    /// <param name="settings">Configuration settings for the Game of Life.</param>
    public BoardController(
        IBoardRepository repo,
        IGameOfLifeService service,
        IOptions<GameOfLifeSettings> settings)
    {
        _repo = repo;
        _service = service;
        _settings = settings;
    }

    /// <summary>
    /// Creates a new Game of Life board with the specified initial state.
    /// </summary>
    /// <param name="initialState">A 2D boolean array representing the initial board state where true represents live cells.</param>
    /// <returns>A new board instance with the specified initial state.</returns>
    /// <response code="201">Returns the newly created board.</response>
    /// <response code="400">If the initial state is null or empty.</response>
    [HttpPost]
    public async Task<IActionResult> UploadBoard([FromBody] bool[][] initialState)
    {
        if (initialState == null || initialState.Length == 0 || initialState[0].Length == 0)
            return BadRequest("Initial state cannot be empty.");

        var board = await _repo.CreateAsync(initialState);
        return CreatedAtAction(nameof(GetNextState), new { id = board.Id }, board);
    }

    /// <summary>
    /// Calculates and returns the next state of a specific board.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <returns>The next state of the specified board.</returns>
    /// <response code="200">Returns the next state of the board.</response>
    /// <response code="404">If the board with the specified ID is not found.</response>
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

    /// <summary>
    /// Calculates and returns the board state after a specified number of generations.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <param name="count">The number of generations to advance.</param>
    /// <returns>The board state after the specified number of generations.</returns>
    /// <response code="200">Returns the board state after the specified number of generations.</response>
    /// <response code="400">If the count is not positive.</response>
    /// <response code="404">If the board with the specified ID is not found.</response>
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

    /// <summary>
    /// Calculates and returns the final stable state of the board or until maximum attempts are reached.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <returns>The final stable state of the board.</returns>
    /// <response code="200">Returns the final stable state of the board.</response>
    /// <response code="400">If the board does not reach a stable state within the maximum attempts.</response>
    /// <response code="404">If the board with the specified ID is not found.</response>
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
