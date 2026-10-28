using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Models;

namespace StudentCourseEFCore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
}

