// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Models
{
    /// <summary>
    /// Represents a paged result set.
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// The data items for the current page.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        /// <summary>
        /// The total number of items available.
        /// </summary>
        public int TotalCount { get; set; }
    }
}