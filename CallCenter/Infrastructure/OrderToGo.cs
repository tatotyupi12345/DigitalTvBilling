using Dapper;
using DigitalTVBilling.CallCenter.InterfaceUser;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class OrderToGo :IUserToGo
    {
        private readonly IDbConnection db;

        // private readonly int user_Id;


        public OrderToGo(IDbConnection db )
        {
            this.db = db;
            // user_Id = user_id;

        }

        public ToGoId Result(int user_Id)
        {
            return db.Query<ToGoId>($"SELECT o.to_go,o.id FROM doc.Orders as o where executor_id={user_Id} and to_go=1").FirstOrDefault();
        }
    }
}