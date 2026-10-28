using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

var repository = new DepartmentRepository();

// CREATE
var department = new Department
{
    DepartmentName = "Computer Science",
    Location = "Chennai"
};

int id = await repository.AddAsync(department);

Console.WriteLine($"Department created with ID: {id}");

// READ ALL
Console.WriteLine("\nAll Departments:");

var departments = await repository.GetAllAsync();

foreach (var d in departments)
{
    Console.WriteLine(
        $"{d.DepartmentId} | {d.DepartmentName} | {d.Location} | {d.IsActive}");
}

// READ BY ID
Console.WriteLine("\nDepartment by ID:");

var foundDepartment = await repository.GetByIdAsync(id);

if (foundDepartment is not null)
{
    Console.WriteLine(
        $"{foundDepartment.DepartmentId} | {foundDepartment.DepartmentName}");
}

// UPDATE
bool updated = await repository.UpdateAsync(
    id,
    "Computer Science and Engineering",
    "Coimbatore");

Console.WriteLine($"\nUpdate successful: {updated}");

// DELETE
// Keep this commented until you have inspected the created and updated records.
// bool deleted = await repository.DeleteAsync(id);
// Console.WriteLine($"Delete successful: {deleted}");
