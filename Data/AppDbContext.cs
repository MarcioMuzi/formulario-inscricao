using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Degree> Degrees => Set<Degree>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Degree: apenas constraints; seed ficou na migration
        builder.Entity<Degree>(e =>
        {
            e.Property(d => d.Name).HasMaxLength(50).IsRequired();
        });

        // Enrollment: limites e FK apontando explicitamente para a navegação 'Degree'
        builder.Entity<Enrollment>(e =>
        {
            e.Property(x => x.MobilePhone).HasMaxLength(11).IsRequired();
            e.Property(x => x.Cpf).HasMaxLength(11).IsRequired();
            e.Property(x => x.Gender).HasMaxLength(20).IsRequired();
            e.Property(x => x.Organization).HasMaxLength(200).IsRequired();

            e.HasOne(x => x.Degree)              // <-- referencia a navegação
             .WithMany()                         // sem coleção no principal
             .HasForeignKey(x => x.DegreeId)     // FK concreta
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
