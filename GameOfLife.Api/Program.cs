using GameOfLife.Api.Endpoints;
using GameOfLife.Api.Extensions;
using GameOfLife.Core.Interfaces;
using GameOfLife.Core.Services;
using GameOfLife.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static GameOfLife.Api.Endpoints.BoardEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Adiciona configuração para ler appsettings.json
builder.Services.Configure<GameOfLifeSettings>(
    builder.Configuration.GetSection("GameOfLifeSettings")); // [cite: 72]

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameOfLifeContext>(options =>
    options.UseSqlite(connectionString)); // Usando SQLite para fácil portabilidade

// Adiciona serviços de domínio e infraestrutura
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();

// Adiciona serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => // [cite: 86]
{
    c.SwaggerDoc("v1", new() { Title = "Game of Life API", Version = "v1" });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // [cite: 35]

var app = builder.Build();

// 2. Configuração do Pipeline de HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Mapeia os endpoints definidos na classe BoardEndpoints
app.MapBoardEndpoints(); // [cite: 99]

// Aplica migrações do EF Core na inicialização
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameOfLifeContext>();
    dbContext.Database.Migrate();
}

app.Run();