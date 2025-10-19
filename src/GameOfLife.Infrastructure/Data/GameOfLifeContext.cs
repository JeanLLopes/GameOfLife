using GameOfLife.Core.Entities;
using GameOfLife.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure.Data;

public class GameOfLifeContext : DbContext
{
    public GameOfLifeContext(DbContextOptions<GameOfLifeContext> options) : base(options) { }

    public DbSet<Board> Boards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.State)
                .HasConversion(new BoardStateConverter()) 
                .IsRequired();
        });
    }
}