// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Models
{
    /// <summary>
    /// Class used for entity list paging.
    /// </summary>
    public class Paging
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public string? Search { get; set; }

        private int _pageSize = 10;
        public int UserID { get; set; } = 0;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}