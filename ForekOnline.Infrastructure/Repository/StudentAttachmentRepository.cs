// <copyright file="StudentAttachmentRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:54 AM
// Purpose:         Defines the StudentAttachmentRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the StudentAttachment Repository.
    /// </summary>
    public class StudentAttachmentRepository : Repository<StudentAttachment>, IStudentAttachment
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentAttachmentRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public StudentAttachmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing StudentAttachment model in the repository.
        /// </summary>
        /// <param name="studentAttachment">The StudentAttachment model to be updated.</param>
        public async Task<StudentAttachment> UpdateStudentAttachmentAsync(StudentAttachment studentAttachment)
        {
            _context.StudentAttachments.Update(studentAttachment);

            await _context.SaveChangesAsync();

            return studentAttachment;
        }
    }
}
