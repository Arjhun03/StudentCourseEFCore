using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

var repository = new DepartmentRepository();

while (true)
{
    Console.WriteLine("\n===== DEPARTMENT MANAGEMENT =====");
    Console.WriteLine("1. Add Department");
    Console.WriteLine("2. View All Departments");
    Console.WriteLine("3. View Department By ID");
    Console.WriteLine("4. Update Department");
    Console.WriteLine("5. Delete Department");
    Console.WriteLine("0. Exit");

    Console.Write("\nEnter your choice: ");
    string? input = Console.ReadLine();

    if (!int.TryParse(input, out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            {
                string name = ReadRequiredInput("Enter department name: ");

                Console.Write("Enter location: ");
                string? location = Console.ReadLine();

                var department = new Department
                {
                    DepartmentName = name,
                    Location = string.IsNullOrWhiteSpace(location)
                        ? null
                        : location.Trim()
                };

                try
                {
                    int id = await repository.AddAsync(department);
                    Console.WriteLine($"Department added successfully. ID: {id}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("A database error occurred while adding the department.");
                }
                break;
            }


        case 2:
            {
                var departments = await repository.GetAllAsync();

                if (departments.Count == 0)
                {
                    Console.WriteLine("No departments found.");
                    break;
                }

                Console.WriteLine("\nID | Department Name | Location | Active");

                foreach (var department in departments)
                {
                    Console.WriteLine(
                        $"{department.DepartmentId} | " +
                        $"{department.DepartmentName} | " +
                        $"{department.Location} | " +
                        $"{department.IsActive}");
                }

                break;
            }

        case 3:
            {
                Console.Write("Enter department ID: ");

                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                var department = await repository.GetByIdAsync(id);

                if (department is null)
                {
                    Console.WriteLine("Department not found.");
                    break;
                }

                Console.WriteLine(
                    $"{department.DepartmentId} | " +
                    $"{department.DepartmentName} | " +
                    $"{department.Location} | " +
                    $"{department.IsActive}");

                break;
            }

        case 4:
            {
                Console.Write("Enter department ID to update: ");

                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                string name = ReadRequiredInput("Enter new department name: ");

                Console.Write("Enter new location: ");
                string? location = Console.ReadLine();

                location = string.IsNullOrWhiteSpace(location)
                    ? null
                    : location.Trim();

                try
                {
                    bool updated = await repository.UpdateAsync(id, name, location);

                    Console.WriteLine(
                        updated
                            ? "Department updated successfully."
                            : "Department not found.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("A database error occurred while updating the department.");
                }

                break;
            }

        case 5:
            {
                Console.Write("Enter department ID to delete: ");

                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                var department = await repository.GetByIdAsync(id);

                if (department is null)
                {
                    Console.WriteLine("Department not found.");
                    break;
                }

                Console.Write(
                    $"Delete department '{department.DepartmentName}'? (y/n): ");

                string? confirmation = Console.ReadLine();

                if (!string.Equals(
                        confirmation, "y",
                        StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deletion cancelled.");
                    break;
                }

                bool deleted = await repository.DeleteAsync(id);

                Console.WriteLine(
                    deleted
                        ? "Department deleted successfully."
                        : "Department not found.");

                break;
            }

        case 0:
            Console.WriteLine("Exiting Department Management. Goodbye!");
            return;

        default:
            Console.WriteLine("Invalid choice. Select an option from 0 to 5.");
            break;
    }
}

static string ReadRequiredInput(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine("Input cannot be empty. Please try again.");
    }
}


