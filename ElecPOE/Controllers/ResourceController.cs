// <copyright file="ResourceController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.ViewModels;
using ForekOnline.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Numerics;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Provides endpoints for managing resources, including uploading, listing, downloading, previewing, and deleting
    /// resources within the application.
    /// </summary>
    /// <remarks>All actions in this controller require the user to be authorized. The controller coordinates
    /// resource operations by interacting with resource and file upload services, and manages user feedback through
    /// TempData. Actions support both file-based and non-file resources, and handle common error scenarios by logging
    /// and providing user-friendly messages. This controller is intended for use within an ASP.NET Core MVC
    /// application.</remarks>
    [Authorize]
    public class ResourceController : Controller
    {
        #region Private Fields
        private readonly IUnitOfWork _context;
        private readonly ILogger<ResourceController> _logger;
        private readonly IResourceService _resourceService;
        private readonly IFileUploadService _fileUploadService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the ResourceController class with the specified dependencies.
        /// </summary>
        /// <param name="context">The unit of work instance used for data access operations. Cannot be null.</param>
        /// <param name="logger">The logger used for logging information and errors related to the ResourceController. Cannot be null.</param>
        /// <param name="resourceService">The service responsible for resource-related business logic. Cannot be null.</param>
        /// <param name="fileUploadService">The service used to handle file upload operations. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
        public ResourceController(IUnitOfWork context, ILogger<ResourceController> logger, IResourceService resourceService, IFileUploadService fileUploadService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
        }

        /// <summary>
        /// Handles HTTP POST requests to upload a new resource using the provided view model.
        /// </summary>
        /// <remarks>This action requires a valid anti-forgery token and is intended to be called from a
        /// form submission. If the model state is invalid or an error occurs during upload, the user is presented with
        /// the resource list view and any relevant error messages.</remarks>
        /// <param name="resourceUploadViewModel">The view model containing the data for the resource to be uploaded. Must not be null and must satisfy all
        /// model validation requirements.</param>
        /// <returns>A redirect to the resource list view if the upload is successful; otherwise, returns the resource list view
        /// with validation errors and the current model state.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResource(ResourceUploadViewModel resourceUploadViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _resourceService.UploadResourceAsync(resourceUploadViewModel).ConfigureAwait(false);
                    TempData["success"] = "Resource shared successfully.";

                    //await _inAppNotificationService.SendToRoleAsync(eSysRole.Admin, $"{plan.CreatedBy} has submitted his/her Lesson Plan on {DateTimeHelper.GetCurrentSastDateTimeOffset()}",
                    //  actionUrl: Url.Action("OnCreatePlan", "LessonPlanConfig", new { IdPass = plan.IdPass }), iconCss: "fa fa-file-alt", createdBy: $"{plan.CreatedBy}");

                    return RedirectToAction(nameof(ListResources));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading resource.");
                    ModelState.AddModelError(string.Empty, "An error occurred while sharing the resource.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for resource upload.");
            }

            var resources = await _resourceService.LoadResourcesAsync().ConfigureAwait(false);
            var categories = await _context.Category.GetAllAsync().ConfigureAwait(false);

            var categoryItems = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            categoryItems.Insert(0, new SelectListItem { Value = "", Text = "Select a category" });

            var viewModel = new ResourceListViewModel
            {
                Resources = resources,
                Categories = categoryItems
            };

            return View(nameof(ListResources), viewModel);
        }

        /// <summary>
        /// Deletes the specified resource by marking it as inactive.
        /// </summary>
        /// <remarks>This action performs a soft delete by setting the resource's active status to false
        /// rather than removing it from the database. Success or error messages are stored in TempData for display
        /// after the redirect.</remarks>
        /// <param name="id">The unique identifier of the resource to delete.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A redirect to the resource list view if the operation succeeds or fails, or a NotFound result if the
        /// resource does not exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var resource = await _context.Resource.GetAsync(r => r.Id == id, cancellationToken: ct).ConfigureAwait(false);

            if (resource is null)
            {
                return NotFound();
            }

            try
            {
                resource.IsActive = false;
                resource.DateModified = DateTimeOffset.UtcNow;

                await _context.Resource.Update(resource).ConfigureAwait(false);
                await _context.SaveAsync().ConfigureAwait(false);

                TempData["success"] = "Resource deleted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete resource {ResourceId}.", id);
                TempData["error"] = "Failed to delete resource.";
            }

            return RedirectToAction(nameof(ListResources));
        }

        /// <summary>
        /// Handles HTTP GET requests to display a list of resources along with available categories for filtering.
        /// </summary>
        /// <remarks>If an error occurs while loading resources or categories, an error message is stored
        /// in TempData and the user is redirected back to the resource list page. The returned view model includes both
        /// the list of resources and a list of categories for use in the view.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the resource list view with the available resources and
        /// categories, or redirects to the same action with an error message if an error occurs.</returns>
        [HttpGet]
        public async Task<IActionResult> ListResources()
        {
            try
            {
                var resources = await _resourceService.LoadResourcesAsync().ConfigureAwait(false);
                var categories = await _context.Category.GetAllAsync().ConfigureAwait(false);

                var categoryItems = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                categoryItems.Insert(0, new SelectListItem { Value = "", Text = "Select a category" });

                var viewModel = new ResourceListViewModel
                {
                    Resources = resources,
                    Categories = categoryItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading resources.");
                TempData["error"] = "An error occurred while loading resources.";
                return RedirectToAction(nameof(ListResources));
            }
        }

        /// <summary>
        /// Retrieves the file associated with the specified resource and returns it as a downloadable response.
        /// </summary>
        /// <param name="id">The unique identifier of the resource to download. Must correspond to an active resource of type file.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the download operation.</param>
        /// <returns>An <see cref="IActionResult"/> that, on success, contains the file stream for the requested resource. Returns a 404
        /// Not Found result if the resource does not exist or is inactive, or a 400 Bad Request result if the resource is not a
        /// stored file.</returns>
        [HttpGet]
        public async Task<IActionResult> Download(Guid id, CancellationToken ct)
        {
            var resource = await _context.Resource.GetAsync(r => r.Id == id, asNoTracking: true, cancellationToken: ct).ConfigureAwait(false);

            if (resource is null || !resource.IsActive)
            {
                return NotFound();
            }

            if (resource.Type != eResourceType.File || string.IsNullOrWhiteSpace(resource.StoredFileId))
            {
                return BadRequest("This resource is not a stored file.");
            }

            var download = await _fileUploadService.DownloadAsync(resource.StoredFileId, ct).ConfigureAwait(false);

            return File(download.FileStream, download.ContentType ?? "application/pdf", download.FileName);
        }

        /// <summary>
        /// Returns a PDF file for inline preview if the specified resource is an active stored file.
        /// </summary>
        /// <remarks>The response is returned with a Content-Disposition header set to "inline" to enable
        /// browser preview. Only resources of type File with a valid stored file ID are supported.</remarks>
        /// <param name="id">The unique identifier of the resource to preview.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An inline PDF file if the resource is an active stored file; otherwise, a NotFound or BadRequest result.</returns>
        [HttpGet]
        public async Task<IActionResult> Preview(Guid id, CancellationToken ct)
        {
            var resource = await _context.Resource.GetAsync(r => r.Id == id, asNoTracking: true, cancellationToken: ct).ConfigureAwait(false);

            if (resource is null || !resource.IsActive)
            {
                return NotFound();
            }

            if (resource.Type != eResourceType.File || string.IsNullOrWhiteSpace(resource.StoredFileId))
            {
                return BadRequest("This resource is not a stored file.");
            }

            var download = await _fileUploadService.DownloadAsync(resource.StoredFileId, ct).ConfigureAwait(false);

            Response.Headers.ContentDisposition = "inline";
            return File(download.FileStream, "application/pdf");
        }
    }
}