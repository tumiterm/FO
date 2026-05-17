// <copyright file="ApiStudentDataSource.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         API-backed student data source

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Reads students from the external API via <see cref="IStudentService"/>.
    /// </summary>
    public class ApiStudentDataSource : IStudentDataSource
    {
        private readonly IStudentService _studentService;

        public ApiStudentDataSource(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public string SourceName => "API";

        public async Task<List<Student>> GetAllStudentsAsync(CancellationToken ct = default)
        {
            return await _studentService.GetStudentListAsync();
        }

        public async Task<Student?> GetStudentByIdentityAsync(string idOrPassport, CancellationToken ct = default)
        {
            var students = await _studentService.GetStudentListAsync();
            return students.FirstOrDefault(s =>
                string.Equals(s.IDNumber?.Trim(), idOrPassport.Trim(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(s.PassportNumber?.Trim(), idOrPassport.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}