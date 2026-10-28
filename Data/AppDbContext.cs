using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Models;

namespace StudentCourseEFCore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=StudentCourseEFCoreDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Department entity configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.DepartmentId);

            entity.Property(d => d.DepartmentName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(d => d.DepartmentName)
                .IsUnique();

            entity.Property(d => d.Location)
                .HasMaxLength(100);

            entity.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // Student-Department relationship
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Students)
            .WithOne(s => s.Department)
            .HasForeignKey(s => s.DepartmentId);

        // Student entity configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.StudentId);

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasIndex(s => s.Email)
                .IsUnique();

            // Database-level age validation
            entity.ToTable("Students", table =>
            {
                table.HasCheckConstraint(
                    "CK_Students_Age",
                    "[Age] BETWEEN 17 AND 60");
            });
        });
    }
}