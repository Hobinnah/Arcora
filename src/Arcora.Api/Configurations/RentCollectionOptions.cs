namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Configuration for the recurring rent collection background job.
    /// Bound from the "RentCollection" configuration section.
    /// </summary>
    public class RentCollectionOptions
    {
        public const string SectionName = "RentCollection";

        /// <summary>
        /// Whether the background scheduler is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// How often the scheduler scans for due rent, in minutes.
        /// </summary>
        public int PollingIntervalMinutes { get; set; } = 60;

        /// <summary>
        /// Hour of day (UTC) at which a lease anniversary charge becomes due.
        /// </summary>
        public int ChargeHourUtc { get; set; } = 9;
    }
}
