// <copyright file="StudentStatisticsViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    20/03/2025 23:35 PM
// Purpose:         Defines the StudentStatisticsViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the statistics of students in a course, including enrollment and completion data.
    /// </summary>
    public record StudentStatisticsViewModel
    {
        /// <summary>
        /// Gets or sets the number of active students currently enrolled in the course.
        /// </summary>
        public int ActiveStudentCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of students enrolled in the course.
        /// </summary>
        public int TotalEnrolled { get; set; }

        /// <summary>
        /// Gets or sets the number of students who have dropped out of the course.
        /// </summary>
        public int DroppedOut { get; set; }

        /// <summary>
        /// Gets or sets the number of students who have successfully completed the course.
        /// </summary>
        public int Completed { get; set; }
    }

}
