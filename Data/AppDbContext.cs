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
    }
}



