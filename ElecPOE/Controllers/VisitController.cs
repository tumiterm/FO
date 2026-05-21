#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class VisitController : Controller
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
        }

        public IActionResult VisitHome() => View();

        [HttpGet]
        public async Task<IActionResult> StudentLookup(string? term, CancellationToken cancellationToken)
        {
            var result = await _visitService.SearchStudentsAsync(term, 25, cancellationToken);
            return Json(result);
        }

        public async Task<ActionResult<VisitViewModel>> Visitation(Guid CompanyId, Guid? PlacementId = null, CancellationToken cancellationToken = default)
        {
            if (CompanyId == Guid.Empty) return RedirectToAction("RouteNotFound", "Global");
            var model = await _visitService.GetCreateViewModelAsync(CompanyId, PlacementId, cancellationToken);
            if (model == null) return RedirectToAction("RouteNotFound", "Global");
            await PopulateLookups(cancellationToken);
            ViewData["CompId"] = CompanyId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Visitation(VisitViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateLookups(cancellationToken);
                return View(model);
            }

            var result = await _visitService.CreateAsync(model, GetCurrentUser(), cancellationToken);
            TempData[result.Success ? "success" : "error"] = result.Message;
            if (!result.Success || result.Visit == null)
            {
                await PopulateLookups(cancellationToken);
                return View(model);
            }

            return result.Visit.PlacementId.HasValue
                ? RedirectToAction("OnPlaceLearner", "LearnerPlacement", new { PlacementId = result.Visit.PlacementId.Value })
                : RedirectToAction(nameof(VisitationList), new { CompanyId = result.Visit.CompanyId });
        }

        public async Task<IActionResult> VisitationList(Guid CompanyId, CancellationToken cancellationToken)
        {
            var visits = await _visitService.GetVisitationListAsync(CompanyId, cancellationToken);
            ViewData["result"] = visits.FirstOrDefault()?.Company;
            ViewData["CompanyId"] = CompanyId;
            return View(visits);
        }

        public async Task<IActionResult> OnModifyVisitation(Guid VisitId, CancellationToken cancellationToken)
        {
            var visit = await _visitService.GetForEditAsync(VisitId, cancellationToken);
            if (visit == null) return RedirectToAction("RouteNotFound", "Global");
            await PopulateLookups(cancellationToken);
            ViewData["CompId"] = visit.CompanyId;
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyVisitation(VisitViewModel model, CancellationToken cancellationToken)
        {
            var result = await _visitService.UpdateAsync(model, GetCurrentUser(), cancellationToken);
            TempData[result.Success ? "success" : "error"] = result.Message;
            return RedirectToAction(nameof(OnModifyVisitation), new { VisitId = model.VisitId });
        }

        public async Task<IActionResult> AttachmentDownload(string filename, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filename)) return Content("Sorry, no attachment found!!!");
            var response = await _visitService.DownloadAttachmentAsync(filename, cancellationToken);
            if (response == null) return Content("Sorry, no attachment found!!!");
            return File(response.FileStream, response.ContentType ?? "application/octet-stream", response.FileName);
        }

        private User? GetCurrentUser()
        {
            var raw = HttpContext.Session.GetString("SessionUser");
            return string.IsNullOrWhiteSpace(raw) ? null : JsonConvert.DeserializeObject<User>(raw);
        }

        private async Task PopulateLookups(CancellationToken cancellationToken)
        {
            var (students, visitors) = await _visitService.GetLookupOptionsAsync(cancellationToken);
            ViewBag.StudentId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(students, "Value", "Text");
            ViewBag.userList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(visitors, "Value", "Text");
        }
    }
}
