
#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the print details for a student, including student information and optional placement data.
    /// </summary>
    /// <param name="Student">The student whose print details are being represented. Cannot be null.</param>
    /// <param name="Placement">The placement information associated with the student, or null if no placement is assigned.</param>
    public sealed record StudentPrintDetailsViewModel(Student Student, Placement? Placement);
}
