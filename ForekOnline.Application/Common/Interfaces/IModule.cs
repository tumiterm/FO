// <copyright file="IModule.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:41 PM
// Purpose:         Defines the IModule interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IModule : IRepository<Module>
    {
        Task<Module> UpdateModuleAsync(Module module);
    }
}
