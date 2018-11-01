using Dapper;
using DigitalTVBilling.Infrastructure.IntefaceClass;
using DigitalTVBilling.ListModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class DamageDistinguishedAnswers : IOrderUserStatus
    {
        private readonly IDbConnection db;
        private readonly ReserveFilter reserveFilter;

        public DamageDistinguishedAnswers(IDbConnection db, ReserveFilter reserveFilter)
        {
            this.db = db;
            this.reserveFilter = reserveFilter;

        }
        public List<IdName> Result()
        {
            return db.Query<IdName>($"SELECT [dbo].[DamageUserReason]([reserve_answer]) AS Name, COUNT(*) AS Id FROM [dbo].[DamageReserveAnswer] {reserveFilter.FilterResult()}  GROUP BY [reserve_answer]").ToList();
        }

    }
}