using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Pagination
{
    // This is the class that we send back to the browser
    public class PaginatedResult<T> where T : class
    {
        public PaginatedResult(IReadOnlyList<T> items, int totalItemsCount, int pageNumber, int pageSize, int totalPages)
        {
            Items = items;
            TotalItemsCount = totalItemsCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        public IReadOnlyList<T> Items { get; set; }
        public int TotalItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
