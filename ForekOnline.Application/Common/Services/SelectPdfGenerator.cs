
#region Usings
using ForekOnline.Domain.ViewModels;
using SelectPdf;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to generate PDF documents from HTML content using the SelectPdf library.
    /// </summary>
    /// <remarks>This class is a sealed implementation of the IPdfGenerator interface and is intended for
    /// scenarios where HTML needs to be converted to PDF with customizable document options, such as page size,
    /// orientation, margins, and optional headers and footers. Instances of this class are not thread-safe; create a
    /// new instance for each conversion if used in multi-threaded environments.</remarks>
    public sealed class SelectPdfGenerator : IPdfGenerator
    {
        /// <summary>
        /// Converts the specified HTML string to a PDF document using the provided options.
        /// </summary>
        /// <remarks>The generated PDF will use A4 page size and apply the specified margins and
        /// orientation. Header and footer HTML will be included if enabled in the options. JavaScript is enabled during
        /// conversion, and the web page width is set to 1024 pixels.</remarks>
        /// <param name="html">The HTML content to convert to PDF. Cannot be null.</param>
        /// <param name="options">The options that configure PDF generation, including page size, orientation, margins, and header/footer
        /// settings. Cannot be null.</param>
        /// <returns>A byte array containing the generated PDF document. The array will be empty if the conversion produces no
        /// content.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> or <paramref name="options"/> is null.</exception>
        public byte[] FromHtml(string html, PdfDocumentOptions options)
        {
            if (html is null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var baseUrl = options.BaseUrl ?? string.Empty;

            var converter = new HtmlToPdf
            {
                Options =
                {
                    PdfPageSize = PdfPageSize.A4,
                    PdfPageOrientation = options.Landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait,

                    MarginTop = options.MarginTop,
                    MarginRight = options.MarginRight,
                    MarginBottom = options.MarginBottom,
                    MarginLeft = options.MarginLeft,

                    JavaScriptEnabled = true,
                    WebPageWidth = 1024
                }
            };

            converter.Options.WebPageWidth = 1280;
            converter.Options.WebPageHeight = 0;

            converter.Options.JpegCompressionEnabled = false;
            converter.Options.PdfCompressionLevel = PdfCompressionLevel.NoCompression;
            converter.Options.RenderingEngine = RenderingEngine.WebKit;

            if (options.DisplayHeader && !string.IsNullOrWhiteSpace(options.HeaderHtml))
            {
                converter.Options.DisplayHeader = true;
                converter.Header.DisplayOnFirstPage = true;
                converter.Header.DisplayOnOddPages = true;
                converter.Header.DisplayOnEvenPages = true;

                converter.Header.Height = 60;

                converter.Header.Add(new PdfHtmlSection(options.HeaderHtml, baseUrl));
            }

            if (options.DisplayFooter && !string.IsNullOrWhiteSpace(options.FooterHtml))
            {
                converter.Options.DisplayFooter = true;
                converter.Footer.DisplayOnFirstPage = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnEvenPages = true;

                converter.Footer.Height = 60;

                converter.Footer.Add(new PdfHtmlSection(options.FooterHtml, baseUrl));
            }

            var doc = converter.ConvertHtmlString(html, baseUrl);

            try
            {
                return doc.Save();
            }
            finally
            {
                doc.Close();
            }
        }
    }
}