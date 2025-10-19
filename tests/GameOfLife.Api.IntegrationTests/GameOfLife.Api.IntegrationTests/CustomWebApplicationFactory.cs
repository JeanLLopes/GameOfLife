using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GameOfLife.Infrastructure.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameOfLifeContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add DbContext with InMemory provider for tests
            services.AddDbContext<GameOfLifeContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Ensure database is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameOfLifeContext>();
            db.Database.EnsureCreated();
        });
    }
}