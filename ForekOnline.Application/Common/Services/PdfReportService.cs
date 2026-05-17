
#region Usings
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality for rendering Razor views to PDF documents using configurable options.
    /// </summary>
    /// <remarks>The PdfReportService enables conversion of Razor views to PDF format, supporting custom
    /// document options and integration with HTTP context for resolving base URLs. This service is typically used in
    /// web applications to generate downloadable or printable PDF reports from dynamic view templates. Thread safety
    /// depends on the underlying implementations of the injected services.</remarks>
    public class PdfReportService : IPdfReportService
    {
        #region Fields
        private readonly IRazorViewToStringRenderer _viewRenderer;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        /// <summary>
        /// Initializes a new instance of the PdfReportService class using the specified view renderer, PDF generator,
        /// and HTTP context accessor.
        /// </summary>
        /// <param name="viewRenderer">The service used to render Razor views to string representations for inclusion in PDF reports. Cannot be
        /// null.</param>
        /// <param name="pdfGenerator">The component responsible for generating PDF documents from rendered content. Cannot be null.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context, enabling user-specific or request-specific data to be included
        /// in reports. Cannot be null.</param>
        public PdfReportService(IRazorViewToStringRenderer viewRenderer, IPdfGenerator pdfGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _viewRenderer = viewRenderer;
            _pdfGenerator = pdfGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Renders the specified view with the provided model and converts the result to a PDF document asynchronously.
        /// </summary>
        /// <remarks>If the <paramref name="options"/> parameter does not specify a base URL, the current
        /// HTTP request's scheme and host are used. This method is thread-safe and can be used in ASP.NET Core
        /// applications to generate PDFs from views.</remarks>
        /// <typeparam name="TModel">The type of the model used to render the view.</typeparam>
        /// <param name="viewPath">The path to the view template to render. Must not be null or empty.</param>
        /// <param name="model">The model data to pass to the view for rendering. Can be any type compatible with the view.</param>
        /// <param name="options">The options used to configure PDF generation, including document settings and base URL. Cannot be null.</param>
        /// <returns>A byte array containing the generated PDF document. The array will be empty if the view renders no content.</returns>
        public async Task<byte[]> RenderViewToPdfAsync<TModel>(string viewPath, TModel model, PdfDocumentOptions options)
        {
            var http = _httpContextAccessor.HttpContext;
            var baseUrl = options.BaseUrl;

            if (http is not null && string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = $"{http.Request.Scheme}://{http.Request.Host}/";
            }

            var html = await _viewRenderer.RenderViewAsync(viewPath, model);

            var mergedOptions = options with { BaseUrl = baseUrl };

            return _pdfGenerator.FromHtml(html, mergedOptions);
        }
    }
}
