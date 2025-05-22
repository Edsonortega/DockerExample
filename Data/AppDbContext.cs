using DockerExample.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerExample.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    }
}