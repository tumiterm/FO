// <copyright file="IEvidence.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:17 PM
// Purpose:         Defines the IEvidence interface.

using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IEvidence : IRepository<Evidence>
    {
        Task<Evidence> UpdateEvidenceAsync(Evidence evidence);

    }
}
