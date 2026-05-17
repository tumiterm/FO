
namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents configuration options for generating a PDF document from HTML content.
    /// </summary>
    /// <remarks>Use this record to specify document metadata, page layout, margins, header and footer
    /// display, and HTML content for headers and footers when converting HTML to PDF. All properties are immutable and
    /// must be set at initialization. This type is intended to be passed to PDF generation APIs that support
    /// customizable output.</remarks>
    public record PdfDocumentOptions
    {
        /// <summary>
        /// Gets the title associated with the object.
        /// </summary>
        public string? Title { get; init; }

        /// <summary>
        /// Gets the top margin, in pixels, applied to the content.
        /// </summary>
        public int MarginTop { get; init; } = 20;

        /// <summary>
        /// Gets the right margin, in pixels, applied to the content.
        /// </summary>
        public int MarginRight { get; init; } = 15;

        /// <summary>
        /// Gets the bottom margin, in pixels, applied to the content.
        /// </summary>
        public int MarginBottom { get; init; } = 20;

        /// <summary>
        /// Gets the left margin, in pixels, applied to the content.
        /// </summary>
        public int MarginLeft { get; init; } = 15;

        /// <summary>
        /// Gets a value indicating whether the content is displayed in landscape orientation.
        /// </summary>
        public bool Landscape { get; init; }
        
        /// <summary>
        /// Gets a value indicating whether the header is displayed.
        /// </summary>
        public bool DisplayHeader { get; init; }

        /// <summary>
        /// Gets a value indicating whether the footer is displayed.
        /// </summary>
        public bool DisplayFooter { get; init; }

        /// <summary>
        /// Gets the HTML markup used for the header section.
        /// </summary>
        public string? HeaderHtml { get; init; }

        /// <summary>
        /// Gets the HTML content to be displayed in the footer section.
        /// </summary>
        public string? FooterHtml { get; init; }

        /// <summary>
        /// Used by the HTML->PDF converter to resolve relative paths (css/images).
        /// Example: https://localhost:5001/
        /// </summary>
        public string? BaseUrl { get; init; }
    }
}
