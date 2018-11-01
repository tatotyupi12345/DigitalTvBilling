using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class StaffsEmailsJob: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            string sql = @"SELECT DISTINCT c.name+' '+c.lastname cr.id FROM book.Cards AS cr 
                                INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status!=4";

            var path = Path.GetTempPath();
            using (DataContext _db = new DataContext())
            {
                List<AutoMessageTemplate> queries = _db.AutoMessageTemplates.ToList();
                string text = string.Empty;
                List<string> receivers = _db.Params.First(p=>p.Name == "Emails").Value.Split(';').ToList();
                foreach (var _query in queries)
                {
                    path = Path.Combine(path, Guid.NewGuid().ToString() + ".txt");
                    text += "დღეს აბონენტებს გაეგზავნება " + _query.MessageText + " სახის შეტყობინება \r\n\r\n";
                    File.WriteAllText(path, String.Join("\r\n", _db.Database.SqlQuery<int>(sql + _query.Query).ToArray()));

                    Utils.Utils.SendEmail(new List<string>(), "", "glovaltv.ge", 25, false, "billing@globaltv.ge", "billing", "@qwerty77", receivers, new List<string>(), text);
                    File.Delete(path);
                }
            }
        }
    }
}