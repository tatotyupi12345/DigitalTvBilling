using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class UserData
    {
        private readonly DataContext db;

        public UserData(DataContext db)
        {
            this.db = db;
        }
        public CallModel Execute()
        {
            return new CallModel
            {
                users = db.Database.SqlQuery<CallUser>(@"SELECT * FROM book.Users  where type=4 or type=44 ").ToList(),
            };
        }
    }
}