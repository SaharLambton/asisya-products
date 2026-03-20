using Asisya.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Asisya.Products.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234!");
            context.Users.Add(new User("admin", "admin@asisya.com", adminHash, "Admin"));
            await context.SaveChangesAsync();
            logger.LogInformation("Default admin user seeded");
        }
    }
}
