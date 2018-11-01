using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using DigitalTVBilling.ListModels;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System.Data;
using PagedList.Mvc;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using PagedList;
using System.Globalization;

namespace DigitalTVBilling.Controllers
{

    public class ReturnedController : Controller
    {
        //private int pageSize = 20;
        // GET: Returned
        public ActionResult Index(int? page, string name, string dt_from, string dt_to, int? drp_filter)
        {
            if (!Utils.Utils.GetPermission("RETURNED_CARDS_SHOW"))
            {
                return new RedirectResult("/Abonent");
            }

            List<ReturnedCard> _returned;
            using (DataContext _db = new DataContext())
            {

                DateTime dateFrom, dateTo;
                if (dt_from == "" || dt_to == "")
                {
                    dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                    dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
                }
                else
                {
                    dateFrom = Utils.Utils.GetRequestDate(dt_from, true);
                    dateTo = Utils.Utils.GetRequestDate(dt_to, false);
                }

                _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Tdate).Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).ToList();
                if (name != null && name != "")
                {
                    if (drp_filter != null)
                        switch (drp_filter)
                        {
                            case 1:
                                var fullname = name.Split(' ');

                                string fname = "", lname = "";
                                if (fullname.Length > 0)
                                {
                                    if (fullname.Length > 1)
                                    {
                                        fname = fullname[0];
                                        lname = fullname[1];
                                    }
                                    else
                                        fname = fullname[0];
                                }
                                if (fullname[0] != "undefined")
                                    _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Tdate).Where(c => (c.Card.Customer.Name.Contains(fname) && c.Card.Customer.LastName.Contains(lname)) || (c.Card.Customer.Name.Contains(lname) && c.Card.Customer.LastName.Contains(fname))).ToList();
                                break;
                            case 2:
                                _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Id).Where(c => c.Card.CardNum.Contains(name)).ToList();
                                break;
                            case 3:
                                _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Id).Where(c => c.Card.Customer.Code.Contains(name)).ToList();
                                break;
                            case 4:
                                var fullname_bort = name.Split(' ');

