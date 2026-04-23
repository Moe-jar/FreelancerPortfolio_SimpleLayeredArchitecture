using FreelancerPortfolio.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTechnology> ProjectTechnologies => Set<ProjectTechnology>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AdminUser
        modelBuilder.Entity<AdminUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).IsRequired();
            e.Property(x => x.JwtId).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.AdminUser)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(x => x.AdminUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Slug).IsUnique();
        });

        // Technology
        modelBuilder.Entity<Technology>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            e.Property(x => x.IconUrl).HasMaxLength(500);
            e.HasIndex(x => x.Slug).IsUnique();
        });

        // Project
        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(220).IsRequired();
            e.Property(x => x.ShortDescription).HasMaxLength(500);
            e.Property(x => x.ThumbnailUrl).HasMaxLength(500);
            e.Property(x => x.LiveUrl).HasMaxLength(500);
            e.Property(x => x.RepoUrl).HasMaxLength(500);
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasOne(x => x.Category)
             .WithMany(c => c.Projects)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ProjectTechnology (many-to-many join)
        modelBuilder.Entity<ProjectTechnology>(e =>
        {
            e.HasKey(x => new { x.ProjectId, x.TechnologyId });
            e.HasOne(x => x.Project)
             .WithMany(p => p.ProjectTechnologies)
             .HasForeignKey(x => x.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Technology)
             .WithMany(t => t.ProjectTechnologies)
             .HasForeignKey(x => x.TechnologyId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
