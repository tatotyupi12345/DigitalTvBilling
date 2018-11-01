using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.JuridicalInfo
{
    public class SaveLoggings
    {
        private readonly int cardID;
        private readonly int userId;
        private readonly int statusID;
        private readonly DataContext db;

        public SaveLoggings(
        int cardID,
        int userId,
        int statusID,
        DataContext db)
        {
            this.cardID = cardID;
            this.userId = userId;
            this.statusID = statusID;
            this.db = db;
        }
        public bool Result()
        {
            try
            {
                if (db.Database.SqlQuery<JuridicalLogging>("SELECT * FROM dbo.JuridicalLogging where card_id=" + cardID + " and status=" + statusID + "").FirstOrDefault() == null)
                {
                    db.JuridicalLoggings.Add(new JuridicalLogging()
                    {
                        tdate = DateTime.Now,
                        card_id = cardID,
                        user_id = userId,
                        status = statusID,
                        name = db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + userId).FirstOrDefault()

                    });
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}