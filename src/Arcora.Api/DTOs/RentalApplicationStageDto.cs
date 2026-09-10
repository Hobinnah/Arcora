namespace Arcora.Api.DTOs
{
    /// <summary>
    /// A single rental application stage exposed to the frontend so status values stay in sync
    /// with the backend enum.
    /// </summary>
    public class RentalApplicationStageDto
    {
        /// <summary>
        /// Canonical value stored in the database and used in API calls (e.g. "SUBMITTED").
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Human-friendly label for display in the UI (e.g. "Submitted").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short description of what the stage means.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Numeric ordinal of the stage, useful for ordering the stages in the UI.
        /// </summary>
        public int Order { get; set; }
    }
}
