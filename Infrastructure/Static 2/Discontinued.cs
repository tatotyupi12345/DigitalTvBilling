using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Static_2
{
    public class Discontinued : IStaticStatus
    {
        private readonly IDbConnection db;

        public Discontinued(IDbConnection db)
        {
            this.db = db;
        }
        public List<int> Status()
        {
            return db.Query<int>($"SELECT * FROM Discontinued").ToList();
        }
    }
}