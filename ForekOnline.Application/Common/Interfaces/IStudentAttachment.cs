// <copyright file="IStudentAttachment.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:42 AM
// Purpose:         Defines the IStudentAttachment interface.


using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IStudentAttachment : IRepository<StudentAttachment>
    {
        Task<StudentAttachment> UpdateStudentAttachmentAsync(StudentAttachment studentAttachment);
    }
}
