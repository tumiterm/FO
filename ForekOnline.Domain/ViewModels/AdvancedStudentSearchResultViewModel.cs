// <copyright file="AdvancedStudentSearchResultViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/02/2026 02:57 AM
// Purpose:         Defines the AdvancedStudentSearchResultViewModel

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the result of an advanced student search, including student details, course information, and
    /// enrollment status.
    /// </summary>
    /// <param name="StudentNumber">The unique identifier assigned to the student. Typically used for administrative and lookup purposes.</param>
    /// <param name="FullName">The full name of the student, including first and last names.</param>
    /// <param name="CourseTitle">The title of the course in which the student is enrolled.</param>
    /// <param name="CourseType">The type or category of the course, such as 'Undergraduate', 'Postgraduate', or 'Certificate'.</param>
    /// <param name="StartDate">The date when the student began the course.</param>
    /// <param name="IsActive">A value indicating whether the student is currently active in the course. <see langword="true"/> if active;
    /// otherwise, <see langword="false"/>.</param>
    public sealed record AdvancedStudentSearchResultViewModel(string StudentNumber, string FullName,
                           string CourseTitle, string CourseType, DateTime StartDate,bool IsActive);
}