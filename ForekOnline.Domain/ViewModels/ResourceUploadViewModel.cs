// <copyright file="ResourceUploadViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the data required for uploading (sharing) a resource.
    /// </summary>
    public record ResourceUploadViewModel : IValidatableObject
    {
        private const long MaxUploadBytes = 3 * 1024 * 1024;

        public Guid ResourceId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Resource Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        public string FileURL { get; set; } = string.Empty;

        public string? FileId { get; set; }

        [Display(Name = "Link (Optional)")]
        [Url]
        public string? ExternalUrl { get; set; }

        [Display(Name = "Tags")]
        [StringLength(500)]
        public string? Tags { get; set; }

        [ValidateNever]
        [Display(Name = "File")]
        public IFormFile? File { get; set; }

        [ValidateNever]
        public List<SelectListItem> Categories { get; set; } = new();

        [Display(Name = "Everyone")]
        public bool IsPublic { get; set; } = true;

        [ValidateNever]
        public List<eSysRole> SelectedRoles { get; set; } = new();

        [ValidateNever]
        public List<Guid> SelectedUserIds { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasFile = File is not null && File.Length > 0;
            var hasLink = !string.IsNullOrWhiteSpace(ExternalUrl);

            if (!hasFile && !hasLink)
            {
                yield return new ValidationResult("Please upload a file or provide a link.", new[] { nameof(File), nameof(ExternalUrl) });
            }

            if (hasFile && hasLink)
            {
                yield return new ValidationResult("Please provide either a file or a link (not both).", new[] { nameof(File), nameof(ExternalUrl) });
            }

            if (!IsPublic && (SelectedRoles.Count == 0) && (SelectedUserIds.Count == 0))
            {
                yield return new ValidationResult("Select at least one audience role/user, or choose Everyone.", new[] { nameof(IsPublic), nameof(SelectedRoles), nameof(SelectedUserIds) });
            }

            if (hasFile)
            {
                if (File!.Length > MaxUploadBytes)
                {
                    yield return new ValidationResult("File must be 3 MB or less.", new[] { nameof(File) });
                }

                var isPdf = File.ContentType?.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) == true
                    || File.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

                if (!isPdf)
                {
                    yield return new ValidationResult("Only PDF files are allowed.", new[] { nameof(File) });
                }
            }
        }
    }
}