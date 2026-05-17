#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    public class ResourceService : IResourceService
    {
        private const string ResourceDocumentType = "ResourceLibrary";

        private readonly IUnitOfWork _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHelperService _helperService;
        private readonly IUserService _userService;

        public ResourceService(IUnitOfWork context, IFileUploadService fileUploadService, IHelperService helperService, IUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task UploadResourceAsync(ResourceUploadViewModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.Name)) throw new ArgumentException("Resource name is required.", nameof(model.Name));

            var currentUser = _userService.OnGetCurrentUser() ?? throw new InvalidOperationException("No current user found.");
            string userFullName = $"{currentUser.Name} {currentUser.LastName}";

            string fileUrl;
            string? fileId = null;
            string? providerName = null;
            eResourceType type;
            string? externalUrl = null;

            if (model.File is not null && model.File.Length > 0)
            {
                UploadFileResponse upload;

                await using (var stream = model.File.OpenReadStream())
                {
                    upload = await _fileUploadService.UploadAsync(
                        new UploadFileRequest(
                            FileStream: stream,
                            FileName: model.File.FileName,
                            ContentType: model.File.ContentType,
                            Metadata: new Dictionary<string, string>
                            {
                                ["resourceName"] = model.Name,
                                ["categoryId"] = model.CategoryId.ToString(),
                                ["createdBy"] = userFullName,
                                ["tags"] = model.Tags ?? string.Empty
                            },
                            ProviderHint: null,
                            ExpiryDate: null,
                            TenantId: null,
                            DocumentType: ResourceDocumentType),
                        CancellationToken.None).ConfigureAwait(false);
                }

                fileId = upload.FileId;
                providerName = upload.ProviderName;
                fileUrl = upload.FileUrl ?? string.Empty;

                type = eResourceType.File;
            }
            else if (!string.IsNullOrWhiteSpace(model.ExternalUrl))
            {
                externalUrl = model.ExternalUrl.Trim();
                fileUrl = externalUrl;

                type = eResourceType.ExternalLink;
            }
            else
            {
                throw new ArgumentException("Please upload a file or provide a link.", nameof(model));
            }

            var resource = new Resource
            {
                Id = _helperService.GenerateGuid(),
                Name = model.Name,
                Description = model.Description ?? string.Empty,
                Type = type,
                FileURL = fileUrl,
                ExternalUrl = externalUrl,
                StoredFileId = fileId,
                StoredFileProvider = providerName,
                TagsCsv = model.Tags,
                CategoryId = model.CategoryId,
                IsActive = true,
                Code = _helperService.GenerateRandomString(5),
                DateCreated = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                DateModified = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                UserCreated = userFullName,
                UserModified = userFullName,
                IsPublic = model.IsPublic
            };

            if (!model.IsPublic && model.SelectedRoles.Count > 0)
            {
                foreach (var role in model.SelectedRoles.Distinct())
                {
                    if (role == eSysRole.None) continue;

                    resource.RoleAudiences.Add(new ResourceRoleAudience
                    {
                        Id = _helperService.GenerateGuid(),
                        ResourceId = resource.Id,
                        Role = role
                    });
                }
            }

            if (!model.IsPublic && model.SelectedUserIds.Count > 0)
            {
                foreach (var userId in model.SelectedUserIds.Distinct())
                {
                    resource.UserAudiences.Add(new ResourceUserAudience
                    {
                        Id = _helperService.GenerateGuid(),
                        ResourceId = resource.Id,
                        UserId = userId
                    });
                }
            }

            try
            {
                await _context.Resource.AddAsync(resource).ConfigureAwait(false);
                await _context.SaveAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(fileId))
                {
                    try { await _fileUploadService.DeleteAsync(fileId).ConfigureAwait(false); }
                    catch { }
                }

                throw new InvalidOperationException("Failed to save resource to database.", ex);
            }
        }

        public async Task<List<ResourceUploadViewModel>> LoadResourcesAsync()
        {
            var currentUser = _userService.OnGetCurrentUser() ?? throw new InvalidOperationException("No current user found.");

            var resources = await _context.Resource.GetAllAsync(
                filter: r =>
                    r.IsActive &&
                    (
                        r.IsPublic ||
                        (currentUser.Role.HasValue && r.RoleAudiences.Any(a => a.Role == currentUser.Role.Value)) ||
                        r.UserAudiences.Any(a => a.UserId == currentUser.Id)
                    ),
                includeProperties: new[]
                {
                    nameof(Resource.Category),
                    nameof(Resource.RoleAudiences),
                    nameof(Resource.UserAudiences)
                }).ConfigureAwait(false);

            return resources.Select(r => new ResourceUploadViewModel
            {
                ResourceId = r.Id,
                Name = r.Name.TruncateWithEllipsisSmart(),
                Description = r.Description.TruncateWithEllipsisSmart(),
                CategoryId = r.CategoryId,
                CategoryName = r.Category?.Name ?? "Unknown",
                FileURL = r.FileURL,
                FileId = r.StoredFileId,
                ExternalUrl = r.ExternalUrl,
                Tags = r.TagsCsv,
                CreatedBy = r.UserCreated
            }).ToList();
        }
    }
}