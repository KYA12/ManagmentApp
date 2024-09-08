using System.Linq;
using System.Collections.Generic;
using System;

namespace BakeryOrderManagmentSystem.API.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex - 1;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var items = source.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 0;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageIndex + 1 < TotalPages;
            }
        }
    }
}