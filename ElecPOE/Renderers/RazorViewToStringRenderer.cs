
#region Usings
using System.Text;
using ForekOnline.Application.Common.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endregion

namespace ElecPOE.Renderers
{
    /// <summary>
    /// Provides functionality to render Razor views to their HTML string representation outside of the standard MVC
    /// pipeline.
    /// </summary>
    /// <remarks>This class is typically used in scenarios where Razor views need to be rendered to strings,
    /// such as generating email templates or producing HTML content for APIs. It requires an active HTTP request
    /// context and access to MVC services, including view engines and action context. Thread safety is not guaranteed;
    /// create a new instance per request if used in multi-threaded environments.</remarks>
    public class RazorViewToStringRenderer : IRazorViewToStringRenderer
    {
        #region Fields
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IActionContextAccessor _actionContextAccessor;
        #endregion

        /// <summary>
        /// Initializes a new instance of the RazorViewToStringRenderer class using the specified dependencies required
        /// for rendering Razor views to strings.
        /// </summary>
        /// <remarks>All parameters must be non-null and properly configured to ensure correct view
        /// rendering. This constructor is typically used in dependency injection scenarios to provide the necessary
        /// services for Razor view rendering.</remarks>
        /// <param name="tempDataProvider">The provider used to supply and persist temporary data during view rendering operations.</param>
        /// <param name="serviceProvider">The service provider used to resolve application services required for view rendering.</param>
        /// <param name="viewEngine">The composite view engine responsible for locating and rendering Razor views.</param>
        /// <param name="actionContextAccessor">The accessor used to obtain the current action context for rendering views within the correct HTTP context.</param>
        public RazorViewToStringRenderer(ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ICompositeViewEngine viewEngine, IActionContextAccessor actionContextAccessor)
        {
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _viewEngine = viewEngine;
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Renders the specified MVC view to a string using the provided model.
        /// </summary>
        /// <remarks>This method is typically used to generate HTML from a Razor view outside of the
        /// standard MVC pipeline, such as for email templates or custom responses. The view must exist and be
        /// accessible to the configured view engine. Ensure that IActionContextAccessor is properly registered and that
        /// the method is called within the context of an HTTP request.</remarks>
        /// <typeparam name="TModel">The type of the model to be passed to the view.</typeparam>
        /// <param name="viewPath">The path to the view file to render. Must be a valid view path recognized by the view engine.</param>
        /// <param name="model">The model object to supply to the view. Can be any type compatible with the view's expected model.</param>
        /// <returns>A string containing the rendered HTML output of the view.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no ActionContext is available or if the specified view cannot be found.</exception>
        public async Task<string> RenderViewAsync<TModel>(string viewPath, TModel model)
        {
            var actionContext = _actionContextAccessor.ActionContext
            ?? throw new InvalidOperationException("No ActionContext available. Ensure IActionContextAccessor is registered and used within an HTTP request.");

            await using var output = new StringWriter(new StringBuilder());

            var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);
            if (!viewResult.Success)
            {
                viewResult = _viewEngine.FindView(actionContext, viewPath, isMainPage: true);
            }

            if (!viewResult.Success || viewResult.View is null)
            {
                throw new InvalidOperationException($"View '{viewPath}' was not found.");
            }

            var viewDictionary = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                tempData,
                output,
                new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);

            return output.ToString();

        }
    }
}
