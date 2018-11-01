using DigitalTVBilling.Juridical;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical.SaveStatus
{
    public class UpdateDeleteStatus
    {
        private readonly DataContext db;
        private readonly StatusInfo statusInfo;

        public UpdateDeleteStatus(DataContext db, StatusInfo statusInfo) {
            this.db = db;
            this.statusInfo = statusInfo;
        }
        public void Delete()
        {
            db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[JuridicalStatus] where card_id=" + statusInfo.id + "");
            db.Database.ExecuteSqlCommand("UPDATE [book].[Cards] SET [juridical_verify_status] ='" + String.Join(",", statusInfo.statusArray.Select(s => s.ToString()).ToArray()) + "' where id=" + statusInfo.id + "");
            db.SaveChanges();
        }
        public void Update()
        {
            db.Database.ExecuteSqlCommand("UPDATE [book].[Cards] SET [juridical_verification] ='" + statusInfo.statusArray[0] + "' where id=" + statusInfo.id);
            db.SaveChanges();
        }
    }
}