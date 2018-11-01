using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public static class PagedListExtendedExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, Int32 pageNumber, Int32 pageSize)
        {
            return await PagedListExtended<T>.Create(superset, pageNumber, pageSize);
        }

        public static async Task<IPagedList<T>> ToRawPagedListAsync<T>(this DbRawSqlQuery<T> superset, Int32 queryCount, Int32 pageNumber, Int32 pageSize)
        {
            return await AsyncRawQueryPagedList<T>.Create(superset, queryCount, pageNumber, pageSize);
        }
    }
}