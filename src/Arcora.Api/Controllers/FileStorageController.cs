using Arcora.Api.Enums;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    /// <summary>
    /// Endpoints for uploading, downloading and deleting files, documents and images
    /// stored in Azure Blob Storage (Azurite during development).
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService)
        {
            this.fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Uploads a file to the container matching the given category.
        /// </summary>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "UploadFile")]
        [RequestSizeLimit(52428800)] // 50 MB
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] StorageCategory category, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file was provided." });

            await using var stream = file.OpenReadStream();
            var result = await fileStorageService.UploadAsync(category, file.FileName, stream, file.ContentType, cancellationToken);
            return CreatedAtAction(nameof(GetSasUrl), new { category, blobName = result.BlobName }, result);
        }

        /// <summary>
        /// Downloads a blob's raw content.
        /// </summary>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet(Name = "DownloadFile")]
        public async Task<IActionResult> Download([FromQuery] StorageCategory category, [FromQuery] string blobName, CancellationToken cancellationToken)
        {
            var result = await fileStorageService.DownloadAsync(category, blobName, cancellationToken);
            if (result == null)
                return NotFound(new { message = "The specified blob was not found." });

            return File(result.Value.Content, result.Value.ContentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Generates a time-limited read-only SAS URL for a blob.
        /// </summary>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet(Name = "GetSasUrl")]
        public async Task<IActionResult> GetSasUrl([FromQuery] StorageCategory category, [FromQuery] string blobName, CancellationToken cancellationToken)
        {
            var url = await fileStorageService.GetReadSasUrlAsync(category, blobName, cancellationToken);
            if (url == null)
                return NotFound(new { message = "The specified blob was not found." });

            return Ok(new { url });
        }

        /// <summary>
        /// Deletes a blob.
        /// </summary>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete(Name = "DeleteFile")]
        public async Task<IActionResult> Delete([FromQuery] StorageCategory category, [FromQuery] string blobName, CancellationToken cancellationToken)
        {
            var deleted = await fileStorageService.DeleteAsync(category, blobName, cancellationToken);
            if (!deleted)
                return NotFound(new { message = "The specified blob was not found." });

            return Ok(new { message = "Blob deleted successfully." });
        }
    }
}
