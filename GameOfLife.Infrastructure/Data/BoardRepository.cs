using GameOfLife.Core.Entities;
using GameOfLife.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Data;

public class BoardRepository : IBoardRepository
{
    private readonly GameOfLifeContext _context;

    public BoardRepository(GameOfLifeContext context)
    {
        _context = context;
    }

    public async Task<Board> CreateAsync(bool[][] initialState)
    {
        var board = new Board(initialState);
        await _context.Boards.AddAsync(board);
        await _context.SaveChangesAsync(); 
        return board;
    }

    public async Task<Board?> GetByIdAsync(Guid id)
    {
        return await _context.Boards.FindAsync(id); 
    }

    public async Task UpdateAsync(Board board)
    {
        _context.Boards.Update(board);
        await _context.SaveChangesAsync(); 
    }
}