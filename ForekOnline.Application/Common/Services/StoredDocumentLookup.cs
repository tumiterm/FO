// <copyright file="StoredDocumentLookup.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 10:09:27 AM
// Purpose:         Defines the StoredDocumentLookup class

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides methods for retrieving information about stored documents from the data store.
    /// </summary>
    /// <remarks>This class is intended for use in scenarios where document metadata, such as the provider
    /// name, must be retrieved by document identifier. Instances of this class are typically used within the
    /// application's data access layer and require an active unit of work for operation. This class is not
    /// thread-safe.</remarks>
    public sealed class StoredDocumentLookup : IStoredDocumentLookup
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the StoredDocumentLookup class using the specified unit of work.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance to be used for data operations. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if unitOfWork is null.</exception>
        public StoredDocumentLookup(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Asynchronously retrieves the provider name associated with the specified document identifier.
        /// </summary>
        /// <param name="documentId">The unique identifier of the document for which to retrieve the provider name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the provider name if the
        /// document exists and is not deleted; otherwise, null.</returns>
        public async Task<string?> GetProviderNameAsync(Guid documentId, CancellationToken cancellationToken = default)
        {
            var storedDocuments = await _unitOfWork.StoredDocument.GetAllAsync(
                                    d => d.Id == documentId && !d.IsDeleted,
                                    asNoTracking: true,
                                    cancellationToken: cancellationToken);

            return storedDocuments.Select(d => d.ProviderName).SingleOrDefault();

        }
    }
}
