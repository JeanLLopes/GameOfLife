using GameOfLife.Api.Extensions;
using GameOfLife.Core.Interfaces;
using GameOfLife.Core.Services;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<GameOfLifeSettings>(
            builder.Configuration.GetSection("GameOfLifeSettings"));

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<GameOfLifeContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
        builder.Services.AddScoped<IBoardRepository, BoardRepository>();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Game of Life API", Version = "v1" });
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<GameOfLifeContext>();
            var databaseProvider = dbContext.Database.ProviderName;
            if (databaseProvider != "Microsoft.EntityFrameworkCore.InMemory")
            {
                dbContext.Database.Migrate();
            }
            else
            {
                dbContext.Database.EnsureCreated();
            }
        }

        app.Run();
    }
}