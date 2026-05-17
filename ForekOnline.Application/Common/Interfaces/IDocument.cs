// <copyright file="IDocument.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:15 PM
// Purpose:         Defines the IDocument interface.

using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IDocument : IRepository<Document>
    {
        Task<Document> UpdateDocumentAsync(Document document);

    }
}
