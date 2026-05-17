// <copyright file="IStudent.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:23 PM
// Purpose:         Defines the IStudent interface.

using ForekOnline.Domain.Entities;


namespace ForekOnline.Application.Common.Interfaces
{
    public interface IStudent : IRepository<Student>
    {
        Task<Student> UpdateStudentAsync(Student student);

    }
}
