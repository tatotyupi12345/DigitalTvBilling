using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class StatusQuery : IStaticStatus
    {
        private readonly IDbConnection db;
        private readonly string query;

        public StatusQuery(IDbConnection db, string Query)
        {
            this.db = db;
            query = Query;
        }
        public List<int> Status()
        {
            return db.Query<int>($"SELECT * FROM {query}").ToList();
        }
    }
}