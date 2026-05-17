using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to generate PDF documents from HTML content using configurable options.
    /// </summary>
    /// <remarks>Implementations of this interface allow conversion of HTML markup to PDF format with
    /// customizable settings such as page size, margins, and rendering options. The interface is not thread-safe;
    /// concurrent operations should use separate instances. Use this interface when you need to programmatically create
    /// PDF files from HTML sources in your application.</remarks>
    public interface IPdfGenerator
    {
        /// <summary>
        /// Converts the specified HTML content to a PDF document using the provided options.
        /// </summary>
        /// <remarks>The PDF generation process may vary depending on the options specified. This method
        /// is not thread-safe; concurrent calls should use separate instances.</remarks>
        /// <param name="html">The HTML markup to be converted to PDF. Cannot be null or empty.</param>
        /// <param name="options">The options that configure PDF generation, such as page size, margins, and rendering settings. Cannot be
        /// null.</param>
        /// <returns>A byte array containing the generated PDF document. The array will be empty if the conversion fails.</returns>
        byte[] FromHtml(string html, PdfDocumentOptions options);
    }
}
