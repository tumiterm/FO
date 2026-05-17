// <copyright file="IReportSubReport.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-03-2026 21:04 PM
// Purpose:         Defines the IReportSubReport interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating reportSubReport entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to reportSubReport management.</remarks>
    public interface IReportSubReport : IRepository<ReportSubReport>
    {
        /// <summary>
        /// Updates the specified reportSubReport in the system and returns the updated reportSubReport.
        /// </summary>
        /// <param name="reportSubReport">The <see cref="ReportSubReport"/> object containing the updated reportSubReport details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ReportSubReport"/> object.</returns>
        Task<ReportSubReport> UpdateReportSubReportAsync(ReportSubReport reportSubReport);
    }
}
