using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem;
using OnlineExamSystem.Data;

namespace OnlineExamSystem.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1) Remove existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OnlineExamSystemContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            // 2) Remove SQL Server provider registrations
            var sqlProviderDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            foreach (var d in sqlProviderDescriptors)
                services.Remove(d);

            // 3) Register InMemory provider
            services.AddDbContext<OnlineExamSystemContext>(options =>
            {
                options.UseInMemoryDatabase("OnlineExamSystem_TestDb");
            });

            // 4) Build & init db
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OnlineExamSystemContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}
