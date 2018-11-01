using Dapper;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalStatusView
    {
        private readonly int card_Id;
        private readonly IDbConnection db;

        public JuridicalStatusView(int card_id, IDbConnection db)
        {
            card_Id = card_id;
            this.db = db;
        }
        public List<JuridicalStatus> Result()
        {
            return db.Query<JuridicalStatus>($"SELECT * FROM dbo.JuridicalStatus where card_id={card_Id}").ToList();
        }
    }
}