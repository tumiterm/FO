using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Holds student data submitted directly from the enrollment form (walk-in or approved applicant).
    /// </summary>
    public class DirectStudentData
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? IDNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? StudyPermitNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Cellphone { get; set; }
        public string? Email { get; set; }
        public string? HighestGrade { get; set; }
        public string? NameOfSchool { get; set; }
        public string? StreetAddressLine1 { get; set; }
        public string? StreetAddressLine2 { get; set; }
    }
}
