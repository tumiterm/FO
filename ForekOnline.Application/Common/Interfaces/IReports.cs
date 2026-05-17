// <copyright file="IReports.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    26-06-2025 22:10 PM
// Purpose:         Defines the IReports interface.

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
    public interface IReports : IRepository<Report>
    {
        Task<Report> UpdateReportAsync(Report report);

    }
}
