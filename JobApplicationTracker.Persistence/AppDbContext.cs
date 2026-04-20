using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<JobApplication>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CompanyName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Position).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.HasOne(x => x.User)
                .WithMany(x => x.JobApplications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Note>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).HasMaxLength(1000).IsRequired();
            builder.HasOne(x => x.JobApplication)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Interview>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.Result).HasMaxLength(500);
            builder.HasOne(x => x.JobApplication)
                .WithMany(x => x.Interviews)
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
