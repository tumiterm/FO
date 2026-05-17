// <copyright file="IForekBaseModels.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    26-06-2024 06:16:00 AM
// Purpose:         Defines the IForekBaseModels interface.

#region Usings
using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IForekBaseModels : IRepository<ForekBaseModel>
    {
        Task<ForekBaseModel> Update(ForekBaseModel forekBase);
    }
}
