using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace IReckonUAssignment
{
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        /// <summary>
        /// Gets the boundary.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="lengthLimit">The length limit.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">
        /// Missing content-type boundary.
        /// or
        /// Multipart boundary length limit {lengthLimit}
        /// </exception>
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (!boundary.HasValue || string.IsNullOrWhiteSpace(boundary.Value))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary.Value;
        }

        /// <summary>
        /// Determines whether [is multipart content type] [the specified content type].
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        ///   <c>true</c> if [is multipart content type] [the specified content type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Determines whether [has form data content disposition] [the specified content disposition].
        /// </summary>
        /// <param name="contentDisposition">The content disposition.</param>
        /// <returns>
        ///   <c>true</c> if [has form data content disposition] [the specified content disposition]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                   && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        /// <summary>
        /// Determines whether [has file content disposition] [the specified content disposition].
        /// </summary>
        /// <param name="contentDisposition">The content disposition.</param>
        /// <returns>
        ///   <c>true</c> if [has file content disposition] [the specified content disposition]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                       || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}