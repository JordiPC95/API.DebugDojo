using Microsoft.EntityFrameworkCore;
using API.DebugDojo.Application.Entities;

namespace API.DebugDojo.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<WorkItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000);

            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            b.Property(x => x.Priority).HasConversion<string>().HasMaxLength(50);

            b.Property(x => x.RowVersion).IsRowVersion();

            b.HasOne(x => x.OwnerUser)
             .WithMany()
             .HasForeignKey(x => x.OwnerUserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(x => !x.IsDeleted); // Soft deletee global
        });
    }
}
