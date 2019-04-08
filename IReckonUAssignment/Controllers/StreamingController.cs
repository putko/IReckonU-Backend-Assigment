using IReckonUAssigment.Business;
using IReckonUAssignment.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IReckonUAssignment.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public class ValidateMimeMultipartContentFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateMimeMultipartContentFilter"/> class.
        /// </summary>
        /// <param name="allowedExtensions">The allowed extensions.</param>
        public ValidateMimeMultipartContentFilter(params string[] allowedExtensions)
        {
        }

        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(context.HttpContext.Request.ContentType))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/Import")]
    [ApiController]
    public class StreamingController : Controller
    {
        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        /// <summary>
        /// The importer
        /// </summary>
        private readonly IImportService importer;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<StreamingController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="importer">The importer.</param>
        public StreamingController(ILogger<StreamingController> logger,
            IImportService importer)
        {
            this.logger = logger;
            this.importer = importer;
        }

        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        /// <summary>
        /// Uploads this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">Form key count limit {StreamingController._defaultFormOptions.ValueCountLimit}</exception>
        [HttpPost]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        [ValidateMimeMultipartContentFilter("csv")]
        public async Task<IActionResult> Upload()
        {
            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();
            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                StreamingController._defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);

                            this.logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = StreamingController.GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }

                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount > StreamingController._defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException(
                                    $"Form key count limit {StreamingController._defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            await this.importer.ImportDataFromCSV(targetFilePath);
            return Ok();
        }
        
        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
}