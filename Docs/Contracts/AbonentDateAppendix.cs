using Dapper;
using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class AbonentDateAppendix
    {
        private readonly IDbConnection db;
        private readonly string custumer_Id;

        public AbonentDateAppendix(IDbConnection db,string custumer_id)
        {
            this.db = db;
            custumer_Id = custumer_id;
        }

        public List<CustomerSellAttachments> Result()
        {
            return db.Query<CustomerSellAttachments>($"SELECT * FROM dbo.CustomerSellAttachments where customer_id={363345}").ToList();  
        }
    }
}