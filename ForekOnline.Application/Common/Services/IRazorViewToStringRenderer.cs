
namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to render a Razor view to a string using the specified model.
    /// </summary>
    /// <remarks>This interface is typically used to generate HTML output from Razor views outside of the
    /// standard MVC pipeline, such as for email templates or background processing. Implementations should ensure that
    /// the view is rendered with the provided model and that any required view data or context is properly
    /// handled.</remarks>
    public interface IRazorViewToStringRenderer
    {
        /// <summary>
        /// Renders the specified view using the provided model and returns the rendered HTML as an asynchronous
        /// operation.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to be passed to the view for rendering.</typeparam>
        /// <param name="viewPath">The path to the view file to render. Must be a valid, non-empty string.</param>
        /// <param name="model">The model object to supply to the view. Can be any type compatible with the view's expected model.</param>
        /// <returns>A task that represents the asynchronous rendering operation. The task result contains the rendered HTML as a
        /// string.</returns>
        Task<string> RenderViewAsync<TModel>(string viewPath, TModel model);
    }
}
