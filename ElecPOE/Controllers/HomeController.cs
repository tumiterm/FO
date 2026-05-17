using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using ForekOnline.Infrastructure.Data;
using ForekOnline.Domain.ViewModels;
using static ForekOnline.Domain.Enums.EnumRegistry;
using Microsoft.EntityFrameworkCore;

namespace ElecPOE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
                              ApplicationDbContext context)
        {
            _logger = logger;

            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminPanel()
        {

            ViewData["ID"] = await _context.StudentAttachments.Where(m => m.DocumentName == eLearnerAdministration.LearnerID).CountAsync();
            ViewData["CV"] = await _context.StudentAttachments.Where(m => m.DocumentName == eLearnerAdministration.CV).CountAsync();
            ViewData["Matric"] = await _context.StudentAttachments.Where(m => m.DocumentName == eLearnerAdministration.Matric).CountAsync();
            ViewData["Guardian"] = await _context.StudentAttachments.Where(m => m.DocumentName == eLearnerAdministration.GuardianId).CountAsync();

            ViewData["Summative"] = await _context.AssessmentAttachments.Where(m => m.Type == eAssessmentAdministration.Summative).CountAsync();
            ViewData["Formative"] = await _context.AssessmentAttachments.Where(m => m.Type == eAssessmentAdministration.Formative).CountAsync();
            ViewData["Practical"] = await _context.AssessmentAttachments.Where(m => m.Type == eAssessmentAdministration.Practical).CountAsync();
            ViewData["IISA"] = await _context.AssessmentAttachments.Where(m => m.Type == eAssessmentAdministration.IISA).CountAsync();


            ViewData["Welder"] = await _context.Results.Where(m => m.Course == eTrade.Welder).CountAsync();
            ViewData["Plumber"] = await _context.Results.Where(m => m.Course == eTrade.Plumber).CountAsync();
            ViewData["Electrician"] = await _context.Results.Where(m => m.Course == eTrade.Electrician).CountAsync();
            ViewData["NATED"] = await _context.Results.Where(m => m.Course == eTrade.NATED).CountAsync();

            ViewData["EnrollmentForm"] = await _context.Training.Where(m => m.Type == eTrainingAdministration.EnrollmentForm).CountAsync();
            ViewData["InductionForm"] = await _context.Training.Where(m => m.Type == eTrainingAdministration.InductionForm).CountAsync();
            ViewData["Declaration"] = await _context.Training.Where(m => m.Type == eTrainingAdministration.Declaration).CountAsync();
            ViewData["Conduct"] = await _context.Training.Where(m => m.Type == eTrainingAdministration.Conduct).CountAsync();

            ViewData["CurrentUser"] = $"{OnGetCurrentUser()?.Name} {OnGetCurrentUser()?.LastName}";

            ViewData["role"] = OnGetCurrentUser()?.Role.ToString();

            return View();
        }
        [Route("/Home/Error")]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult BlockUserAccount()
        {
            return View();
        }

        public IActionResult ForekSystemsHub()
        {
            return View();
        }


        /// <summary>
        /// Retrieves the current user from the session, if available.
        /// </summary>
        /// <returns>The <see cref="User"/> object from the session, or null if not found or if deserialization fails.</returns>
        private User? OnGetCurrentUser()
        {
            string? sessionUserData = HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrEmpty(sessionUserData))
            {
                _logger.LogWarning("Session data for 'SessionUser' is null or empty. User is likely not logged in.");

                return null;
            }

            try
            {
                User? user = DeserializeUser(sessionUserData);

                if (user == null)
                {
                    _logger.LogWarning("Deserialization of 'SessionUser' returned null. Possible data corruption.");
                }

                return user;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize session data for 'SessionUser'. Data may be corrupted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving 'SessionUser' from the session.");
            }

            return null;
        }

        /// <summary>
        /// Deserializes JSON string to a <see cref="User"/> object.
        /// </summary>
        /// <param name="userData">The JSON string representing the user data.</param>
        /// <returns>The deserialized <see cref="User"/> object, or null if deserialization fails.</returns>
        private User? DeserializeUser(string userData)
        {
            if (string.IsNullOrWhiteSpace(userData))
            {
                _logger.LogWarning("Attempted to deserialize empty or whitespace-only user data.");

                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<User>(userData);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize user data due to JSON format issues.");

                return null;
            }
        }


    }
}