namespace GameOfLife.Core.Entities;

public class Board
{
    public Guid Id { get; private set; }
    public bool[][] State { get; private set; }
    public int Generation { get; private set; }
    public bool IsStable { get; private set; }

    private Board()
    {
        State = Array.Empty<bool[]>();
    }

    public Board(bool[][] initialState)
    {
        Id = Guid.NewGuid();
        State = initialState;
        Generation = 1;
        IsStable = false;
    }

    public void SetNextState(bool[][] newState, bool isStable)
    {
        State = newState;
        Generation++;
        IsStable = isStable;
    }
}