using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(IEnumerable<T> items, int totalItemsCount, int pageNumber, int pageSize, int totalPages)
        {
            TotalItemsCount = totalItemsCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;

            AddRange(items);
        }

        public int TotalItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }


        #region Static Methods
        // Executes against the database and generates a PaginatedList of type T and initializing the values
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalItemsCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItemsCount / (double)pageSize);

            if (pageNumber > totalPages && totalPages > 0)
            {
                // set to the last page
                pageNumber = totalPages;
            }

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, totalItemsCount, pageNumber, pageSize, totalPages);
        }
        #endregion
    }
}
