// <copyright file="VisitController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the VisitController controller


#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion
namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class VisitController : Controller
    {
        private readonly IUnitOfWork _context;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IHelperService _helperService;
        public VisitController(IUnitOfWork context,
                                    IWebHostEnvironment hostEnvironment, IHelperService helperService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public async Task<ActionResult<VisitViewModel>> Visitation(Guid CompanyId, Guid? PlacementId = null)
        {
            if (CompanyId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var company = await _context.Company.GetAsync(filter: c => c.CompanyId == CompanyId);

            if (company == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var contact = await _context.ContactPerson.GetAllAsync();
            var users = await _context.Users.GetAllAsync();
            var students = await StudentList();
            var contactInfo = contact.FirstOrDefault(m => m.AssociativeId == company.CompanyId);
            var regStudents = students.Where(n => !string.IsNullOrEmpty(n.StudentNumber));

            IEnumerable<SelectListItem> studObj = regStudents.Select(m => new SelectListItem
            {
                Value = m.StudentId.ToString(),
                Text = $"{m.FirstName} {m.LastName} ({m.StudentNumber})"
            });


            var filterUsers = users.Where(n => n.Role != eSysRole.Student);

            var nonStudentUsers = from user in users
                                  where user.Role != eSysRole.Student
                                  select new SelectListItem
                                  {
                                      Value = user.Id.ToString(),
                                      Text = $"{user.Name} {user.LastName} ({user.StudentNumber})"
                                  };

            IEnumerable<SelectListItem> userList = nonStudentUsers.ToList();

            if (contactInfo == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            VisitViewModel visitDTO = null;

            visitDTO = new VisitViewModel
            {
                Company = company.CompanyName,

                HasReport = false,

                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),

                LearnerFeedback = "",

                Mentor = $"{contactInfo.Name} {contactInfo.LastName}",

                CompanyId = CompanyId,
                PlacementId = PlacementId,

            };

            ViewBag.StudentId = new SelectList(studObj, "Value", "Text");

            ViewBag.userList = new SelectList(userList, "Value", "Text");

            ViewData["CompId"] = CompanyId;

            return View(visitDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Visitation(VisitViewModel model)
        {

            Visit visit = new()
            {
                VisitId = Helper.GenerateGuid(),
                SelectedEmployeeIDs = string.Join(",", model.SelectedIDArray),
                CreatedOn = _helperService.GetCurrentTime().ToString(),
                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",
                CompanyId = model.CompanyId,
                PlacementId = model.PlacementId,
                Date = ParseDateTime(model.Date),
                DurationMinutes = model.DurationMinutes,
                HasReport = model.HasReport,
                IsActive = true,
                LearnerFeedback = model.LearnerFeedback,
                AttendanceObserved = model.AttendanceObserved,
                EngagementObserved = model.EngagementObserved,
                WorkplaceConditionsObserved = model.WorkplaceConditionsObserved,
                SafetyObserved = model.SafetyObserved,
                SkillApplicationObserved = model.SkillApplicationObserved,
                ObservationNotes = model.ObservationNotes,
                ActionItems = model.ActionItems,
                ActionItemDueDate = model.ActionItemDueDate,
                ActionItemAssignee = model.ActionItemAssignee,
                VisitBy = model.VisitBy,
                Mentor = model.Mentor,
                VisitPurpose = model.VisitPurpose,
                Report = model.Report,
                ReportFile = model.ReportFile,

            };

            Company company = await _context.Company.GetAsync(filter: c => c.CompanyId == visit.CompanyId);

            if (visit != null)
            {
                if (ModelState.IsValid)
                {
                    var visitObj = await _context.Visit.AddAsync(visit);

                    if (visitObj != null)
                    {
                        if (visit.HasReport)
                        {
                            await AttachmentUploader(visit);
                        }

                        int rc = await _context.SaveAsync();

                        if (rc > 0)
                        {
                            TempData["success"] = "Visitation successfully captured";

                            return visit.PlacementId.HasValue
                                ? RedirectToAction("OnPlaceLearner", "LearnerPlacement", new { PlacementId = visit.PlacementId.Value })
                                : RedirectToAction(nameof(VisitationList), new { CompanyId  = visit.CompanyId});
                        }
                        else
                        {
                            TempData["error"] = "Error: Unable to save visitation details!!!";
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error: Something went wrong!!!";
                    }
                }
                else
                {
                   // _notify.Error("Error: Something went wrong!");
                }
            }
               

            return View();
        }
        public async Task<IActionResult> VisitationList(Guid CompanyId)
        {
            var visitList = await _context.Visit.GetAllAsync();

            var visitations = visitList
            .Where(item => item.CompanyId == CompanyId)
            .Select(item => new VisitationViewModel
            {
                    //CompanyId = CompanyId,

                    Date = item.Date,
                    DurationMinutes = item.DurationMinutes,
                    PlacementId = item.PlacementId,
                    Company = OnGetCompany(item.CompanyId).Result.CompanyName,
                    HasReport = item.HasReport,
                    LearnerFeedback = item.LearnerFeedback,
                    AttendanceObserved = item.AttendanceObserved,
                    EngagementObserved = item.EngagementObserved,
                    WorkplaceConditionsObserved = item.WorkplaceConditionsObserved,
                    SafetyObserved = item.SafetyObserved,
                    SkillApplicationObserved = item.SkillApplicationObserved,
                    ObservationNotes = item.ObservationNotes,
                    ActionItems = item.ActionItems,
                    ActionItemDueDate = item.ActionItemDueDate,
                    ActionItemAssignee = item.ActionItemAssignee,
                    Mentor = item.Mentor,
                    Report = item.Report,
                    ReportFile = item.ReportFile,
                    VisitId = item.VisitId,
                    //VisitBy = $"{_userContext.OnLoadItemAsync(item.VisitBy).Result.Name} {_userContext.OnLoadItemAsync(item.VisitBy).Result.LastName}",
                    VisitPurpose = item.VisitPurpose
                })
                .ToList();

            ViewData["result"] = visitations.FirstOrDefault()?.Company;  
            ViewData["CompanyId"] = CompanyId;   

            return View(visitations);
        }

        public async Task<IActionResult> OnModifyVisitation(Guid VisitId)
        {
            var visitation = await _context.Visit.GetAsync(filter: v => v.VisitId == VisitId);

            VisitViewModel visit = new()
            {
                VisitId = VisitId,
                Company = OnGetCompany(visitation.CompanyId).Result.CompanyName,
                PlacementId = visitation.PlacementId,
                Date = visitation.Date.ToString("dddd, dd MMMM yyyy hh:mm tt"),
                DurationMinutes = visitation.DurationMinutes,
                SelectedEmployeeIDs = visitation.SelectedEmployeeIDs,
                CompanyId = visitation.CompanyId, 
                HasReport = visitation.HasReport, 
                LearnerFeedback = visitation.LearnerFeedback,
                AttendanceObserved = visitation.AttendanceObserved,
                EngagementObserved = visitation.EngagementObserved,
                WorkplaceConditionsObserved = visitation.WorkplaceConditionsObserved,
                SafetyObserved = visitation.SafetyObserved,
                SkillApplicationObserved = visitation.SkillApplicationObserved,
                ObservationNotes = visitation.ObservationNotes,
                ActionItems = visitation.ActionItems,
                ActionItemDueDate = visitation.ActionItemDueDate,
                ActionItemAssignee = visitation.ActionItemAssignee,
                Mentor = visitation.Mentor,
                Report = visitation.Report,
                SelectedIDArray= visitation.SelectedIDArray,
                VisitPurpose = visitation.VisitPurpose,

            };

            if(visit == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var students = await StudentList();
            var users = await _context.Users.GetAllAsync();
            visit.SelectedIDArray = visit.SelectedEmployeeIDs.Split(',').ToArray();
            var regStudents = students.Where(n => !string.IsNullOrEmpty(n.StudentNumber));

            IEnumerable<SelectListItem> studObj = regStudents.Select(m => new SelectListItem
            {
                Value = m.StudentId.ToString(),
                Text = $"{m.FirstName} {m.LastName} ({m.StudentNumber})"
            });

            var filterUsers = users.Where(n => n.Role != eSysRole.Student);

            var nonStudentUsers = from user in users

                                  where user.Role != eSysRole.Student

                                  select new SelectListItem
                                  {
                                      Value = user.Id.ToString(),
                                      Text = $"{user.Name} {user.LastName} ({user.StudentNumber})"
                                  };

            IEnumerable<SelectListItem> userList = nonStudentUsers.ToList();

            ViewBag.StudentId = new SelectList(studObj, "Value", "Text");
            ViewBag.userList = new SelectList(userList, "Value", "Text");
            ViewData["CompId"] = visit.CompanyId;

            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyVisitation(VisitViewModel model)
        {
            var visitation = await _context.Visit.GetAsync(filter: v => v.VisitId == model.VisitId);

            Visit visit = new()
            {
                CompanyId = model.CompanyId,
                PlacementId = model.PlacementId,
                Date = ParseDateTime(model.Date),
                DurationMinutes = model.DurationMinutes,
                HasReport = model.HasReport,
                SelectedEmployeeIDs = model.SelectedEmployeeIDs,
                SelectedIDArray = model.SelectedIDArray,
                LearnerFeedback = model.LearnerFeedback,
                AttendanceObserved = model.AttendanceObserved,
                EngagementObserved = model.EngagementObserved,
                WorkplaceConditionsObserved = model.WorkplaceConditionsObserved,
                SafetyObserved = model.SafetyObserved,
                SkillApplicationObserved = model.SkillApplicationObserved,
                ObservationNotes = model.ObservationNotes,
                ActionItems = model.ActionItems,
                ActionItemDueDate = model.ActionItemDueDate,
                ActionItemAssignee = model.ActionItemAssignee,
                Mentor = model.Mentor,
                ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",
                ModifiedOn = Helper.OnGetCurrentDateTime(),
                Report = model.Report,
                ReportFile = model.ReportFile,
                VisitId = model.VisitId,
                VisitBy = model.VisitBy,
                VisitPurpose = model.VisitPurpose,
            };

            if (ModelState.IsValid)
            {
                var modVisitation = await _context.Visit.UpdateVisitAsync(visit);

                if(modVisitation != null)
                {
                    TempData["success"] = "Visitation details saved";
                }
                else
                {
                    TempData["error"] = "Error: Unable to save visit!!!";
                }
            }
            else
            {
                TempData["error"] = "Error: Something went wrong!!!";
            }

            //SelectedEmployeeIDs = string.Join(",", model.SelectedIDArray),

            return View();
        }

        private User OnGetCurrentUser()
        {
            string sessionUserJson = HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrEmpty(sessionUserJson))
            {
                return null;
            }

            try
            {
                User user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                return user;
            }
            catch (JsonException)
            {
                return null;
            }
        }
        public async Task AttachmentUploader(Visit visit)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(visit.ReportFile.FileName);

                string extension = Path.GetExtension(visit.ReportFile.FileName);

                visit.Report = fileName = fileName + Helper.GenerateGuid() + extension;

                string path = Path.Combine(wwwRootPath + "/Visitation/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await visit.ReportFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public static DateTime ParseDateTime(string input)
        {
            DateTime result;

            if (DateTime.TryParse(input, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Failed to parse the input string as DateTime.");
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"Visitation", filename);

                var memory = new MemoryStream();

                using (var stream = new FileStream(folder, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }
        public async Task<Company> OnGetCompany(Guid CompanyId)
        {
            return await _context.Company.GetAsync(filter: c => c.CompanyId == CompanyId);
        }
        public async Task<User> OnGetUser(Guid Id)
        {
            return await _context.Users.GetAsync(filter: u => u.Id == Id);
        }
        private async Task<List<Student>> StudentList()
        {
            return await Helper.GetStudentListAsync();
        }

    }
}
