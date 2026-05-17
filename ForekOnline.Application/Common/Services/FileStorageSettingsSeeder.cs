using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Services
{
    public static class FileStorageSettingsSeeder
    {
        public static async Task SeedAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (unitOfWork is null) throw new ArgumentNullException(nameof(unitOfWork));

            var existingDefaults = await unitOfWork.FileStorage.GetAllAsync(
                s => s.IsDefault && s.IsActive && s.TenantId == null && s.DocumentType == null,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            if (existingDefaults.Any())
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;

            var seed = new FileStorageSetting
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                DocumentType = null,
                ProviderType = /*"AzureBlob"*/ "Database",
                ConnectionDetails = null,
                MaxSizeMB = 100,
                EncryptAtRest = true,
                Compress = false,
                AllowedMimeTypesJson = null,
                RetentionDays = 0,
                FallbackProviderType = null,
                CustomMetadataJson = null,
                IsDefault = true,
                IsActive = true,
                DateCreated = now,
                DateModified = now,
                CreatedBy = "seed",
                ModifiedBy = "seed",
            };

            await unitOfWork.FileStorage.AddAsync(seed);
            await unitOfWork.SaveAsync();
        }
    }
}
