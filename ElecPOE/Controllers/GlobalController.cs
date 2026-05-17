using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class GlobalController : Controller
    {
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
            return View();
        }
    }
}
