using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Data;
using OnlineExamSystem;

namespace OnlineExamSystem.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove SQL Server registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OnlineExamSystemContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Use InMemory DB for tests
            services.AddDbContext<OnlineExamSystemContext>(options =>
            {
                options.UseInMemoryDatabase("OnlineExamSystem_TestDb");
            });

            // Ensure DB is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OnlineExamSystemContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}
