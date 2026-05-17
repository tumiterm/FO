using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Services
{
    public class DatabaseFileStorageProvider : IFileStorageProvider
    {
        private const string Provider = "Database";

        private readonly IUnitOfWork _unitOfWork;

        public DatabaseFileStorageProvider(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public string ProviderName => Provider;

        public async Task<DownloadFileResponse> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("fileId is required.", nameof(fileId));

            if (!Guid.TryParse(fileId, out var id))
                throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var storedDocument = await _unitOfWork.StoredDocument
                .GetAsync(d => d.Id == id && !d.IsDeleted, asNoTracking: true, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (storedDocument is null)
                throw new FileNotFoundException("The requested file does not exist.");

            var storedContent = await _unitOfWork.StoredDocumentContent
                .GetAsync(c => c.Id == id, asNoTracking: true, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (storedContent is null || storedContent.Content is null || storedContent.Content.Length == 0)
                throw new FileNotFoundException("The requested file content does not exist.");

            return new DownloadFileResponse(
                FileStream: new MemoryStream(storedContent.Content, writable: false),
                FileName: storedDocument.FileName,
                FileSizeBytes: storedDocument.FileSizeBytes,
                ContentType: storedDocument.ContentType);
        }

        public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (request.FileStream is null) throw new ArgumentException("FileStream is required.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.FileName)) throw new ArgumentException("FileName is required.", nameof(request));

            var fileId = Guid.NewGuid();

            byte[] content;

            using (var ms = new MemoryStream())
            {
                if (request.FileStream.CanSeek)
                {
                    request.FileStream.Position = 0;
                }

                await request.FileStream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
                content = ms.ToArray();
            }

            var metadataJson = request.Metadata is null ? null : JsonSerializer.Serialize(request.Metadata);

            var storedDocument = new StoredDocument
            {
                Id = fileId,
                ProviderName = ProviderName,
                ProviderKey = fileId.ToString("D"),
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSizeBytes = content.LongLength,
                MetadataJson = metadataJson,
                ExpiryDate = request.ExpiryDate,
                IsDeleted = false,
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedBy = "system",
            };

            var storedContent = new StoredDocumentContent
            {
                Id = fileId,
                Content = content,
            };

            await _unitOfWork.StoredDocument.AddAsync(storedDocument, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.StoredDocumentContent.AddAsync(storedContent, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveAsync().ConfigureAwait(false);

            return new UploadFileResponse(
               FileId: fileId.ToString("D"),
               UploadTimestamp: DateTimeOffset.UtcNow,
               ProviderName: ProviderName,
               FileSizeBytes: storedDocument.FileSizeBytes,
               FileUrl: null,
               AdditionalMetadata: request.Metadata);
        }

        public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("fileId is required.", nameof(fileId));

            if (!Guid.TryParse(fileId, out var id))
                throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var stored = await _unitOfWork.StoredDocument
                .GetAsync(d => d.Id == id && !d.IsDeleted, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (stored is null)
                return;

            stored.IsDeleted = true;
            stored.ModifiedOn = DateTimeOffset.UtcNow;
            stored.ModifiedBy = "system";

            await _unitOfWork.StoredDocument.UpdateStoredDocumentAsync(stored).ConfigureAwait(false);

            var storedContent = await _unitOfWork.StoredDocumentContent
                .GetAsync(c => c.Id == id, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (storedContent is not null)
            {
                await _unitOfWork.StoredDocumentContent.RemoveAsync(storedContent, cancellationToken).ConfigureAwait(false);
                await _unitOfWork.SaveAsync().ConfigureAwait(false);
            }
        }

        public Task<string> GeneratePresignedUrlAsync(string fileId, int expiryInMinutes, CancellationToken cancellationToken = default)
        {
            // Not applicable for DB storage. Caller uses FileUploadService guard anyway.
            return Task.FromResult(string.Empty);
        }
    }
}
