// <copyright file="WorplaceModulesController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the WorplaceModulesController controller

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
using System.Reflection;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]

    public class WorplaceModulesController : Controller
    {
        private readonly IUnitOfWork _context;
        private IHelperService _helperService;

        private IWebHostEnvironment _hostEnvironment;
        public WorplaceModulesController(IUnitOfWork context,
                                    IWebHostEnvironment hostEnvironment, IHelperService helperService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        public async Task<IActionResult> WorkplaceModules(Guid PlacementId)
        {
            var placement = await _context.Placement.GetAllAsync(filter: p => p.PlacementId == PlacementId);

            List<WorkplaceModulesViewModel> moduleList = new List<WorkplaceModulesViewModel>();

            WorkplaceModulesViewModel dto = null;

            var lwm = await _context.WorkplaceModule.GetAllAsync(); 

            var lwmList = lwm.Where(n => n.PlacementId == PlacementId && n.IsActive == true).ToList();

            foreach (var item in lwmList)
            {
                dto = new WorkplaceModulesViewModel
                {
                    LearnerWorkplaceModulesId = item.LearnerWorkplaceModulesId,

                    Days = item.Days,

                    Progress = item.Progress,

                    Student = item.Student,

                    Course = GetItemName(item.CourseId, eOperationType.Course),

                    Module = GetItemName(item.ModuleId, eOperationType.Module),

                    RegisteredBy = item.CreatedBy,

                    IsActive = item.IsActive,

                    PlacementId = item.PlacementId,

                    StartDate = item?.StartDate.Value.ToShortDateString(),

                    EndDate= item?.EndDate.Value.ToShortDateString(),  

                    Company = GetItemName( placement.FirstOrDefault().CompanyId, eOperationType.Company),
                };

                ViewData["Stud"] = item.Student;

                ViewData["Course"] = $"{dto.Course}";

                ViewData["PlacementId"] = PlacementId;

                ViewData["Comp"] = dto.Company;

                moduleList.Add(dto);
            }

            return View(moduleList);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterModule(Guid PlacementId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterModule(LearnerWorkplaceModules modules, Guid PlacementId)
        {
            if(PlacementId == Guid.Empty)
            {
                return NotFound();
            }

            var placement = await _context.Placement.GetAsync(filter: p => p.PlacementId == PlacementId);

            modules.Student = placement.Student;

            modules.IsActive = true;

            modules.LearnerWorkplaceModulesId = Helper.GenerateGuid();

            modules.CreatedOn = _helperService.GetCurrentTime().ToString();

            modules.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            if (ModelState.IsValid)
            {
                var wmObject = await _context.WorkplaceModule.AddAsync(modules);

                if(wmObject != null) 
                {
                    int rc = await _context.SaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = "Module added and saved successfully";

                        return RedirectToAction(nameof(WorkplaceModules), new { PlacementId = modules.PlacementId });
                    }
                    else
                    {
                        TempData["error"] = "Module NOT saved!!!";
                    }

                }
            }

            return View();
        }

        public async Task<IActionResult> OnViewModule(Guid LearnerWorkplaceModulesId, Guid PlacementId)
        {
            if(LearnerWorkplaceModulesId == Guid.Empty)
            {
                return NotFound();
            }

            var lwm = await _context.WorkplaceModule.GetAsync(filter: wm => wm.LearnerWorkplaceModulesId == LearnerWorkplaceModulesId);

            var placement = await _context.Placement.GetAsync(filter: p => p.PlacementId == PlacementId);

            var courses = await _context.Courses.GetAllAsync();

            var modules = await _context.Modules.GetAllAsync();

            WorkplaceModulesViewModel dto = new WorkplaceModulesViewModel
            {
                Course = GetItemName(lwm.CourseId, eOperationType.Course),

                Days = lwm.Days,

                IsActive = lwm.IsActive,

                Module = GetItemName(lwm.ModuleId, eOperationType.Module),

                PlacementId = PlacementId,

                Progress = lwm.Progress,

                RegisteredBy = lwm.CreatedBy,

                Student = lwm.Student,

                ModuleId = lwm.ModuleId,

                CourseId = lwm.CourseId, 

                LearnerWorkplaceModulesId = lwm.LearnerWorkplaceModulesId,

                Company = GetItemName(placement.CompanyId, eOperationType.Company),
            };

            IEnumerable<SelectListItem> courseList = courses.Select(s => new SelectListItem
            {
                Value = s.CourseId.ToString(),

                Text = s.CourseName
            });

            IEnumerable<SelectListItem> moduleList = modules.Select(s => new SelectListItem
            {
                Value = s.ModuleId.ToString(),

                Text = s.ModuleName
            });

            ViewBag.CourseId = new SelectList(courseList, "Value", "Text");

            ViewBag.ModuleId = new SelectList(moduleList, "Value", "Text");

            return View(dto); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnViewModule(WorkplaceModulesViewModel mods)
        {

            LearnerWorkplaceModules lwm = new LearnerWorkplaceModules
            {
                LearnerWorkplaceModulesId = mods.LearnerWorkplaceModulesId,

                ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                CourseId = ConvertStringToGuid(mods.Course),

                ModuleId = ConvertStringToGuid(mods.Module),

                Days = mods.Days,

                IsActive = mods.IsActive,

                ModifiedOn = Helper.OnGetCurrentDateTime(),

                Progress = mods.Progress,

                Student = mods.Student,

                PlacementId = mods.PlacementId,

                StartDate = ParseDateTime(mods.StartDate),

                EndDate = ParseDateTime(mods.EndDate),  

            };

            if (ModelState.IsValid)
            {
                var modObj = await _context.WorkplaceModule.UpdateWorkPlaceModuleAsync(lwm);

                if(modObj != null) 
                {
                    TempData["success"] = "Module changes successfully applied";

                    return RedirectToAction(nameof(WorkplaceModules), new { PlacementId = mods.PlacementId });
                }
                else
                {
                    TempData["error"] = "Error: Module changes NOT saved!!";
                }
            }
            else
            {
                TempData["error"] = "Error: Something went wrong!!";
            }
            return View();
        }
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {

            var mods = await _context.Modules.GetAllAsync();

            var filterMods = from n in mods
                             where n.CourseIdFK == CourseId
                             select n;

            return Json(filterMods.ToList());
        }
        private string GetItemName(Guid itemId, eOperationType itemType)
        {
            string itemName = string.Empty;

            switch (itemType)
            {
                case eOperationType.Course:

                    itemName = $"{_context.Courses.GetAsync(filter: c => c.CourseId == itemId).GetAwaiter().GetResult().CourseName} {_context.Courses.GetAsync(filter: c => c.CourseId == itemId).GetAwaiter().GetResult().Type}";

                    break;

                case eOperationType.Module:

                    itemName =  _context.Modules.GetAsync(filter: m => m.ModuleId == itemId).GetAwaiter().GetResult().ModuleName;

                    break;

                case eOperationType.Company:

                    itemName = _context.Company.GetAsync(filter: c => c.CompanyId == itemId).GetAwaiter().GetResult().CompanyName;

                    break;

                default:
                    break;
            }

            return itemName;
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
        private Guid ConvertStringToGuid(string inputString)
        {
            Guid guid;

            if (Guid.TryParse(inputString, out guid))
            {
                return guid;
            }
            else
            {
                throw new ArgumentException("Invalid GUID format.");
            }
        }
        public static DateTime ParseDateTime(string? input)
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
    }
}
