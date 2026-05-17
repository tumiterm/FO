using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to render a Razor view to a PDF document asynchronously using the specified model and PDF
    /// options.
    /// </summary>
    /// <remarks>This interface is intended for services that generate PDF reports from Razor views.
    /// Implementations should ensure that the PDF output accurately reflects the rendered view and respects the
    /// provided document options. Thread safety and performance characteristics depend on the specific
    /// implementation.</remarks>
    public interface IPdfReportService
    {
        /// <summary>
        /// Renders the specified view using the provided model and converts the result to a PDF document
        /// asynchronously.
        /// </summary>
        /// <remarks>The rendered PDF will reflect the output of the view as populated by the specified
        /// model. The operation is performed asynchronously and does not block the calling thread. Ensure that the view
        /// path and model are valid to avoid errors during rendering.</remarks>
        /// <typeparam name="TModel">The type of the model used to populate the view.</typeparam>
        /// <param name="viewPath">The path to the view file to render. Must be a valid view location.</param>
        /// <param name="model">The model object used to supply data to the view.</param>
        /// <param name="options">The options used to configure PDF generation, such as page size, margins, and output settings.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a byte array with the generated
        /// PDF content.</returns>
        Task<byte[]> RenderViewToPdfAsync<TModel>(string viewPath, TModel model, PdfDocumentOptions options);
    }
}
