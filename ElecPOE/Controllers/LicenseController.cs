// <copyright file="LicenseController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the LicenseController controller

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.Design;
#endregion

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LicenseController : Controller
    {
        #region Usings
        private readonly IUnitOfWork _context;
        private readonly ILogger<LicenseController> _logger;
        private readonly IFileUploadService _fileUploadService;

        private IWebHostEnvironment _hostEnvironment;
        #endregion
        public LicenseController(IUnitOfWork context, IWebHostEnvironment hostEnvironment,
                                ILogger<LicenseController> logger, IFileUploadService fileUploadService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _logger = logger;
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
        }

        [HttpGet]
        public async Task<IActionResult> AddUserLicense()
        {
            var companyList = await _context.Company.GetAllAsync();

            var courseList = await _context.Courses.GetAllAsync();

            IEnumerable<SelectListItem> courses = courseList.Select(l => new SelectListItem
            {
                Value = l.CourseId.ToString(),
                Text = $"({l.CourseName}) - ({l.Type})",

            });

            IEnumerable<SelectListItem> companies = companyList
                                       .OrderBy(l => l.CompanyName)
                                       .Select(l => new SelectListItem
                                       {
                                           Value = l.CompanyId.ToString(),

                                           Text = $"{l.CompanyName}",

                                       });

            ViewBag.CompanyId = new SelectList(companies, "Value", "Text");

            ViewBag.CourseId = new SelectList(courses, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserLicense(LicenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            License license = new()
            {
                DateOfExpiry = model.DateOfExpiry,
                ClientType = model.ClientType,
                Title = model.Title,
                CourseKey = model.CourseKey,
                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",
                DateOfIssue = model.DateOfIssue,
                CreatedOn = Helper.OnGetCurrentDateTime(),
                FileUpload = model.FileUpload,
                IDNumber = model.IDNumber,
                Frequency = model.Frequency,
                IsActive = true,
                LastName = model.LastName,
                LicenseId = Helper.GenerateGuid(),
                Name = model.Name,
                CompanyId = model.CompanyId,
                IFormFileUpload = model.IFormFileUpload,

            };

            if (license.IFormFileUpload is not null && license.IFormFileUpload.Length > 0)
            {
                await using var stream = license.IFormFileUpload.OpenReadStream();

                var uploadResponse = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        FileStream: stream,
                        FileName: license.IFormFileUpload.FileName,
                        ContentType: license.IFormFileUpload.ContentType,
                        Metadata: new Dictionary<string, string>
                        {
                            ["Entity"] = "License",
                            ["LicenseId"] = license.LicenseId.ToString("D")
                        },
                        ProviderHint: null,
                        ExpiryDate: null,
                        TenantId: null,
                        DocumentType: "License"
                    ),
                    HttpContext.RequestAborted);

                license.FileUpload = uploadResponse.FileId;
            }

            var licenseObj = await _context.License.AddAsync(license);
            int rc = licenseObj != null ? await _context.SaveAsync() : 0;

            if (rc > 0)
            {
                TempData["success"] = "License saved successfully";
                return RedirectToAction(nameof(AddUserLicense));
            }

            TempData["error"] = "License NOT saved!!!";
            return View(license);
        }

        public async Task<IActionResult> Licenses()
        {
            var licenses = await _context.License.GetAllAsync();

            var licenseList = new List<LicenseViewModel>();

            foreach (var lic in licenses)
            {
                var companyName = await OnGetCompanyNameAsync(lic.CompanyId);

                var courseName = await OnGetCourseNameAsync(lic.CourseKey);

                licenseList.Add(new LicenseViewModel
                {
                    DateOfExpiry = lic.DateOfExpiry,

                    DateOfIssue = lic.DateOfIssue,

                    IDNumber = Helper.MaskInput(lic.IDNumber),

                    ClientType = lic.ClientType,

                    Company = companyName,

                    LicenseName = courseName,

                    CompanyId = lic.CompanyId,

                    CourseKey = lic.CourseKey,

                    FileUpload = lic.FileUpload,

                    Frequency = lic.Frequency,

                    LastName = lic.LastName,

                    LicenseId = lic.LicenseId,

                    Name = lic.Name,

                    Title = lic.Title,

                });
            }

            return View(licenseList);
        }

        [HttpGet]
        public async Task<IActionResult> LicenseInfo(Guid licenseId)
        {
            if (licenseId == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var license = await _context.License.GetAsync(filter: lic => lic.LicenseId == licenseId);

                if (license == null)
                {
                    return NotFound();
                }

                var viewModel = new LicenseViewModel
                {
                    LastName = license.LastName,

                    Name = license.Name,

                    ClientType = license.ClientType,

                    Company = await OnGetCompanyNameAsync(license.CompanyId),

                    CompanyId = license.CompanyId,

                    CourseKey = license.CourseKey,

                    DateOfExpiry = license.DateOfExpiry,

                    DateOfIssue = license.DateOfIssue,

                    LicenseId = license.LicenseId,

                    FileUpload = license.FileUpload,

                    Frequency = license.Frequency,

                    IDNumber = license.IDNumber,

                    LicenseName = await OnGetCourseNameAsync(license.CourseKey),

                    Title = license.Title
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching license information: {ex.Message}");

                return StatusCode(500, "Internal Server Error");
            }
        }

        public async Task<IActionResult> LicenseDownload(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return Content("Sorry, no attachment found!!!");
            }

            var download = await _fileUploadService.DownloadAsync(fileId, HttpContext.RequestAborted);

            return File(download.FileStream, download.ContentType ?? "application/octet-stream", download.FileName);
        }

        private User? OnGetCurrentUser()
        {
            try
            {
                string? sessionUserJson = HttpContext.Session.GetString("SessionUser");

                if (string.IsNullOrEmpty(sessionUserJson))
                {
                    return null;
                }

                User? user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                return user;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async Task<string> OnGetCompanyNameAsync(Guid companyId)
        {
            try
            {
                var company = await _context.Company.GetAsync(filter: c => c.CompanyId == companyId);

                return !string.IsNullOrEmpty(company?.CompanyName) ? company.CompanyName : "Company Loading...";

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                return "Error retrieving company";
            }
        }

        public async Task<string> OnGetCourseNameAsync(Guid courseId)
        {
            try
            {
                var course = await _context.Courses.GetAsync(filter: c => c.CourseId == courseId);

                return course != null ? $"{course.CourseName} - ({course.Type.ToString()})" : "Course not found";

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                return "Error retrieving course";
            }

        }

        public async Task<bool> AttachmentUploader(License license)
        {
            if (license.IFormFileUpload != null && license.IFormFileUpload.Length > 0)
            {
                try
                {

                    string wwwRootPath = _hostEnvironment.WebRootPath;

                    string fileName = Path.GetFileNameWithoutExtension(license.IFormFileUpload.FileName);

                    string extension = Path.GetExtension(license.IFormFileUpload.FileName);

                    string path = Path.Combine(wwwRootPath + "/Licenses/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await license.IFormFileUpload.CopyToAsync(fileStream);
                    }

                    license.FileUpload = fileName = fileName + Helper.GenerateGuid() + extension;

                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }

            }

            return false;
        }


    }
}
