using Dapper;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalStatusList
    {
        private readonly IDbConnection db;

        public JuridicalStatusList(IDbConnection db) {
            this.db = db;
        }
        public List<JuridicalStatus> Result()
        {
            return db.Query<JuridicalStatus>("SELECT * FROM dbo.JuridicalStatus").ToList();
        }
    }
}