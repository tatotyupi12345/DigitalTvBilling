using Dapper;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalLogginView
    {
        private readonly int card_Id;
        private readonly IDbConnection db;

        public JuridicalLogginView(int card_id,IDbConnection db)
        {
            card_Id = card_id;
            this.db = db;
        }
        public List<JuridicalLogging> Result()
        {
            return db.Query<JuridicalLogging>($"SELECT * FROM dbo.JuridicalLogging where card_id={card_Id}").ToList();
        }
    }
}