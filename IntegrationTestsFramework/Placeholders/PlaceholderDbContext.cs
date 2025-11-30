using Microsoft.EntityFrameworkCore;

namespace IntegrationTestsFramework.Placeholders
{
    public class PlaceholderDbContext : DbContext
    {
        public DbSet<PlaceholderEntity> PlaceholderEntities { get; set; }
    }
}
