using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class OnlineApplicationsAuthController : Controller
    {
        private readonly ILogger<OnlineApplicationsAuthController> _logger;
        private readonly IApplicantUserService _applicantUserService;
        private readonly IUnitOfWork _unitOfWork;
        public OnlineApplicationsAuthController(ILogger<OnlineApplicationsAuthController> logger, IApplicantUserService applicantUserService, IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicantUserService = applicantUserService ?? throw new ArgumentNullException(nameof(applicantUserService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterRequest user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _applicantUserService.RegisterAsync(user);

            if (!result.IsError)
            {
                TempData["success"] = "Registration successful. Please sign in.";
                return RedirectToAction("Home", "OnlineApplications");
            }

            ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(result.Message)
                ? "Registration failed. Please try again."
                : result.Message);

            TempData["error"] = result.Message;
            return View();
        }

        public IActionResult ApplicantProfile() 
        {
            return View();
        }
    }
}
