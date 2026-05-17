// <copyright file="SqliteStudentDataSource.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         SQLite-backed student data source

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Reads students from the SQLite cache via <see cref="IStudentCacheStore"/>.
    /// </summary>
    public class SqliteStudentDataSource : IStudentDataSource
    {
        private readonly IStudentCacheStore _cacheStore;

        public SqliteStudentDataSource(IStudentCacheStore cacheStore)
        {
            _cacheStore = cacheStore;
        }

        public string SourceName => "SQLite";

        public async Task<List<Student>> GetAllStudentsAsync(CancellationToken ct = default)
        {
            return await _cacheStore.GetCachedStudentsAsync();
        }

        public async Task<Student?> GetStudentByIdentityAsync(string idOrPassport, CancellationToken ct = default)
        {
            var students = await _cacheStore.GetCachedStudentsAsync();
            return students.FirstOrDefault(s =>
                string.Equals(s.IDNumber?.Trim(), idOrPassport.Trim(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(s.PassportNumber?.Trim(), idOrPassport.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}