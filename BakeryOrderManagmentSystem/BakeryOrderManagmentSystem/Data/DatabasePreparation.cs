using Microsoft.EntityFrameworkCore;

namespace BakeryOrderManagmentSystem.Data
{
    public static class DatabasePreparation
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<BakeryDbContext>();

            try
            {
                Console.WriteLine("Applying migrations...");
                dataContext.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
            }
        }
    }
}