using GameOfLife.Core.Entities;

namespace GameOfLife.Core.Interfaces;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id);
    Task<Board> CreateAsync(bool[][] initialState);
    Task UpdateAsync(Board board);
}