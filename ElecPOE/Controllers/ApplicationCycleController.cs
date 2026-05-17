using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class ApplicationCycleController : Controller
    {
        #region Private Fields
        private readonly IApplicationCycleService _applicationCycleService;
        private readonly ILogger<ApplicationCycleController> _logger;
        #endregion

        #region Constructor
        public ApplicationCycleController(IApplicationCycleService applicationCycleService, ILogger<ApplicationCycleController> logger)
        {
            _applicationCycleService = applicationCycleService ?? throw new ArgumentNullException(nameof(applicationCycleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index(bool includeDeleted = false, bool onlyActive = false, CancellationToken ct = default)
        {
            var cycles = await _applicationCycleService.GetApplicationCyclesAsync(includeDeleted, onlyActive, ct).ConfigureAwait(false);

            ViewData["includeDeleted"] = includeDeleted;
            ViewData["onlyActive"] = onlyActive;

            return View(cycles);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var cycle = await _applicationCycleService.GetApplicationCycleByIdAsync(id, ct).ConfigureAwait(false);
            if (cycle is null)
            {
                return NotFound();
            }

            ViewData["isWindowOpen"] = await _applicationCycleService.IsApplicationWindowOpenAsync(id, ct).ConfigureAwait(false);

            return View(cycle);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var now = DateTimeHelper.GetCurrentSastDateTimeOffset();

            var model = new ApplicationCycle
            {
                Id = Guid.NewGuid(),
                AcademicYear = now.Year,
                OpensAt = now.Date,
                Closes = now.Date.AddMonths(1),
                ApplicationPeriod = "Intake",
                TurnaroundDays = 0,
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCycle model, CancellationToken ct = default)
        {

            try
            {
                model.UserCreated = User?.Identity?.Name ?? "System";
                model.UserModified = model.UserCreated;
                model.DateCreated = model.DateCreated == default ? DateTimeOffset.UtcNow : model.DateCreated;
                model.DateModified = DateTimeOffset.UtcNow;

                var result = await _applicationCycleService.CreateApplicationCycleAsync(model, ct).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(result?.Message))
                {
                    TempData["error"] = result.Message;
                    return View(model);
                }

                TempData["success"] = "Application cycle created.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating application cycle.");
                TempData["error"] = "Unexpected error occurred while creating the application cycle.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var cycle = await _applicationCycleService.GetApplicationCycleByIdAsync(id, ct).ConfigureAwait(false);
            if (cycle is null)
            {
                return NotFound();
            }

            return View(cycle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationCycle model, CancellationToken ct = default)
        {
            if (model is null)
            {
                return BadRequest();
            }

            if (model.Id == Guid.Empty)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Validation error. Please review the form.";
                return View(model);
            }

            try
            {
                model.UserModified = User?.Identity?.Name ?? "System";
                model.DateModified = DateTimeOffset.UtcNow;

                var result = await _applicationCycleService.UpdateApplicationCycleAsync(model, ct).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(result?.Message))
                {
                    if (result.Message.Contains("updated", StringComparison.OrdinalIgnoreCase) ||
                        result.Message.Contains("saved", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["success"] = result.Message;
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["error"] = result.Message;
                    return View(model);
                }

                TempData["success"] = "Application cycle updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application cycle {CycleId}", model.Id);
                TempData["error"] = "Unexpected error occurred while updating the application cycle.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigureTurnaroundTime(Guid cycleId, int days, CancellationToken ct = default)
        {
            if (cycleId == Guid.Empty)
            {
                return BadRequest();
            }

            var result = await _applicationCycleService.ConfigureTurnaroundTimeAsync(cycleId, days, ct).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(result?.Message))
            {
                if (result.Message.Contains("updated", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["success"] = result.Message;
                }
                else
                {
                    TempData["error"] = result.Message;
                }
            }
            else
            {
                TempData["success"] = "Turnaround time updated.";
            }

            return RedirectToAction(nameof(Details), new { id = cycleId });
        }
    }
}
