// <copyright file="OnlineApplicationsController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    30/01/2026 01:26 AM
// Purpose:         Defines the OnlineApplicationsController class

#region Usings
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Provides endpoints for displaying the Home page and retrieving online course information for applications.
    /// </summary>
    /// <remarks>This controller exposes actions for rendering the main application view and for searching
    /// available courses with pagination. All course data returned by this controller is limited to summary
    /// information; use other endpoints to access detailed course data if required.</remarks>
    public class OnlineApplicationsController : Controller
    {
        #region Fields
        private readonly ICourseService _courseService;
        private readonly IOnlineApplicationsService _applicationsService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the OnlineApplicationsController class with the specified course service.
        /// </summary>
        /// <param name="courseService">The service used to manage and retrieve course information. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if courseService is null.</exception>
        public OnlineApplicationsController(ICourseService courseService, IOnlineApplicationsService applicationsService)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _applicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
        }

        /// <summary>
        /// Returns the default view for the Home page.
        /// </summary>
        /// <returns>An <see cref="ViewResult"/> that renders the Home page view.</returns>
        public IActionResult Home()
        {
            return View();
        }

        /// <summary>
        /// Retrieves a paginated list of courses that match the specified search criteria.
        /// </summary>
        /// <remarks>The returned course items include only summary fields such as CourseId, Name, Type,
        /// NqfLevel, Credit, and ModuleCount. Use additional endpoints to retrieve detailed course information if
        /// needed.</remarks>
        /// <param name="q">An optional search query used to filter courses by name or other relevant fields. If null or empty, all
        /// courses are returned.</param>
        /// <param name="page">The page number of the results to retrieve. Must be greater than or equal to 1. Defaults to 1.</param>
        /// <param name="pageSize">The maximum number of courses to include in a single page of results. Must be greater than 0. Defaults to
        /// 30.</param>
        /// <returns>An IActionResult containing a paginated list of courses that match the search criteria. The result includes
        /// the current page, page size, total count, and a collection of course items with summary information.</returns>
        [Route("api/courses")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            var result = await _courseService.SearchCoursesAsync(
                q: q,
                type: null,
                category: null,
                isFunded: null,
                nqfMin: null,
                nqfMax: null,
                page: page,
                pageSize: pageSize);

            return Ok(new
            {
                result.Page,
                result.PageSize,
                result.TotalCount,
                Items = result.Items.Select(x => new
                {
                    x.CourseId,
                    Name = x.Name,
                    Type = x.Type.ToString(),
                    NqfLevel = x.NQFLevel?.ToString(),
                    x.Credit,
                    x.ModuleCount
                })
            });
        }

        public IActionResult StudentProfile()
        {
            return View();
        }

        /// <summary>
        /// Displays a view containing all application cycles, including inactive and non-deleted cycles.
        /// </summary>
        /// <remarks>The returned view includes both active and inactive application cycles, but excludes
        /// cycles that have been deleted. This method performs an asynchronous operation and supports cancellation via
        /// the provided token.</remarks>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the view with the list of application cycles.</returns>
        public async Task<IActionResult> ApplicationCycles(CancellationToken ct)
        {
            var cycles = await _applicationsService.GetApplicationCyclesAsync(includeDeleted: false, onlyActive: false, ct: ct);
            return View(cycles);
        }

        /// <summary>
        /// Returns the view for configuring the application cycle.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the configuration view for the application cycle.</returns>
        public IActionResult ConfigureApplicationCycle()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation and configuration of a new application cycle.
        /// </summary>
        /// <remarks>This action requires a valid anti-forgery token and is intended to be called via HTTP
        /// POST. Validation errors and operation results are communicated to the user through model state and temporary
        /// data.</remarks>
        /// <param name="cycle">The application cycle data to be created and configured. Cannot be null.</param>
        /// <returns>A view displaying validation errors if the input is invalid or the creation fails; otherwise, a redirect to
        /// the application cycles list upon successful creation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigureApplicationCycle(ApplicationCycle cycle)
        {
            if (cycle == null)
            {
                var errorMessage = "Application cycle data is required.";
                ModelState.AddModelError(string.Empty, errorMessage);
                TempData["error"] = errorMessage;
                return View(cycle);
            }

            var response = await _applicationsService.CreateApplicationCycleAsync(cycle);

            if (response.IsError)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                TempData["error"] = response.ErrorDescription;
                return View(cycle);
            }

            TempData["success"] = string.IsNullOrWhiteSpace(response.Message) ? "Application cycle saved." : response.Message;

            return RedirectToAction(nameof(ApplicationCycles));
        }
    }
}

