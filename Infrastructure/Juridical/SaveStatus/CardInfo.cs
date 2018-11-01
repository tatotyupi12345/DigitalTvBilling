using Dapper;
using DigitalTVBilling.Juridical;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical.SaveStatus
{
    public class CardInfo
    {
        private readonly IDbConnection _db;
        private readonly int id;

        public CardInfo(IDbConnection _Db,int id) {
            _db = _Db;
            this.id = id;
        }
        public CardNumID Result()
        {
            return _db.Query<CardNumID>($"SELECT c.id,c.card_num FROM book.Cards c where c.id={id}").FirstOrDefault();
        }
    }
}