using ElecPOE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers;

[Authorize]
[Route("LearningPractice")]
public sealed class LearningPracticeController : Controller
{
    private readonly ILearningPracticeService _learningPracticeService;

    public LearningPracticeController(ILearningPracticeService learningPracticeService)
    {
        _learningPracticeService = learningPracticeService;
    }

    [HttpGet("questions")]
    public async Task<IActionResult> Questions(
        string subject = "maths",
        string difficulty = "easy",
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(subject, "maths", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(subject, "science", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Choose either maths or science." });
        }

        var assessment = await _learningPracticeService.GetAssessmentAsync(
            subject,
            difficulty,
            count,
            cancellationToken);

        return Json(assessment);
    }
}
