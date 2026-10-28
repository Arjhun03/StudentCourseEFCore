namespace StudentCourseEFCore.Models;

public class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int Credits { get; set; }

    public bool IsActive { get; set; } = true;
}

