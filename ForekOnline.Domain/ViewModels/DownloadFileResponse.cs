using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public record DownloadFileResponse(

        /// <summary>
        /// The file stream.
        /// </summary>
        [Required]
        Stream FileStream,

        /// <summary>
        /// The original file name.
        /// </summary>
        [Required]
        string FileName,

        /// <summary>
        /// File size in bytes.
        /// </summary>
        long FileSizeBytes,

        /// <summary>
        /// The MIME type.
        /// </summary>
        string? ContentType = null
    );

}
