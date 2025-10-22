using GameOfLife.Core.Entities;
using GameOfLife.Core.Interfaces;

namespace GameOfLife.Core.Services;

public class GameOfLifeService : IGameOfLifeService
{
    private const int MinLiveNeighborsForSurvival = 2;
    private const int MaxLiveNeighborsForSurvival = 3;
    private const int LiveNeighborsForBirth = 3;

    public Board CalculateNextState(Board currentBoard)
    {
        if (currentBoard.IsStable)
            return currentBoard;

        var currentState = currentBoard.State;
        var dimensions = GetBoardDimensions(currentState);
        var nextState = InitializeNextState(dimensions);
        var hasChanged = ProcessCellStates(currentState, nextState, dimensions);

        currentBoard.SetNextState(nextState, !hasChanged);
        return currentBoard;
    }

    private record struct BoardDimensions(int Rows, int Columns);

    private static BoardDimensions GetBoardDimensions(bool[][] state) =>
        new(state.Length, state[0].Length);

    private static bool[][] InitializeNextState(BoardDimensions dimensions)
    {
        var nextState = new bool[dimensions.Rows][];
        for (int i = 0; i < dimensions.Rows; i++)
        {
            nextState[i] = new bool[dimensions.Columns];
        }
        return nextState;
    }

    private bool ProcessCellStates(bool[][] currentState, bool[][] nextState, BoardDimensions dimensions)
    {
        bool hasChanged = false;

        for (int row = 0; row < dimensions.Rows; row++)
        {
            for (int col = 0; col < dimensions.Columns; col++)
            {
                int liveNeighbors = CountLiveNeighbors(currentState, row, col);
                bool isAlive = currentState[row][col];

                var newState = DetermineCellState(isAlive, liveNeighbors);
                nextState[row][col] = newState;

                if (newState != isAlive)
                {
                    hasChanged = true;
                }
            }
        }

        return hasChanged;
    }

    private static bool DetermineCellState(bool isAlive, int liveNeighbors)
    {
        if (isAlive)
        {
            return liveNeighbors >= MinLiveNeighborsForSurvival &&
                   liveNeighbors <= MaxLiveNeighborsForSurvival;
        }

        return liveNeighbors == LiveNeighborsForBirth;
    }

    private static int CountLiveNeighbors(bool[][] board, int row, int col)
    {
        const int NeighborhoodSize = 1; 

        var dimensions = new BoardDimensions(board.Length, board[0].Length);
        int count = 0;

        int startRow = Math.Max(0, row - NeighborhoodSize);
        int endRow = Math.Min(dimensions.Rows - 1, row + NeighborhoodSize);
        int startCol = Math.Max(0, col - NeighborhoodSize);
        int endCol = Math.Min(dimensions.Columns - 1, col + NeighborhoodSize);

        for (int r = startRow; r <= endRow; r++)
        {
            for (int c = startCol; c <= endCol; c++)
            {
                if (r == row && c == col)
                {
                    continue;
                }

                if (board[r][c])
                {
                    count++;
                }
            }
        }

        return count;
    }
}