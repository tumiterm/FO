using ForekOnline.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class SampleController : Controller
    {
        #region Private DbContext

        /// <summary>
        /// Controller for handling book-related operations.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<SampleController> _logger;

        //private readonly IHelperService _helperService;

        private IWebHostEnvironment _hostEnvironment;

        #endregion
        public IActionResult Index()
        {
            return View();
        }
    }
}
