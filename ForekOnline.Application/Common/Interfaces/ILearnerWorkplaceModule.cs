// <copyright file="ILearnerWorkplaceModule.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:32 PM
// Purpose:         Defines the ILearnerWorkplaceModule interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface ILearnerWorkplaceModule : IRepository<LearnerWorkplaceModules>
    {
        Task<LearnerWorkplaceModules> UpdateWorkPlaceModuleAsync(LearnerWorkplaceModules application);

    }
}
