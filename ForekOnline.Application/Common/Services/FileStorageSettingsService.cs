// <copyright file="FileStorageSettingsService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    10/01/2026 21:04:27 PM
// Purpose:         Defines the FileStorageSettingsService class

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to resolve file storage settings based on tenant and document type criteria.
    /// </summary>
    /// <remarks>This service is typically used to retrieve the most appropriate file storage configuration
    /// for a given tenant and document type. It applies a fallback strategy to select the best matching settings,
    /// considering both tenant-specific and default configurations. Instances of this class require an implementation
    /// of IUnitOfWork to access storage settings data.</remarks>
    public class FileStorageSettingsService : IFileStorageSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the FileStorageSettingsService class with the specified unit of work.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance used to manage data operations for file storage settings. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if unitOfWork is null.</exception>
        public FileStorageSettingsService(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Resolves the most appropriate file storage setting for the specified tenant and document type
        /// asynchronously.
        /// </summary>
        /// <remarks>The method attempts to find the most specific file storage setting based on the
        /// provided tenant and document type. If no exact match is found, it falls back to less specific settings in
        /// the following order: tenant and document type, tenant only, document type only, and finally the default
        /// global setting. The search is case-sensitive for document type and prioritizes the most recently created
        /// setting when multiple matches exist.</remarks>
        /// <param name="tenantId">The unique identifier of the tenant to resolve the file storage setting for, or null to resolve a global
        /// setting.</param>
        /// <param name="documentType">The document type to resolve the file storage setting for, or null to resolve a setting not specific to a
        /// document type. Leading and trailing whitespace is ignored.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the resolved file storage
        /// setting, or null if no matching setting is found.</returns>
        /// <exception cref="NotImplementedException">This exception is not expected to be thrown in normal usage. It is included as a safeguard and should not
        /// occur.</exception>
        public async Task<FileStorageSetting?> ResolveAsync(Guid? tenantId, string? documentType, CancellationToken cancellationToken = default)
        {
            documentType = string.IsNullOrWhiteSpace(documentType) ? null : documentType.Trim();

            var baseQuery = await _unitOfWork.FileStorage.GetAllAsync(
                          s => s.IsDefault, asNoTracking: true,
                          cancellationToken: cancellationToken);

            if (tenantId is null && documentType is null)
            {
                return baseQuery
                    .Where(s => s.TenantId == null && s.DocumentType == null)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();
            }

            var match = baseQuery
                .Where(s => s.TenantId == tenantId && s.DocumentType == documentType)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            if (match is not null) return match;
            if (tenantId is not null)
            {
                match = baseQuery
                    .Where(s => s.TenantId == tenantId && s.DocumentType == null)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();

                if (match is not null) return match;
            }

            if (documentType is not null)
            {
                match = baseQuery
                    .Where(s => s.TenantId == null && s.DocumentType == documentType)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();

                if (match is not null) return match;
            }

            return baseQuery
                .Where(s => s.TenantId == null && s.DocumentType == null)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
        }
    }
}
