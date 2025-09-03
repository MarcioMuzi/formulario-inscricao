using Microsoft.EntityFrameworkCore;

namespace WebApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
}
