
using Microsoft.EntityFrameworkCore;
using StudentCourseEFCore.Models;
using StudentCourseEFCore.Repositories;

var departmentRepository = new DepartmentRepository();
var studentRepository = new StudentRepository();

while (true)
{
    Console.WriteLine("\n===== STUDENT COURSE MANAGEMENT =====");
    Console.WriteLine("1. Department Management");
    Console.WriteLine("2. Student Management");
    Console.WriteLine("0. Exit");

    Console.Write("\nEnter your choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Invalid input. Enter a number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            await ManageDepartmentsAsync(departmentRepository);
            break;

        case 2:
            await ManageStudentsAsync(studentRepository, departmentRepository);
            break;

        case 0:
            Console.WriteLine("Exiting application. Goodbye!");
            return;

        default:
            Console.WriteLine("Invalid choice. Select 0, 1, or 2.");
            break;
    }
}

// ================= DEPARTMENT MANAGEMENT =================

static async Task ManageDepartmentsAsync(
    DepartmentRepository repository)
{
    while (true)
    {
        Console.WriteLine("\n===== DEPARTMENT MANAGEMENT =====");
        Console.WriteLine("1. Add Department");
        Console.WriteLine("2. View All Departments");
        Console.WriteLine("3. View Department By ID");
        Console.WriteLine("4. Update Department");
        Console.WriteLine("5. Delete Department");
        Console.WriteLine("0. Back to Main Menu");

        Console.Write("\nEnter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid input. Enter a number.");
            continue;
        }

        switch (choice)
        {
            case 1:
                {
                    string name = ReadRequiredInput(
                        "Enter department name: ");

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

                        Console.WriteLine(
                            $"Department added successfully. ID: {id}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (DbUpdateException)
                    {
                        Console.WriteLine(
                            "A database error occurred while adding the department.");
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

                    Console.WriteLine(
                        "\nID | Department Name | Location | Active");

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
                    if (!TryReadId(
                            "Enter department ID: ", out int id))
                    {
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
                    if (!TryReadId(
                            "Enter department ID to update: ", out int id))
                    {
                        break;
                    }

                    string name = ReadRequiredInput(
                        "Enter new department name: ");

                    Console.Write("Enter new location: ");
                    string? location = Console.ReadLine();

                    location = string.IsNullOrWhiteSpace(location)
                        ? null
                        : location.Trim();

                    try
                    {
                        bool updated = await repository.UpdateAsync(
                            id, name, location);

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
                        Console.WriteLine(
                            "A database error occurred while updating the department.");
                    }

                    break;
                }

            case 5:
                {
                    if (!TryReadId(
                            "Enter department ID to delete: ", out int id))
                    {
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

                    if (!string.Equals(
                            Console.ReadLine(),
                            "y",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Deletion cancelled.");
                        break;
                    }

                    try
                    {
                        bool deleted = await repository.DeleteAsync(id);

                        Console.WriteLine(
                            deleted
                                ? "Department deleted successfully."
                                : "Department not found.");
                    }
                    catch (DbUpdateException)
                    {
                        Console.WriteLine(
                            "Cannot delete this department. " +
                            "It may have students assigned to it.");
                    }

                    break;
                }

            case 0:
                return;

            default:
                Console.WriteLine("Invalid choice. Select an option from 0 to 5.");
                break;
        }
    }
}

// ================= STUDENT MANAGEMENT =================

static async Task ManageStudentsAsync(
    StudentRepository repository,
    DepartmentRepository departmentRepository)
{
    while (true)
    {
        Console.WriteLine("\n===== STUDENT MANAGEMENT =====");
        Console.WriteLine("1. Add Student");
        Console.WriteLine("2. View All Students");
        Console.WriteLine("3. View Student By ID");
        Console.WriteLine("4. Update Student");
        Console.WriteLine("5. Delete Student");
        Console.WriteLine("0. Back to Main Menu");

        Console.Write("\nEnter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid input. Enter a number.");
            continue;
        }

        switch (choice)
        {
            case 1:
                {
                    var departments =
                        await departmentRepository.GetAllAsync();

                    if (departments.Count == 0)
                    {
                        Console.WriteLine(
                            "Create a department before adding students.");
                        break;
                    }

                    DisplayDepartments(departments);

                    string name = ReadRequiredInput("Enter student name: ");
                    string email = ReadRequiredInput("Enter student email: ");

                    if (!TryReadAge(out int age))
                    {
                        break;
                    }

                    if (!TryReadId(
                            "Enter department ID: ", out int departmentId))
                    {
                        break;
                    }

                    if (!departments.Any(
                            d => d.DepartmentId == departmentId))
                    {
                        Console.WriteLine("Department not found.");
                        break;
                    }

                    var student = new Student
                    {
                        Name = name,
                        Email = email,
                        Age = age,
                        DepartmentId = departmentId
                    };

                    try
                    {
                        int id = await repository.AddAsync(student);

                        Console.WriteLine(
                            $"Student added successfully. ID: {id}");
                    }
                    catch (DbUpdateException)
                    {
                        Console.WriteLine(
                            "Could not add student. Check the email " +
                            "and database constraints.");
                    }

                    break;
                }

            case 2:
                {
                    var students = await repository.GetAllAsync();

                    if (students.Count == 0)
                    {
                        Console.WriteLine("No students found.");
                        break;
                    }

                    DisplayStudents(students);
                    break;
                }

            case 3:
                {
                    if (!TryReadId(
                            "Enter student ID: ", out int id))
                    {
                        break;
                    }

                    var student = await repository.GetByIdAsync(id);

                    if (student is null)
                    {
                        Console.WriteLine("Student not found.");
                        break;
                    }

                    DisplayStudent(student);
                    break;
                }

            case 4:
                {
                    if (!TryReadId(
                            "Enter student ID to update: ", out int id))
                    {
                        break;
                    }

                    var existingStudent =
                        await repository.GetByIdAsync(id);

                    if (existingStudent is null)
                    {
                        Console.WriteLine("Student not found.");
                        break;
                    }

                    var departments =
                        await departmentRepository.GetAllAsync();

                    if (departments.Count == 0)
                    {
                        Console.WriteLine("No departments available.");
                        break;
                    }

                    DisplayDepartments(departments);

                    string name = ReadRequiredInput(
                        "Enter new student name: ");

                    string email = ReadRequiredInput(
                        "Enter new student email: ");

                    if (!TryReadAge(out int age))
                    {
                        break;
                    }

                    if (!TryReadId(
                            "Enter new department ID: ",
                            out int departmentId))
                    {
                        break;
                    }

                    if (!departments.Any(
                            d => d.DepartmentId == departmentId))
                    {
                        Console.WriteLine("Department not found.");
                        break;
                    }

                    try
                    {
                        bool updated = await repository.UpdateAsync(
                            id, name, email, age, departmentId);

                        Console.WriteLine(
                            updated
                                ? "Student updated successfully."
                                : "Student not found.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (DbUpdateException)
                    {
                        Console.WriteLine(
                            "Could not update student. Check the email " +
                            "and database constraints.");
                    }

                    break;
                }

            case 5:
                {
                    if (!TryReadId(
                            "Enter student ID to delete: ", out int id))
                    {
                        break;
                    }

                    var student = await repository.GetByIdAsync(id);

                    if (student is null)
                    {
                        Console.WriteLine("Student not found.");
                        break;
                    }

                    Console.Write(
                        $"Delete student '{student.Name}'? (y/n): ");

                    if (!string.Equals(
                            Console.ReadLine(),
                            "y",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Deletion cancelled.");
                        break;
                    }

                    bool deleted = await repository.DeleteAsync(id);

                    Console.WriteLine(
                        deleted
                            ? "Student deleted successfully."
                            : "Student not found.");

                    break;
                }

            case 0:
                return;

            default:
                Console.WriteLine("Invalid choice. Select an option from 0 to 5.");
                break;
        }
    }
}

// ================= HELPER METHODS =================

static void DisplayDepartments(List<Department> departments)
{
    Console.WriteLine("\nAvailable Departments");
    Console.WriteLine("ID | Department Name");

    foreach (var department in departments)
    {
        Console.WriteLine(
            $"{department.DepartmentId} | {department.DepartmentName}");
    }
}

static void DisplayStudents(List<Student> students)
{
    Console.WriteLine(
        "\nID | Name | Email | Age | Department");

    foreach (var student in students)
    {
        Console.WriteLine(
            $"{student.StudentId} | " +
            $"{student.Name} | " +
            $"{student.Email} | " +
            $"{student.Age} | " +
            $"{student.Department?.DepartmentName ?? "Not loaded"}");
    }
}

static void DisplayStudent(Student student)
{
    Console.WriteLine("\nStudent Details");
    Console.WriteLine($"ID: {student.StudentId}");
    Console.WriteLine($"Name: {student.Name}");
    Console.WriteLine($"Email: {student.Email}");
    Console.WriteLine($"Age: {student.Age}");
    Console.WriteLine($"Department ID: {student.DepartmentId}");
    Console.WriteLine(
        $"Department: {student.Department?.DepartmentName ?? "Not loaded"}");
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

static bool TryReadId(string message, out int id)
{
    Console.Write(message);

    if (int.TryParse(Console.ReadLine(), out id) && id > 0)
    {
        return true;
    }

    Console.WriteLine("Enter a valid positive ID.");
    return false;
}

static bool TryReadAge(out int age)
{
    Console.Write("Enter student age: ");

    if (int.TryParse(Console.ReadLine(), out age)
        && age >= 17
        && age <= 60)
    {
        return true;
    }

    Console.WriteLine("Age must be between 17 and 60.");
    return false;
}
