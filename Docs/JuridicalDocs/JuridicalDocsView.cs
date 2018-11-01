using Dapper;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical.JuridicalDocs
{
    public class JuridicalDocsView
    {
        private readonly int card_Id;
        private readonly IDbConnection db;

        public JuridicalDocsView(int card_id, IDbConnection db) {
            card_Id = card_id;
            this.db = db;
        }
        public JuridicalDocsInfo Result()
        {
            return db.Query<JuridicalDocsInfo>($"SELECT * FROM [dbo].[JuridicalDocsInfo] where card_id= {card_Id}").FirstOrDefault();
        }
    }
}