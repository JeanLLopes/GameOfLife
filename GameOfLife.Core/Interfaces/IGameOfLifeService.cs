using GameOfLife.Core.Entities;

namespace GameOfLife.Core.Interfaces;

public interface IGameOfLifeService
{
    Board CalculateNextState(Board currentBoard);
}