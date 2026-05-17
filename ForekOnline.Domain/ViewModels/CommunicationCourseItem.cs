
namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a communication course with its unique identifier and name.
    /// </summary>
    /// <param name="CourseId">The unique identifier of the course.</param>
    /// <param name="CourseName">The name of the course.</param>
    public sealed record CommunicationCourseItem(Guid CourseId, string CourseName);
}
