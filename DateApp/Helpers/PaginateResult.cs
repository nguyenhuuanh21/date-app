using Microsoft.EntityFrameworkCore;

namespace DateApp.Helpers
{
    public class PaginateResult<T>
    {
        public PaginationMetadata Metadata { get; set; } = default!;
        public List<T> Data { get; set; } = [];
    }

    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public static class PaginateHelper
    {
        public static async Task<PaginateResult<T>> CreateAsync<T>(IQueryable<T> query, int page, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginateResult<T>
            {
                Metadata = new PaginationMetadata
                {
                    TotalCount = count,
                    PageSize = pageSize,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize)
                },
                Data = items
            };
        }
    }
}