                                string fname_bort = "", lname_bort = "";
                                if (fullname_bort.Length > 0)
                                {
                                    if (fullname_bort.Length > 1)
                                    {
                                        fname_bort = fullname_bort[0];
                                        lname_bort = fullname_bort[1];
                                    }
                                    else
                                        fname_bort = fullname_bort[0];
                                }
                                _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Id).Where(c => (c.User_bort.Name.Contains(fname_bort) && c.User_bort.Name.Contains(lname_bort)) || (c.User_bort.Name.Contains(lname_bort) && c.User_bort.Name.Contains(fname_bort))).ToList();
                                break;
                            case 5:
                                var fullname_user = name.Split(' ');

                                string fname_user = "", lname_user = "";
                                if (fullname_user.Length > 0)
                                {
                                    if (fullname_user.Length > 1)
                                    {
                                        fname_user = fullname_user[0];
                                        lname_user = fullname_user[1];
                                    }
                                    else
                                        fname_bort = fullname_user[0];
                                }
                                //var user_id = _db.Users.Where(c => (c.Name.Contains(fname_user) && c.Name.Contains(lname_user)) || (c.Name.Contains(lname_user) && c.Name.Contains(fname_user))).Select(s => s.Id).FirstOrDefault();
                                _returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Id).Where(c => (c.Card.User.Name.Contains(fname_user) && c.Card.User.Name.Contains(lname_user)) || (c.Card.User.Name.Contains(lname_user) && c.Card.User.Name.Contains(fname_user))).ToList();
                                //_returned = _db.ReturnedCards.Include("User_bort").Include("User").Include("Card.Subscribtions.SubscriptionPackages.Package").Include("Card.Customer").OrderByDescending(o => o.Id).Where(c => c.Card.UserId==user_id).ToList();
                                break;
                            default:
                                break;
                        }

                }

                List<CommisionDesc> comms = new List<CommisionDesc>();


                JArray amount, commision_type;
                for (int i = 0; i < _returned.Count(); i++)
                {
                    JObject parsed = JObject.Parse(_returned[i].commission);

                    commision_type = (JArray)parsed["commisionType"];
                    amount = (JArray)parsed["amount"];
                    List<double> amounts = new List<double>();// = amount.ToList();
                    List<int> comm_types = new List<int>();

                    foreach (var item in amount.ToList())
                    {
                        amounts.Add(Convert.ToDouble(item));

                    }

                    foreach (var item in commision_type.ToList())
                    {
                        comm_types.Add(Convert.ToInt32(item));
                    }
                    comms.Add(new CommisionDesc { card_id = _returned[i].card_id, amount = amounts, commision_type = comm_types });

                }
                List<ReturnedCardAttachment> attamantes = _db.ReturnedCardAttachments.Include("ReceiverAttachment").ToList();
                List<CardCharge> _charges;
                List<Payment> _payments;
                List<Abonent> abonent = new List<Abonent>();
                decimal sum_attamantes = 0;
                int[] card_ids;
                int[] canceled = _db.Cards.Where(cr => cr.CardStatus == CardStatus.Canceled).Select(cr => cr.Id).ToArray();
                for (int k = 0; k < _returned.Count(); k++)
                {
                    Abonent ab = new Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card() } };
                    int? id = _returned[k].Card.Customer.Id;
                    var _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package")
                       .Include("CardServices").Include("CardDamages").Where(c => c.CustomerId == id)
                       .ToListAsync();
                    ab.Cards = _card.Result;
                    card_ids = ab.Cards.Where(c => c.Id == _returned[k].card_id).Select(cr => cr.Id).ToArray();
                    _charges = _db.CardCharges.Where(c => card_ids.Contains(c.CardId)).ToList();
                    _payments = _db.Payments.Where(c => card_ids.Contains(c.CardId)).ToList();
                    sum_attamantes += (decimal)(attamantes.Where(a => a.ReturnedCardsID == _returned[k].Id).Select(a => a.ReceiverAttachment.Price).Sum());
                    ab.AbonentDetailInfo = new AbonentDetailInfo
                    {
                        CanceledCardAmount = Math.Round(_payments.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum() - _charges.Where(c => canceled.Contains(c.CardId)).Select(c => c.Amount).Sum(), 3),
                    };
                    ab.UserID = _returned[k].Id;
                    abonent.Add(ab);
                }
                double amount_cash = 0;
                double amount_cashcashless = 0;
                for (int j = 0; j < comms.Count(); j++)
                {
                    if (comms[j].commision_type.Count() > 0)
                    {
                        if (comms[j].commision_type[0] == 2)
                        {
                            amount_cash += comms[j].amount[0];
                        }
                        if (comms[j].commision_type.Count() > 1)
                        {
                            amount_cashcashless += comms[j].amount[1];
                        }
                    }
                }
                ViewBag.returned_balance = abonent;
                ViewBag.commDesc = comms;
                ViewBag.Cash = amount_cash;
                ViewBag.Cashcashless = amount_cashcashless;
                ViewBag.ReturnedList = _returned.ToPagedList(page ?? 1, 20);
                ViewBag.attachments = attamantes;
                ViewBag.sum_attamantes = sum_attamantes;
                //ViewBag.privilegies = privilegies_return;
            }

            return View(_returned.ToPagedList(page ?? 1, 20));
        }
        [HttpPost]
        public PartialViewResult GetReturnedCardCancle(int returned_card_id)
        {
            using (DataContext _db = new DataContext())
            {
                var ReturnedCardCancel = _db.ReturnedCards.Include("ReturnedCardAttachments").Where(c => c.Id == returned_card_id).Select(s => s).ToList();
                int card_id = ReturnedCardCancel.Where(c => c.Id == returned_card_id).Select(s => s.card_id).FirstOrDefault();
                double commision_ammount = 0;
                string comm = _db.Params.Where(p => p.Name == "ReturnedCardCommision").FirstOrDefault().Value;
                if (comm == null || comm == "")
                {
                    throw new Exception("საკომისიოს რაოდენობა ვერ მოიძებნა!");
                }

                double.TryParse(comm, out commision_ammount);
                ViewBag.CommisionAmount = commision_ammount;
                Card _card = _db.Cards.Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.Id == card_id).FirstOrDefault();
                if (_card != null)
                {
                    var __card = _db.Cards.Where(c => c.Id == _card.Id).Select(c => new CardDetailData
                    {
                        CustomerType = c.Customer.Type,
                        IsBudget = c.Customer.IsBudget,
                        SubscribAmount = c.Subscribtions.FirstOrDefault(s => s.Status).Amount,
                        CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                        PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                        ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        Card = c,
                        MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                        ServiceAmount = c.CardCharges.Where(s => s.Status == CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        WithoutServiceAmount = c.CardCharges.Where(s => s.Status != CardChargeStatus.Service).Select(s => (decimal?)s.Amount).Sum() ?? 0,
                        CardServices = c.CardServices.ToList()
                    }).First();

                    decimal balance = Utils.Utils.GetBalance(__card.PaymentAmount, __card.ChargeAmount);
                    ViewBag.Balance = balance;
                    if (balance >= (decimal)commision_ammount)
                    {
                        ViewBag.Amount = balance - (decimal)commision_ammount;
                    }
                    else
                    {
                        ViewBag.Amount = 0;
                    }
                    ViewBag.card_id = card_id;
                }
                ViewBag.attachmenlist = _db.ReceiverAttachments.ToList();

                List<User> user_info = _db.Users.Include("UserType").Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5).ToList();
                // ნაღდი უნაღდოს ამოღება
                string[] arrraytechnical = _db.ReturnedCards.Where(c => c.Id == returned_card_id).Select(ss => ss.commission).ToArray();
                Commisions _returne_cash = new Commisions();
                CashCashles returncard_cash = new CashCashles();
                returncard_cash = _returne_cash.ReturnedCard_commision_arr(arrraytechnical);
                ViewBag.returncard_cash = returncard_cash;
                ViewBag.ReturnedCardCancel = ReturnedCardCancel.Where(c => c.Id == returned_card_id).FirstOrDefault();
                ViewBag.ReturnedCardAttachments = ReturnedCardCancel.Select(s => s.ReturnedCardAttachments.Select(ss => ss).ToList()).FirstOrDefault();
                ViewBag.Returned_Amount = ReturnedCardCancel.Select(s => s.returned_amount).FirstOrDefault();
                return PartialView("~/Views/Returned/_ReturnedCardCancl.cshtml", user_info);
            }
        }
        //[HttpPost]
        //public JsonResult ReturnedCancleDelete(int card_id, string date)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        try
        //        {
        //            DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
        //            DateTime date_cancle = Convert.ToDateTime(date, usDtfi);
        //            var card_charges = _db.CardCharges.Where(c => c.CardId == card_id).Select(s => s).ToList();
        //            var card_payments = _db.Payments.Where(c => c.CardId == card_id).Select(s => s).ToList();
        //            var card_returneds = _db.ReturnedCards.Where(c => c.card_id == card_id).Select(s => s).ToList();
        //            var id = card_returneds.Select(s => s.Id).FirstOrDefault();
        //            var card_returneds_attachments = _db.ReturnedCardAttachments.Where(c => c.ReturnedCardsID == id).Select(s => s).ToList();

        //            _db.ReturnedCardAttachments.RemoveRange(card_returneds_attachments);
        //            _db.ReturnedCards.RemoveRange(card_returneds);
        //            _db.SaveChanges();
        //            foreach (var item_charges in card_charges)
        //            {
        //                if (item_charges.Tdate.ToString("yyyyMMddHHmm") == date_cancle.ToString("yyyyMMddHHmm"))
        //                {
        //                    _db.CardCharges.Remove(item_charges);
        //                    _db.SaveChanges();
        //                }
        //            }
        //            foreach (var item_payments in card_payments)
        //            {
        //                if (item_payments.Tdate.ToString("yyyyMMddHHmm") == date_cancle.ToString("yyyyMMddHHmm"))
        //                {
        //                    _db.Payments.Remove(item_payments);
        //                    _db.SaveChanges();
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {

        //            return Json(1);
        //        }
        //    }

        //        return Json(0);
        //}
        [HttpPost]
        public PartialViewResult GetReturnedBort(int card_id)
        {
            List<BortHistory> bort_history;
            using (DataContext _db = new DataContext())
            {

                List<User> user_info = _db.Users.Include("UserType").Where(c => c.UserType.Name.Contains("დილერი") || c.UserType.Id == 1 || c.UserType.Id == 2 || c.UserType.Id == 4 || c.UserType.Id == 5).ToList();
                bort_history = _db.BortHistorys.Where(c => c.ReturnedCardID == card_id).ToList();
                ViewBag.Bort = user_info;
                ViewBag.ReturnedCardID = card_id;
                ViewBag.Bort_History = bort_history;
            }
            return PartialView("~/Views/Returned/_bort.cshtml", bort_history);
        }


        [HttpPost]
        public JsonResult RecordApprove(int card_id)
        {
            if (!Utils.Utils.GetPermission("RETURNED_CARDS_VERIFY"))
            {
                return Json(0);
            }
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        var returne_card = _db.ReturnedCards.Include("Card").Where(c => c.Id == card_id).FirstOrDefault();
                        if (returne_card != null)
                        {
                            returne_card.approve = 1;
                            _db.Entry(returne_card).State = EntityState.Modified;

                            _db.Loggings.Add(new Logging()
                            {
                                Tdate = DateTime.Now,
                                UserId = user_id,
                                Type = LogType.Card,
                                Mode = LogMode.Change,
                                TypeValue = returne_card.Card.CardNum,
                                TypeId = returne_card.card_id
                            });

                            _db.SaveChanges();

                            tran.Commit();
                            return Json(1);
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }

        [HttpPost]
        public JsonResult UpdateEntry(int bortID, int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    DateTime dateFrom = DateTime.Now;
                    _db.BortHistorys.Add(new BortHistory
                    {
                        Tdate = dateFrom,
                        BortID = bortID,
                        ReturnedCardID = card_id,
                        Status = 1
                    });
                    var returne_card = _db.ReturnedCards.Where(c => c.card_id == card_id).FirstOrDefault();
                    if (returne_card != null)
                    {
                        returne_card.bort_id = bortID;
                        _db.Entry(returne_card).State = EntityState.Modified;

                    }
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {

                    return Json(0);
                }
            }
            return Json(1);
        }
        [HttpPost]
        public JsonResult DeleteCard(int id)
        {

            using (DataContext _db = new DataContext())
            {
                
                {
                    DateTime dateFrom = DateTime.Now;
                    try
                    {
                        ReturnedCardDelete returnedCardDelete = new ReturnedCardDelete(_db, id);
                    }
                    catch (Exception ex)
                    {
                       
                        return Json(0);
                    }
                }

            }
            return Json(1);
        }

    }
}
public class CashCashles
{
    public double Cash { get; set; }
    public double Cashless { get; set; }
}
public class Commisions
{
    public CashCashles ReturnedCard_commision_arr(string[] commisiontype)
    {
        CashCashles _returnedcash = new CashCashles();
        JArray amount, commision_type;
        double Cash = 0;
        double Cashless = 0;
        for (int i = 0; i < commisiontype.Length; i++)
        {
            if (commisiontype[i].Length != 0)
            {
                string arr = commisiontype[i].ToString();
                var _returned = commisiontype.GetValue(i);
                string parse_returned = arr;
                JObject parsed = JObject.Parse(parse_returned);
                commision_type = (JArray)parsed["commisionType"];
                amount = (JArray)parsed["amount"];
                for (int j = 0; j < amount.Count(); j++)
                {
                    if (Convert.ToInt32(commision_type[j]) == 2)
                    {
                        Cash += Convert.ToDouble(amount[j]);
                    }
                    if (Convert.ToInt32(commision_type[j]) == 18)
                    {
                        Cashless += Convert.ToDouble(amount[j]);
                    }

                }
            }
        }
        _returnedcash.Cash = Cash;
        _returnedcash.Cashless = Cashless;
        return _returnedcash;
    }
}
