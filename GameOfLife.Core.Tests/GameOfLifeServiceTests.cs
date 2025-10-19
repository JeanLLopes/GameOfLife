using FluentAssertions;
using GameOfLife.Core.Entities;
using GameOfLife.Core.Services;

namespace GameOfLife.Core.Tests;

public class GameOfLifeServiceTests
{
    private readonly GameOfLifeService _service;

    public GameOfLifeServiceTests()
    {
        _service = new GameOfLifeService();
    }

    [Fact]
    public void CalculateNextState_StillLifeBlock_ShouldNotChange()
    {
        // Arrange
        var initialState = new bool[][]
        {
            new[] { false, false, false, false },
            new[] { false, true,  true,  false },
            new[] { false, true,  true,  false },
            new[] { false, false, false, false }
        };
        var board = new Board(initialState);

        // Act
        var nextBoard = _service.CalculateNextState(board);

        // Assert
        nextBoard.IsStable.Should().BeTrue();
        nextBoard.State.Should().BeEquivalentTo(initialState);
        nextBoard.Generation.Should().Be(2);
    }

    [Fact]
    public void CalculateNextState_OscillatorBlinker_ShouldOscillate() // [cite: 40] Edge case
    {
        // Arrange
        var initialState = new bool[][]
        {
            new[] { false, false, false },
            new[] { true,  true,  true  },
            new[] { false, false, false }
        };
        var board = new Board(initialState);

        // Act
        var nextBoard = _service.CalculateNextState(board);
        var expectedState1 = new bool[][]
        {
            new[] { false, true,  false },
            new[] { false, true,  false },
            new[] { false, true,  false }
        };

        // Assert
        nextBoard.IsStable.Should().BeFalse();
        nextBoard.Generation.Should().Be(2);
        nextBoard.State.Should().BeEquivalentTo(expectedState1);

        // Act 
        var finalBoard = _service.CalculateNextState(nextBoard);

        // Assert
        finalBoard.IsStable.Should().BeFalse();
        finalBoard.Generation.Should().Be(3);
        finalBoard.State.Should().BeEquivalentTo(initialState); 
    }

    [Fact]
    public void CalculateNextState_CellWithTwoNeighbors_ShouldSurvive()
    {
        // Arrange
        var initialState = new bool[][]
        {
            new[] { true, true, false },
            new[] { true, false, false },
            new[] { false, false, false }
        };
        var board = new Board(initialState);

        // Act
        var nextBoard = _service.CalculateNextState(board);

        // Assert
        nextBoard.State[0][0].Should().BeTrue();
    }

    [Fact]
    public void CalculateNextState_CellWithOneNeighbor_ShouldDie() 
    {
        // Arrange
        var initialState = new bool[][]
        {
            new[] { true, false, false },
            new[] { false, true,  false },
            new[] { false, false, false }
        };
        var board = new Board(initialState); 

        // Act
        var nextBoard = _service.CalculateNextState(board);

        // Assert
        nextBoard.State[0][0].Should().BeFalse();
    }
}