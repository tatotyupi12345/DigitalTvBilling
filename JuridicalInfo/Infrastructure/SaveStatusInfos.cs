using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.JuridicalInfo
{
    public class SaveStatusInfos
    {
        private readonly int cardID;
        private readonly int userId;
        private readonly int statusID;

        public SaveStatusInfos(
        int cardID,
        int userId,
        int statusID,
        DataContext db)
        {
            this.cardID = cardID;
            this.userId = userId;
            this.statusID = statusID;
            Db = db;
        }

        public DataContext Db { get; }

        public bool Result()
        {
            try
            {
                Db.JuridicalStatus.Add(new JuridicalStatus()
                {
                    tdate = DateTime.Now,
                    card_id = cardID,
                    user_id = userId,
                    status = statusID

                });
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}