// <copyright file="StoredDocumentRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    10-01-2026 18:07 PM
// Purpose:         Defines the StoredDocumentRepository Repository.

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the StoredDocument Repository.
    /// </summary>
    public class StoredDocumentRepository : Repository<StoredDocument>, IStoredDocument
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredDocumentRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public StoredDocumentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing storedDocument model in the repository.
        /// </summary>
        /// <param name="storedDocument">The StoredDocument model to be updated.</param>
        public async Task<StoredDocument> UpdateStoredDocumentAsync(StoredDocument storedDocument)
        {
            _context.StoredDocuments.Update(storedDocument);

            await _context.SaveChangesAsync();

            return storedDocument;
        }
    }
}
