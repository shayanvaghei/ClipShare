using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Core.Pagination
{
    public class BaseParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize;
        private string _sortBy;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize || value < 0 ? MaxPageSize : value;
        }
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }
    }
}
