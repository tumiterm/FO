using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using ForekOnline.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElecPOE.Controllers
{
    public class GlobalController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<GlobalController> _logger;

        public GlobalController(ApplicationDbContext dbContext, ILogger<GlobalController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult RouteNotFound()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Acknowledgement()
        {
            ViewData["ShowRatingPrompt"] = TempData.Peek("ApplicationId") is string;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplicationRating(
            ApplicationRatingViewModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(value => value.ErrorMessage)
                    .FirstOrDefault() ?? "Please check your feedback and try again.";

                return BadRequest(new { message = error });
            }

            if (TempData.Peek("ApplicationId") is not string applicationIdValue ||
                !Guid.TryParse(applicationIdValue, out var applicationId))
            {
                return BadRequest(new { message = "This feedback request has expired." });
            }

            var applicationExists = await _dbContext.Applications
                .AsNoTracking()
                .AnyAsync(application => application.ApplicationId == applicationId, cancellationToken);

            if (!applicationExists)
            {
                return NotFound(new { message = "The related application could not be found." });
            }

            var ratingExists = await _dbContext.ApplicationRatings
                .AsNoTracking()
                .AnyAsync(rating => rating.ApplicationId == applicationId, cancellationToken);

            if (ratingExists)
            {
                TempData.Remove("ApplicationId");
                return Conflict(new { message = "Feedback has already been submitted for this application." });
            }

            var rating = new ApplicationRating
            {
                ApplicationRatingId = Guid.NewGuid(),
                ApplicationId = applicationId,
                Rating = model.Rating,
                Comment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim(),
                SubmittedOnUtc = DateTime.UtcNow
            };

            try
            {
                _dbContext.ApplicationRatings.Add(rating);
                await _dbContext.SaveChangesAsync(cancellationToken);
                TempData.Remove("ApplicationId");

                return Ok(new { message = "Thank you for helping us improve." });
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(exception, "Unable to save application feedback for {ApplicationId}.", applicationId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "We could not save your feedback right now. Please try again." });
            }
        }
    }
}
