using GameOfLife.Core.Entities;
using GameOfLife.Core.Interfaces;
using System.Security.Cryptography;

namespace GameOfLife.Core.Services;

public class GameOfLifeService : IGameOfLifeService
{
    public Board CalculateNextState(Board currentBoard)
    {
        if (currentBoard.IsStable)
            return currentBoard;

        var currentState = currentBoard.State;
        int rows = currentState.Length;
        int cols = currentState[0].Length;
        var nextState = new bool[rows][];
        bool hasChanged = false;

        for (int i = 0; i < rows; i++)
        {
            nextState[i] = new bool[cols];
            for (int j = 0; j < cols; j++)
            {
                int liveNeighbors = CountLiveNeighbors(currentState, i, j);
                bool isAlive = currentState[i][j];

                if (isAlive && (liveNeighbors < 2 || liveNeighbors > 3))
                {
                    nextState[i][j] = false; 
                    hasChanged = true;
                }
                else if (!isAlive && liveNeighbors == 3)
                {
                    nextState[i][j] = true;
                    hasChanged = true;
                }
                else
                {
                    nextState[i][j] = isAlive;
                }
            }
        }

        currentBoard.SetNextState(nextState, !hasChanged);
        return currentBoard;
    }

    private int CountLiveNeighbors(bool[][] board, int row, int col)
    {
        int count = 0;
        int rows = board.Length;
        int cols = board[0].Length;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int r = row + i;
                int c = col + j;

                if (r >= 0 && r < rows && c >= 0 && c < cols && board[r][c])
                {
                    count++;
                }
            }
        }
        return count;
    }
}