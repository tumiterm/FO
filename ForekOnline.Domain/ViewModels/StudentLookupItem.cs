// <copyright file="StudentLookupItem.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 16:01 PM
// Purpose:         Defines the StudentLookupItem model

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a student entry for lookup operations, containing the student's full name, email address, and student
    /// number.
    /// </summary>
    /// <param name="FullName">The full name of the student as it should appear in lookup results. Cannot be null.</param>
    /// <param name="Email">The email address associated with the student. Cannot be null.</param>
    /// <param name="StudentNumber">The unique student number assigned to the student. Cannot be null.</param>
    public sealed record StudentLookupItem(string FullName, string Email, string StudentNumber);
}
