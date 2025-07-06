using Microsoft.EntityFrameworkCore;
using TasksAPI.Models;

namespace TasksAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
}
