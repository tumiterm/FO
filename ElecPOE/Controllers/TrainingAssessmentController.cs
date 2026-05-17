// <copyright file="TrainingAssessmentController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    02/02/2025 13:22 PM
// Purpose:         Defines the TrainingAssessmentController controller

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;
#endregion

namespace ElecPOE.Controllers
{
    [Authorize]
    public class TrainingAssessmentController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _context;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IFileUploadService _fileUploadService;
        #endregion

        public TrainingAssessmentController(IUnitOfWork context, IStudentService studentService, IUserService userService, IFileUploadService fileUploadService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
        }

        [HttpGet]
        public async Task<IActionResult> TrainingMaterial(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
            {
                TempData["error"] = "Training Material Error:Student number is required.";
            }

            try
            {
                var student = await _studentService.GetStudentAsync(StudentNumber);

                if (student is null)
                {
                    return NotFound();
                }

                if (student.EnrollmentHistory?.Count > 0)
                {
                    ViewData["course"] = student.EnrollmentHistory[0].CourseTitle;
                }

                dynamic trainingObj = new ExpandoObject();

                var trainings = await _context.Training.GetAllAsync();

                var filterTraining = trainings.Where(n => n.StudentNumber == StudentNumber).ToList();

                trainingObj.MaterialList = filterTraining.ToList();

                ViewData["name"] = $"{student.FirstName} {student.LastName}";
                ViewData["StudentNumber"] = StudentNumber;

                return View(trainingObj);
            }
            catch
            {
                TempData["error"] = "An error occurred while retrieving training materials";

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainingMaterial(Training training)
        {
            training.IsActive = true;
            var currentUser = _userService.OnGetCurrentUser();
            training.CreatedBy = $"{currentUser?.Name} {currentUser?.LastName}";
            training.CreatedOn = DateTimeHelper.GetCurrentSastDateTimeOffset().ToString();

            await AttachmentUploadAsync(training);

            var material = await _context.Training.AddAsync(training);

            if (material != null)
            {
                int rc = await _context.SaveAsync();

                if (rc > 0)
                {
                    TempData["success"] = "File saved successfully";
                    return RedirectToAction(nameof(TrainingMaterial), new { StudentNumber = training.StudentNumber });
                }
                else
                {
                    TempData["error"] = "Error: File upload failed";
                }
            }

            return View();
        }

        /// <summary>
        /// Uploads a training document using the file upload service and stores the resulting file ID
        /// in the <see cref="Training.Document"/> property.
        /// </summary>
        /// <param name="training">The training entity whose <see cref="Training.DocumentFile"/> will be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the upload operation.</param>
        /// <returns>A task representing the asynchronous upload operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the file upload fails.</exception>
        private async Task AttachmentUploadAsync(Training training, CancellationToken cancellationToken = default)
        {
            if (training.DocumentFile is null || training.DocumentFile.Length <= 0)
            {
                return;
            }

            await using var stream = training.DocumentFile.OpenReadStream();

            var uploadResponse = await _fileUploadService.UploadAsync(
                new UploadFileRequest(
                    FileStream: stream,
                    FileName: training.DocumentFile.FileName,
                    ContentType: training.DocumentFile.ContentType,
                    Metadata: new Dictionary<string, string>
                    {
                        ["Entity"] = "Training",
                        ["StudentNumber"] = training.StudentNumber ?? string.Empty,
                        ["FileType"] = "TrainingMaterial"
                    },
                    ProviderHint: null,
                    ExpiryDate: null,
                    TenantId: null,
                    DocumentType: "Training.Material"),
                cancellationToken).ConfigureAwait(false);
            training.Document = uploadResponse.FileId;
        }

        /// <summary>
        /// Downloads a training attachment by its file ID using the file upload service.
        /// </summary>
        /// <param name="fileId">The unique file identifier returned during upload.</param>
        /// <returns>A file result for downloading the attachment, or a content message if no file ID is provided.</returns>
        public async Task<IActionResult> AttachmentDownload(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return Content("Sorry NO Attachment found!!!");
            }

            var attachment = await _fileUploadService.DownloadAsync(fileId, HttpContext.RequestAborted);

            return File(attachment.FileStream, attachment.ContentType ?? "application/octet-stream", attachment.FileName);
        }
    }
}
