namespace GameOfLife.Api.Extensions;

public class GameOfLifeSettings
{
    public const int DefaultMaxSimulationAttempts = 1000;

    public int MaxSimulationAttempts { get; set; } = DefaultMaxSimulationAttempts;
}