using DigitalTVBilling.Juridical;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical.SaveStatus
{
    public class SaveJuridicalStatusLogging
    {
        private readonly DataContext _db;
        private readonly StatusInfo statusInfo;
        private readonly int user_Id;
        private readonly CardNumID card;

        public SaveJuridicalStatusLogging(DataContext db, StatusInfo statusInfo, int user_id, CardNumID card)
        {
            this._db = db;
            this.statusInfo = statusInfo;
            user_Id = user_id;
            this.card = card;
        }
        public ReturnJson Result()
        {
            try {
                _db.Loggings.Add(new Logging()
                {
                    Tdate = DateTime.Now,
                    UserId = user_Id,
                    Type = LogType.Card,
                    Mode = LogMode.JuridVerify,
                    TypeValue = card.card_num,
                    TypeId = card.id
                });

                _db.SaveChanges();
                return new ReturnJson
                {
                    Status = _db.Database.SqlQuery<int>($"SELECT s.status FROM dbo.JuridicalStatus s where card_id={statusInfo.id}").ToList(),
                    ID = 1
                };

            }
            catch
            {
                return new ReturnJson
                {
                    Status = null,
                    ID = 0
                };
            }
        }
    }
}