using ForekOnline.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IFileUploadService
    {
        Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default);

        Task<DownloadFileResponse> DownloadAsync(string fileId, CancellationToken cancellationToken = default);

        Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);

        Task<string> GeneratePresignedUrlAsync(string fileId, int expiryInMinutes, CancellationToken cancellationToken = default);
    }
}
