
namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a student entry used for communication purposes, including identification and contact information.
    /// </summary>
    /// <param name="StudentId">The unique identifier of the student.</param>
    /// <param name="StudentNumber">The official student number assigned to the student.</param>
    /// <param name="FullName">The full name of the student.</param>
    /// <param name="Email">The email address of the student, or null if not available.</param>
    /// <param name="Cellphone">The cellphone number of the student, or null if not available.</param>
    public sealed record CommunicationStudentLookupItem(Guid StudentId, string StudentNumber, string FullName, string? Email, string? Cellphone);
}
