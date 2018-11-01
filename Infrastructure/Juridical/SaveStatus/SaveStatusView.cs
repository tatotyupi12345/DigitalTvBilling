using DigitalTVBilling.Infrastructure.Juridical.SaveStatus;
using DigitalTVBilling.Juridical;
using DigitalTVBilling.JuridicalInfo;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class SaveStatusView
    {
        private readonly StatusInfo statusInfo;
        private readonly int user_Id;
        private readonly CardInfo cardInfo;
        private readonly UpdateDeleteStatus updateDelete;
        private readonly SaveStatusInfos saveStatusInfos;
        private readonly SaveLoggings saveLoggings;
        private readonly DataContext db;

        public SaveStatusView(StatusInfo statusInfo, int user_id, CardInfo cardInfo, UpdateDeleteStatus updateDelete, DataContext db)
        {
            this.statusInfo = statusInfo;
            user_Id = user_id;
            this.cardInfo = cardInfo;
            this.updateDelete = updateDelete;
            this.db = db;
        }
        public ReturnJson Result()
        {
            if (cardInfo.Result() != null)
            {
                updateDelete.Delete();
            }
            if (statusInfo.statusArray.Length == 1)
            {
                updateDelete.Update();
            }

            statusInfo.statusArray.Select(st =>
                          new SaveStatusInfos(statusInfo.id, user_Id, st, db).
                              Result()).Select(sl =>
                                  new SaveLoggings(statusInfo.id, user_Id, 1, db).
                                      Result()
                      ).ToList();
            return new SaveJuridicalStatusLogging(db,statusInfo,user_Id, cardInfo.Result()).Result();
        }
    }
}