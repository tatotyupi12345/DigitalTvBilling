using Dapper;
using DigitalTVBilling.CallCenter.InterfaceUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class Users : IUser
    {
        private readonly IDbConnection _Db;

        public Users(IDbConnection _db)
        {
            _Db = _db;
        }
        public List<CallUser> Result()
        {
            return _Db.Query<CallUser>(@"SELECT * FROM book.Users where type=4 or type=44").ToList();
        }
    }
}