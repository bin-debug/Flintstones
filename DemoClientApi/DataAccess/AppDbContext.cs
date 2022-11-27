using DemoClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoClientApi.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
    }
}
