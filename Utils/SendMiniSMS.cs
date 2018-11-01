using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class SendMiniSMS
    {
        public SendMiniSMS() { }

        public void SaveMiniSMSData(int card_num,int card_id,short [] cas_ids,DateTime cas_date,DateTime finish_date,int status,bool mini_SMS, int send_status)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    if (_db.Params.Where(p => p.Name == "MiniSMS").Select(s => s.Value).FirstOrDefault() == "0")
                    {
                        string result = "";
                        if (cas_ids.Length == 1)
                        {
                            result = Convert.ToString(cas_ids[0]);
                        }
                        else
                        {
                            result = Convert.ToString(cas_ids[0]) + " , " + Convert.ToString(cas_ids[1]);
                        }

                        _db.MiniSMS.Add(new MiniSMS
                        {
                            tdate = DateTime.Now,
                            card_num = card_num,
                            card_id = card_id,
                            cas_ids = result,
                            cas_date = cas_date,
                            finish_date = finish_date,
                            status = status,
                            mini_sms = Convert.ToInt32(mini_SMS),
                            send_status = send_status
                        });
                        _db.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    var _ex = ex;
                }
            }
        }
        public enum StatusMiniSMS
        {
            [Description("Entitle-განახლება")]
            EntitleRefresh,
            [Description("Entitle-წაშლა")]
            EntitleDelete,
            [Description("დარიცხვები")]
            Charges,
            [Description("ავტომატური პაკეტები")]
            AutoPackages,
            [Description("ჩარიცხვა")]
            Payment,
            [Description("ჩარიცხვა BOG")]
            PaymentBOG,
            [Description("ჩარიცხვა TBC")]
            PaymentTBC,
            [Description("ჩარიცხვა NOVA")]
            PaymentNOVA,
            [Description("ჩარიცხვა LIB")]
            PaymentLIB,
            [Description("ჩარიცხვა UCC")]
            PaymentUCC,
            [Description("ჩარიცხვა UPS")]
            PaymentUPS,


        }
    }
}