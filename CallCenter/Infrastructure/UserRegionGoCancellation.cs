using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class UserRegionGoCancellation
    {
        private readonly IDbConnection db;

        public UserRegionGoCancellation(IDbConnection db)
        {
            this.db = db;
        }

        public List<RegionNameID> Result(int user_id)
        {
            var xx = db.Query<RegionNameID>($"SELECT o.data,id FROM dbo.Cancellation as o where " + new DateFrom().Datetime() + " and status!=2 and  executor_id=" + user_id).ToList();
            return db.Query<RegionNameID>($"SELECT o.data,id FROM dbo.Cancellation as o where " + new DateFrom().Datetime() + " and (status!=2 and status!=3 ) and  executor_id=" + user_id).ToList();
        }
    }
}