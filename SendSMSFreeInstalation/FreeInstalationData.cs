using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.SendSMSFreeInstalation
{
    public class FreeInstalationData
    {
        public FreeInstalationData() { }

        public List<FreeInstalationModel> SendSMSData()
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<FreeInstalationModel>(@"SELECT cus.phone1,c.id,c.finish_date FROM book.Cards AS c 
                                                                        INNER JOIN book.Customers AS  cus ON cus.id=c.customer_id 
                                                                        INNER JOIN doc.Subscribes AS s ON c.id=s.card_id
                                                                        LEFT JOIN doc.SubscriptionPackages AS sp ON sp.subscription_id=s.id
                                                                        LEFT JOIN book.Packages AS p ON p.id=sp.package_id where p.id=304086 and s.status=1  and DATEDIFF(day, '" + DateTime.Now + "', c.finish_date)=1").ToList();

            }
        }
        public string ReturnMessageText()
        {
            using (DataContext _db = new DataContext())
            {
                return _db.MessageTemplates.Where(m => m.Name == "On_ShareFreeInstalation15").Select(m => m.Desc).FirstOrDefault();
            }
        }

        public void SaveMessageLogging(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                _db.MessageLoggings.Add(new MessageLogging()
                {
                    card_id = card_id,
                    tdate = DateTime.Now,
                    status = MessageLoggingStatus.FreeInstalation,
                    message_id = -2


                });
                _db.SaveChanges();
            }
        }
        public string returnMessage_Geo()
        {
            using (DataContext _db = new DataContext())
            {
                return _db.MessageTemplates.Where(m => m.Name == "On_ShareFreeInstalation15_Geo").Select(m => m.Desc).FirstOrDefault();
            }
        }
        public List<Param> returnParam()
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Params.ToList();
            }
        }
       
    }
}