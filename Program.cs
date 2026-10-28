using StudentCourseEFCore.Data;
using StudentCourseEFCore.Menus;
using StudentCourseEFCore.Repositories;
using StudentCourseEFCore.Services;

// 1. Create repositories
var departmentRepository = new DepartmentRepository();
var studentRepository = new StudentRepository();

using var courseDbContext = new AppDbContext();
var courseRepository = new CourseRepository(courseDbContext);

var enrollmentRepository = new EnrollmentRepository();

// 2. Create services
var departmentService =
    new DepartmentService(departmentRepository);

var studentService =
    new StudentService(studentRepository);

var courseService =
    new CourseService(courseRepository);

var enrollmentService =
    new EnrollmentService(enrollmentRepository);

// 3. Create menus
var departmentMenu =
    new DepartmentMenu(departmentService);

var studentMenu =
    new StudentMenu(studentService, departmentService);

var courseMenu =
    new CourseMenu(courseService);

var enrollmentMenu =
    new EnrollmentMenu(
        enrollmentService,
        studentService,
        courseService);

// 4. Main menu
while (true)
{
    Console.WriteLine("\n===== STUDENT COURSE MANAGEMENT =====");
    Console.WriteLine("1. Department Management");
    Console.WriteLine("2. Student Management");
    Console.WriteLine("3. Course Management");
    Console.WriteLine("4. Enrollment Management");
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
            await departmentMenu.ShowAsync();
            break;

        case 2:
            await studentMenu.ShowAsync();
            break;

        case 3:
            courseMenu.Show();
            break;

        case 4:
            await enrollmentMenu.ShowAsync();
            break;

        case 0:
            Console.WriteLine("Application closed.");
            return;

        default:
            Console.WriteLine(
                "Invalid choice. Select an option from 0 to 4.");
            break;
    }
}
