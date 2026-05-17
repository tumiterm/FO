// <copyright file="IStudentDataSource.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Strategy interface for student data sources

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Abstraction over the source of student data.
    /// Implementations: API, SQLite cache, or direct form input.
    /// </summary>
    public interface IStudentDataSource
    {
        /// <summary>
        /// The source identifier (e.g. "API", "SQLite", "Direct").
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Retrieves all students from this data source.
        /// </summary>
        Task<List<Student>> GetAllStudentsAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves a single student by ID number or passport.
        /// </summary>
        Task<Student?> GetStudentByIdentityAsync(string idOrPassport, CancellationToken ct = default);
    }
}