using Microsoft.AspNetCore.Http;

namespace Arcora.Api.Models
{
    /// <summary>
    /// Multipart form payload for uploading a listing photo (image) together with metadata.
    /// </summary>
    public class ListingPhotoUploadRequest
    {
        /// <summary>
        /// The image file to upload.
        /// </summary>
        public IFormFile? File { get; set; }

        /// <summary>
        /// FK to Listing.
        /// </summary>
        public Guid? ListingID { get; set; }

        /// <summary>
        /// Photo location (LivingRoom, Bedroom, Bathroom, Laundry, Exterior, Additional)
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Photo caption
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Alternative text for photo
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Order of display
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Indicates if photo is cover photo
        /// </summary>
        public bool IsCoverPhoto { get; set; }

        /// <summary>
        /// Identifier of the user who captured the photo.
        /// </summary>
        public string? CapturedBy { get; set; }

        /// <summary>
        /// Optional numeric user id who captured the photo.
        /// </summary>
        public long? UserID { get; set; }
    }
}
