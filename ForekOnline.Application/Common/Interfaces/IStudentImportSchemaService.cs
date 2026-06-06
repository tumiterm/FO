// <copyright file="IStudentImportSchemaService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Ensures that the SQL Server tables written by the student import match the
    /// current student domain model.
    /// </summary>
    public interface IStudentImportSchemaService
    {
        /// <summary>
        /// Applies safe, idempotent compatibility changes required by student imports.
        /// </summary>
        Task EnsureCompatibleAsync(CancellationToken ct = default);
    }
}
