// <copyright file="StudentListViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the StudentListViewModel class

using ForekOnline.Domain.Entities;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a view model for displaying a list of students with additional metadata.
    /// </summary>
    public class StudentListViewModel
    {

        /// <summary>
        /// Gets or sets the collection of students in this view model.
        /// </summary>
        public IEnumerable<Student> Students { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this student list.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the name associated with this student list.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets a value indicating whether this list should fall back due to a low number of students.
        /// If there are fewer than 5 students in the <see cref="Students"/> collection, this property returns <c>true</c>;
        /// otherwise, it returns <c>false</c>.
        /// </summary>
        public bool IsFallBackStudents
        {
            get
            {
                return Students != null && Students.Count() < 5;
            }
        }

        /// <summary>
        /// Gets or sets the total number of students.
        /// </summary>
        public int TotalCount { get; set; }
        public string Source { get; set; } = "Live";


    }
}
