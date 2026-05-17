// <copyright file="EvidenceController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the EvidenceController class


#region Usings
using ElecPOE.Common;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mail;
using System.Threading.Tasks;
#endregion

namespace ElecPOE.Controllers
{
    [Authorize]
    public class EvidenceController : Controller
    {
        #region Private Fields
        private readonly IUnitOfWork _context;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IHelperService _helperService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the EvidenceController class.
        /// </summary>
        /// <param name="context">The unit of work interface for database operations</param>
        /// <param name="hostEnvironment">The web hosting environment interface</param>
        public EvidenceController(IUnitOfWork context, IWebHostEnvironment hostEnvironment, IHelperService helperService, IFileUploadService fileUploadService, IStudentService studentService, IUserService userService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _helperService = helperService;
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Displays a specific student file based on evidence ID and student number.
        /// </summary>
        /// <param name="EvidenceId">The unique identifier of the evidence</param>
        /// <param name="StudentNumber">The student number</param>
        /// <returns>View with evidence details or NotFound if evidence not found</returns>
        public async Task<IActionResult> ViewStudentFile(Guid EvidenceId,string StudentNumber)
        {
            if(EvidenceId == Guid.Empty)
            {
                return NotFound();
            }

            var evidence = await _context.Evidence.GetAsync(filter: e => e.EvidenceId == EvidenceId);

            var student = await GetStudent(StudentNumber);

            ViewData["StudInfo"] = $"{student.FirstName} {student.LastName}";

            ViewData["By"] = evidence.CreatedBy;

            ViewData["On"] = evidence.CreatedOn;

            ViewData["Active"] = evidence.IsActive;

            ViewData["Module"] = evidence.Module;

            return View(evidence);
        }
        /// <summary>
        /// Displays a gallery of evidence for a specific student.
        /// </summary>
        /// <param name="StudentNumber">The student number to filter evidence</param>
        /// <returns>View with student's evidence gallery</returns>

        [HttpGet]
        public async Task<IActionResult> StudentGallery(string StudentNumber)
        {

            var students = await _studentService.GetStudentListAsync();

            var galleryList = await _context.Evidence.GetAllAsync();

            var filterGallery = from n in galleryList

                                where n.StudentNumber == StudentNumber

                                select n;

            var finalList = filterGallery.ToList();

            Student student = await GetStudent(StudentNumber);

            ViewData["StudData"] = $"{student.FirstName} {student.LastName}";

            return View(finalList);
        }

        /// <summary>
        /// Displays the upload evidence page for a specific student.
        /// </summary>
        /// <param name="StudentNumber">The student number</param>
        /// <returns>View for uploading evidence or NotFound if student not found</returns>
        [HttpGet]
        public async Task<IActionResult> UploadEvidence(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
            {
                return BadRequest("Student number is required");
            }

            Student student = await GetStudent(StudentNumber);

            if (student is null)
            {
                return NotFound();
            }

            ViewData["name"] = $"{student.FirstName} {student.LastName}";

            ViewData["StudentId"] = $"{student.StudentId}";

            ViewData["StudentNumber"] = $"{student.StudentNumber}";

            return View(nameof(UploadEvidence));
        }

        [HttpGet("/Evidence/Image")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> EvidenceImage([FromQuery] string fileId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return NotFound();
            }

            var download = await _fileUploadService.DownloadAsync(fileId, ct).ConfigureAwait(false);
            return File(download.FileStream, download.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Handles the evidence upload process.
        /// </summary>
        /// <param name="evidence">The evidence object to upload</param>
        /// <param name="StudentNumber">The student number</param>
        /// <returns>Redirects to upload page with success/error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadEvidence(Evidence evidence, string StudentNumber)
        {
            evidence.IsActive = true;

            evidence.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            evidence.CreatedOn = _helperService.GetCurrentTime().ToString();

            await AttachmentUploader(evidence);

            Evidence uploads = await _context.Evidence.AddAsync(evidence);

            if(uploads != null)
            {
                if (uploads != null)
                {
                    int rc = await _context.SaveAsync();
                    TempData["success"] = rc > 0 ? "File Uploaded Successfully" : null;
                    TempData["error"] = rc <= 0 ? "Error: Unable to upload item" : null;
                }

            }

            return RedirectToAction(nameof(UploadEvidence), new { StudentNumber = evidence.StudentNumber});
        }
        public async Task<Student> GetStudent(string StudentNumber)
        {
           return await _studentService.GetStudentAsync(StudentNumber);
        }

        /// <summary>
        /// Uploads the attachment file to the server.
        /// </summary>
        /// <param name="evidence">The evidence containing the file to upload</param>
        private async Task<string> AttachmentUploader(Evidence evidence)
        {
            var fileId = await _fileUploadService.UploadIfPresentAsync(
                file: evidence.PhotoFile,
                documentType: "Evidence",
                metadata: new Dictionary<string, string>
                {
                    ["Entity"] = "Evidence",
                    ["AttachmentId"] = evidence.EvidenceId.ToString("D"),
                    ["StudentNumber"] = evidence.StudentNumber ?? string.Empty,
                    ["StudentId"] = evidence.StudentId.ToString() ?? string.Empty,
                },
                ct: HttpContext.RequestAborted).ConfigureAwait(false);

            evidence.Photo = fileId;

            return fileId ?? string.Empty;
        }

        /// <summary>
        /// Downloads an attachment file.
        /// </summary>
        /// <param name="filename">The name of the file to download</param>
        /// <returns>File stream or error content</returns>
        public async Task<IActionResult> AttachmentDownload(string fileId, CancellationToken ct = default)
        {
            var download = await _fileUploadService.DownloadIfPresentAsync(fileId, ct);
            if (download is null) return NotFound();
            return File(download.Value.FileStream, download.Value.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Gets the current user from session.
        /// </summary>
        /// <returns>User object or null if not found</returns>
        private User OnGetCurrentUser()
        {
            return _userService.OnGetCurrentUser() ?? new User();
        }

        /// <summary>
        /// Displays all evidence files for a student.
        /// </summary>
        /// <param name="StudentNumber">The student number to filter evidence</param>
        /// <returns>View with list of evidence</returns>
        [HttpGet]
        public async Task<IActionResult> OnViewAllFiles(string StudentNumber)
        {
            var studentEvidence = await _context.Evidence.GetAllAsync();

            EvidenceViewModel evidence = null;

            List<EvidenceViewModel> evidenceList = new List<EvidenceViewModel>();

            var filteredEvidence = from n in studentEvidence

                                   where n.StudentNumber == StudentNumber

                                   select n;

            var list = filteredEvidence.ToList();

            foreach (var item in list)
            {
                evidence = new EvidenceViewModel
                {
                    Module = item.Module,

                    Photo = item.Photo,

                    StudentId = item.StudentId,

                    StudentNumber = item.StudentNumber,
                };

                evidenceList.Add(evidence);

            }


            ViewData["Number"] = evidenceList.Count;

            Student student = await GetStudent(StudentNumber);

            ViewData["HolderNumber"] = student.StudentNumber;

            ViewData["EvidenceHolder"] = $"{student.FirstName} {student.LastName} ({student.StudentNumber})";

            return View(evidenceList);
        }
    }
}
