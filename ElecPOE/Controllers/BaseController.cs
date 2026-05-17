// <copyright file="BaseController.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/01/2025 10:13:27 PM
// Purpose:         Defines the BaseController class

#region Usings
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
#endregion

namespace ElecPOE.Controllers
{
    /// <summary>
    /// Base controller to handle common functionality across controllers.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Called after an action method is executed.
        /// Checks if an error exists in the HttpContext and sets it in TempData for view consumption.
        /// </summary>
        /// <param name="context">The action execution context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Items.ContainsKey("error"))
            {
                TempData["error"] = context.HttpContext.Items["error"];
            }
            base.OnActionExecuted(context);
        }
    }
}
