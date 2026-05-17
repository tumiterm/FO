// <copyright file="ApplicationsDashboardViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the ApplicationsDashboardViewModel class

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the data required for the dashboard view.
    /// </summary>
    public class ApplicationsDashboardViewModel
    {
        /// <summary>
        /// Gets or sets the total number of applications submitted in the current month.
        /// </summary>
        public int TotalApplicationsForCurrentMonth { get; set; }

        /// <summary>
        /// Gets or sets the total number of male applicants.
        /// </summary>
        public int TotalMales { get; set; }

        /// <summary>
        /// Gets or sets the total number of female applicants.
        /// </summary>
        public int TotalFemales { get; set; }


        /// <summary>
        /// Gets or sets the name of the program that received the highest number of applications.
        /// </summary>
        public string MostAppliedProgram { get; set; }

        /// <summary>
        /// Gets or sets the monthly trend of applications for the current year.
        /// The dictionary key represents the month name, and the value represents the number of applications.
        /// </summary>
        public Dictionary<string, int> MonthlyApplicationsTrend { get; set; }

        /// <summary>
        /// Gets or sets the gender distribution of applicants.
        /// The dictionary key represents the gender (e.g., "Male", "Female"), and the value represents the count.
        /// </summary>
        public Dictionary<string, int> GenderDistribution { get; set; }

        /// <summary>
        /// Gets or sets the top 5 Course applied - of all time
        /// </summary>
        public IReadOnlyList<string> Top5ProgramsAppliedFor { get; set; } = new List<string>();
    }
}
