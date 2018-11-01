using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class Cancel : IStaticStatus
    {
        private readonly IDbConnection db;

        public Cancel(IDbConnection db)
        {
            this.db = db;
        }
        public List<int> Status()
        {
            return db.Query<int>($"SELECT * FROM Cancel").ToList();
        }
    }
}