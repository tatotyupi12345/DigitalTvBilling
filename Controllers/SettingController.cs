using DigitalTVBilling.Filters;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class SettingController : BaseController
    {
        public ActionResult Index()
        {
            if (!Utils.Utils.GetPermission("SETTING_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(_db.Params.ToList());
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(List<Param> Params, string Logging)
        {
            using (DataContext _db = new DataContext())
            {
                List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(Logging);
                if (logs != null && logs.Count > 0)
                {
                    this.AddLoging(_db,
                        LogType.Settings,
                        LogMode.Change,
                        ((User)Session["CurrentUser"]).Id,
                        0, "", logs);
                }

                List<Param> b_p = _db.Params.ToList();

                _db.Params.RemoveRange(b_p);
                _db.Params.AddRange(Params);
                _db.SaveChanges();
                string old_percent = b_p.First(p => p.Name == "PackageDiscount").Value;
                string new_percent = Params.First(p => p.Name == "PackageDiscount").Value;
                if (old_percent != new_percent)
                {
                    var _cards = _db.Cards.Where(q => q.Subscribtions.Where(c => c.Status).Any())
                        .Where(c => c.CardStatus != CardStatus.Canceled).Select(c => new CardDetailData
                    {
                        CustomerType = c.Customer.Type,
                        IsBudget = c.Customer.IsBudget,
                        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        Card = c,
                        Subscribtion = c.Subscribtions.Where(s => s.Status).FirstOrDefault(),
                        SubscriptionPackages = c.Subscribtions.Where(s => s.Status).FirstOrDefault().SubscriptionPackages.ToList()
                    }).ToList();

                    List<Package> _packages = _db.Packages.ToList();
                    double discount = double.Parse(new_percent);
                    string charge_time = Params.First(p => p.Name == "CardCharge").Value;
                    decimal jurid_limit_months = decimal.Parse(Params.First(p => p.Name == "JuridLimitMonths").Value);
                    foreach (CardDetailData _card in _cards)
                    {
                        if (_card.Subscribtion != null)
                        {
                            int[] ids = _card.SubscriptionPackages.Select(s => s.PackageId).ToArray();

                            _card.Subscribtion.Amount = _packages.Where(p => ids.Contains(p.Id)).Sum(p => _card.CustomerType== CustomerType.Juridical ? p.JuridPrice : p.Price);
                            _card.Subscribtion.Amount -= (_card.Subscribtion.Amount * discount / 100);
                            _db.Entry(_card.Subscribtion).State = System.Data.Entity.EntityState.Modified;

                            Utils.Utils.ChangeFinishDateByPackage(_db, _card, charge_time, jurid_limit_months, (decimal)_card.Subscribtion.Amount);
                        }
                    }
                }

                string old_cas_address = b_p.First(p => p.Name == "CASAddress").Value;
                string new_cas_address = Params.First(p => p.Name == "CASAddress").Value;

                string old_message_time = b_p.First(p => p.Name == "MessageTime").Value;
                string new_message_time = Params.First(p => p.Name == "MessageTime").Value;

                if(old_cas_address != new_cas_address || old_message_time != new_message_time)
                {
                    Utils.Utils.RestartAppPool();
                }
            }

            return View(Params);
        }

        [HttpPost]
        public JsonResult UpdateRs()
        {
            try
            {
                return Json(new RsServiceFuncs(false).UpdateRsUser());
            }
            catch(Exception ex)
            {
                return Json(ex.ToString());
            }
        }
	}
}