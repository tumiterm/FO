// <copyright file="IStudentImportJob.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Interface for the Hangfire student import job

#region Usings
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Hangfire job contract for importing students from any data source into SQL Server.
    /// </summary>
    public interface IStudentImportJob
    {
        /// <summary>
        /// Executes the import. Called by Hangfire.
        /// </summary>
        Task ExecuteAsync(StudentImportPayload payload, CancellationToken ct = default);
    }
}