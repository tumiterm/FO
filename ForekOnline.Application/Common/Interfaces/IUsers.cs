// <copyright file="IUsers.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    24-02-2025 20:50 PM
// Purpose:         Defines the IUsers interface.

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
    public interface IUsers : IRepository<User>
    {
        Task<User> UpdateUserAsync(User user);

    }
}
