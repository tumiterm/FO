// <copyright file="IFile.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:03 PM
// Purpose:         Defines the IFile interface.

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IFile : IRepository<Domain.Entities.File>
    {
        Task<Domain.Entities.File> UpdateFileAsync(Domain.Entities.File file);
    }
}
