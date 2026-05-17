// <copyright file="LearnerViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the LearnerViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the details of a learner.
    /// </summary>
    public class LearnerViewModel
    {
        /// <summary>
        /// Gets or sets the name of the learner.
        /// </summary>
        public string LearnerName { get; set; }

        /// <summary>
        /// Gets or sets the status of the learner (e.g., Active, Inactive, Completed).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the start date of the learner's program or course.
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// Gets or sets the end date of the learner's program or course.
        /// </summary>
        public string End { get; set; }
    }

}
