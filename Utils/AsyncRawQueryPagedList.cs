using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class AsyncRawQueryPagedList<T> : BasePagedList<T>
    {
        public AsyncRawQueryPagedList()
        {

        }
        public static async Task<IPagedList<T>> Create(DbRawSqlQuery<T> superset, int count, int pageNumber, int pageSize)
        {
            var list = new AsyncRawQueryPagedList<T>();
            await list.Init(superset, count, pageNumber, pageSize);
            return list;
        }

        async Task Init(DbRawSqlQuery<T> superset, int queryCount, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");
            TotalItemCount = superset == null ? 0 : queryCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
            PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < PageCount;
            IsFirstPage = PageNumber == 1;
            IsLastPage = PageNumber >= PageCount;
            FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
            var num = FirstItemOnPage + PageSize - 1;
            LastItemOnPage = num > TotalItemCount ? TotalItemCount : num;
            if (superset == null || TotalItemCount <= 0)
                return;

            Subset.AddRange(await superset.ToListAsync());
        }
    }
}