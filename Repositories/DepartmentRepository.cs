
using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Data;
using StudentCourseEFCore.Models;

namespace StudentCourseEFCore.Repositories;

public class DepartmentRepository
{
    public async Task<int> AddAsync(Department department)
    {
        await using var db = new AppDbContext();

        db.Departments.Add(department);

        await db.SaveChangesAsync();

        return department.DepartmentId;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        await using var db = new AppDbContext();

        return await db.Departments
            .OrderBy(d => d.DepartmentId)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        await using var db = new AppDbContext();

        return await db.Departments.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(
        int id,
        string departmentName,
        string? location)
    {
        await using var db = new AppDbContext();

        var department = await db.Departments.FindAsync(id);

        if (department is null)
            return false;

        department.DepartmentName = departmentName;
        department.Location = location;

        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = new AppDbContext();

        var department = await db.Departments.FindAsync(id);

        if (department is null)
            return false;

        db.Departments.Remove(department);

        await db.SaveChangesAsync();

        return true;
    }
}
