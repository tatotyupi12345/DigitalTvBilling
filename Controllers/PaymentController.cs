using DigitalTVBilling.Filters;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using System.Linq.Expressions;
using DigitalTVBilling.ListModels;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using DigitalTVBilling.N_layer_Rent.RentSms;
using static DigitalTVBilling.Utils.SendMiniSMS;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class PaymentController : BaseController
    {
        private int pageSize = 20;
        public async Task<ActionResult> Index(int page = 1)
        {
            if (!Utils.Utils.GetPermission("PAYMENT_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                ViewBag.FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First();
                DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

                //original code 
                //string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id, d.cust_name+' '+d.lastname AS AbonentName,d.abonent_num AS AbonentNum,d.card_num AS CardNum,d.name AS PayType,d.amount AS Amount,d.tdate AS Date, d.file_attach AS FileName
                //         FROM (SELECT row_number() over(ORDER BY p.tdate DESC) AS row_num,c.name AS cust_name, c.lastname,cr.abonent_num,cr.card_num,p.id,p.amount,p.tdate,pt.name, p.file_attach FROM doc.Payments AS p
                //          INNER JOIN book.Cards AS cr ON cr.id= p.card_id
                //          INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                //          INNER JOIN book.PayTypes AS pt ON pt.id=p.pay_type_id
                //         WHERE cr.status !=4 AND p.tdate BETWEEN @date_from AND @date_to) AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

                string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id, d.cust_name+' '+d.lastname AS AbonentName,d.abonent_num AS AbonentNum,d.card_num AS CardNum,d.name AS PayType, d.UserName,d.pay_rent AS RentAmount, d.amount AS Amount,d.tdate AS Date, d.file_attach AS FileName
                         FROM (SELECT row_number() over(ORDER BY p.tdate DESC) AS row_num,c.name AS cust_name, c.lastname,cr.abonent_num,cr.card_num,p.id,p.amount,p.pay_rent,p.tdate,pt.name, p.file_attach, usr.name as UserName FROM doc.Payments AS p
                          INNER JOIN book.Cards AS cr ON cr.id= p.card_id
                          INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                          INNER JOIN book.PayTypes AS pt ON pt.id=p.pay_type_id
						  INNER JOIN book.Users AS usr ON usr.id=p.user_id
                         WHERE cr.status !=4 AND p.tdate BETWEEN @date_from AND @date_to) AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

                int count = await _db.Database.SqlQuery<int>(@"SELECT COUNT(p.id) FROM doc.Payments AS p 
                          INNER JOIN book.Cards AS cr ON cr.id= p.card_id
                          INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                          INNER JOIN book.PayTypes AS pt ON pt.id=p.pay_type_id
                         WHERE cr.status !=4 AND p.tdate BETWEEN @date_from AND @date_to",
                                                                                    new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();
                return View(await _db.Database.SqlQuery<PaymentList>(sql, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, pageSize));
            }
        }

        public PartialViewResult changePayType(int payId)
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.payments = _db.PayTypes.Select(p => p.Name).ToList();
            }
            ViewBag.PayId = payId;
            return PartialView("~/Views/Payment/changePayType.cshtml");
        }

        public int editPayType(string payType, string paymentId)
        {
            using (DataContext _db = new DataContext())
            {
                Payment payment = _db.Payments.Find(Convert.ToInt32(paymentId));
                payment.PayTypeId = _db.PayTypes.Where(p => p.Name == payType).Select(p => p.Id).FirstOrDefault();
                _db.Entry(payment).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return 0;
        }

        public PartialViewResult NewPayment(int id)
        {
            PaymentData payment = new PaymentData() { Cards = new List<int>(), PayType = 2 };
            using (DataContext _db = new DataContext())
            {
                ViewBag.PayTypes = _db.PayTypes.Where(c => c.Id > 1).Select(p => new SelectListItem() { Text = p.Name, Value = p.Id.ToString() }).ToList();
                ViewBag.Customer = "";
                if (id > 0)
                {
                    Payment pay = _db.Payments
                        .Include(c => c.Card.Customer).Where(p => p.Id == id).FirstOrDefault();
                    if (pay != null)
                    {
                        payment.TransactionId = pay.PayTransaction.ToString();
                        payment.PayType = pay.PayTypeId;
                        payment.Id = pay.Id;
                        payment.Amount = pay.Amount;
                        payment.Cards = new List<int>() { pay.CardId };
                        payment.RentAmount = pay.PayRent;
                        ViewBag.CardNames = new List<string>() { pay.Card.AbonentNum + " - " + pay.Card.Address };
                        ViewBag.Customer = pay.Card.Customer.Name + " " + pay.Card.Customer.LastName;
                    }
                }
            }
            return PartialView("~/Views/Payment/_NewPayment.cshtml", payment);
        }

        [HttpPost]
        public ActionResult NewPayment(PaymentData paydata, string fl)
        {
            if (ModelState.IsValid)
            {
                //if (fl != null && fl.ContentLength > 0)
                {
                    //if (fl.ContentType == "image/jpeg" || fl.ContentType == "application/pdf")
                    {
                        using (DataContext _db = new DataContext())
                        {

                            if (paydata.Id == 0)
                            {
                                if (paydata.PayRent == 1)
                                {
                                    Rent.RentPresentation rentPresentation = new Rent.RentPresentation();
                                    RentSMSPresentation rentSMS = new RentSMSPresentation();
                                    Rent.RentModel.ResultRent resultRent = new Rent.RentModel.ResultRent
                                    {
                                        pay_data = paydata,
                                        fromPay = false,
                                        posted_file = fl,
                                        user_id = ((User)Session["CurrentUser"]).Id,
                                        _db = _db
                                    };
                                    rentPresentation.ResultRentSavePayment(resultRent);
                                    rentSMS.SendSMS(resultRent._db, resultRent.pay_data.Cards[0]);
                                    return Redirect("/Payment");
                                }
                                else
                                {
                                    SavePayment(paydata, ((User)Session["CurrentUser"]).Id, false, fl);
                                }
                            }
                            else
                            {

                                Payment pay = _db.Payments.Include(c => c.Card).Where(p => p.Id == paydata.Id).FirstOrDefault();
                                if (pay != null)
                                {
                                    //var fileName = Path.GetFileName(fl.FileName);
                                    //fileName = Utils.Utils.GenerateFileName(pay.Card.AbonentNum, fileName.Substring(fileName.IndexOf('.')));

                                    //var _params = _db.Params.ToList();
                                    //Utils.Utils.UploadFile(fl.InputStream, _params.First(p => p.Name == "FTPHost").Value, _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, fileName);

                                    pay.FileAttach = fl;
                                    _db.Entry(pay).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return Redirect("/Payment");
        }

        [NonAction]
        public int SavePayment(PaymentData pay_data, int user_id, bool fromPay = false, string posted_file = null)
        {
            using (DataContext _db = new DataContext())
            {
                SendMiniSMS sendMiniSMS = new SendMiniSMS();
                List<Param> _params = _db.Params.ToList();
                List<Payment> payments = new List<Payment>();
                List<PayType> _payTypes = _db.PayTypes.ToList();
                List<Card> cards = null;
                MessageTemplate msg = null;
                MessageTemplate msg_geo = null;
                string onPayMsg = "";
                string onPayMsg_geo = "";
                string phone = "";
                string cardnum = "";

                try
                {
                    cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => pay_data.Cards.Contains(c.Id)).ToList();
                    if (cards.Count == 0)
                        return 5;
                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                    if (ex.InnerException != null)
                        ms = ex.InnerException.InnerException.Message;
                    ms += ms + "  Date:" + DateTime.Now + " from: System";
                    List<string> l = new List<string>();
                    l.Add(ms);
                    System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);
                }

                if (fromPay)
                {
                    pay_data.PayType = 2;
                }

                decimal coeff = pay_data.Amount / (decimal)cards.SelectMany(c => c.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * c.Discount) / 100)).Sum();
                long pay_transaction_id = 0;
                DateTime dt = DateTime.Now;

                string fileName = "";
                if (posted_file != null)
                {
                    //try
                    //{
                    //    fileName = Path.GetFileName(posted_file.FileName);
                    //    fileName = Utils.Utils.GenerateFileName(cards.Select(c => c.AbonentNum).First(), fileName.Substring(fileName.IndexOf('.')));
                    //    Utils.Utils.UploadFile(posted_file.InputStream, _params.First(p => p.Name == "FTPHost").Value, _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, fileName);
                    //}
                    //catch
                    //{
                    //    fileName = "";
                    //}
                }

                foreach (Card _card in cards)
                {
                    var paym = new Payment
                    {
                        CardId = _card.Id,
                        UserId = user_id,
                        Tdate = dt,
                        FileAttach = fileName,
                        Amount = Math.Round((decimal)_card.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * _card.Discount) / 100).Sum() * coeff, 3),
                        LogCard = _card.Customer.Name + " " + _card.Customer.LastName + " ის ბარათზე - " + _card.CardNum,
                        LogCardNum = _card.CardNum,
                        LogPayType = _payTypes.FirstOrDefault(p => p.Id == pay_data.PayType).Name,
                        PayTypeId = pay_data.PayType
                    };
                    payments.Add(paym);
                }

                string[] address = _params.First(c => c.Name == "CASAddress").Value.Split(':');
                _db.Database.CommandTimeout = 6000;
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (fromPay)
                        {
                            if (!_db.PayTransactions.Any(t => t.TransactionId == pay_data.TransactionId))
                            {
                                PayTransaction pay_tran = new PayTransaction
                                {
                                    Amount = pay_data.Amount,
                                    Tdate = dt,
                                    TransactionId = pay_data.TransactionId,
                                    PayTransactionCards = pay_data.Cards.Select(c => new PayTransactionCard { CardId = c }).ToList()
                                };
                                _db.PayTransactions.Add(pay_tran);
                                _db.SaveChanges();
                                pay_transaction_id = pay_tran.Id;
                            }
                            else
                            {
                                return 2;
                            }
                        }

                        int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                        decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        _socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            pay.PayTransaction = pay_transaction_id;
                            _db.Payments.Add(pay);
                            _db.SaveChanges();

                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                Card = c,
                                IsBudget = c.Customer.IsBudget,
                                CustomerType = c.Customer.Type,
                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                CardLogs = c.CardLogs.ToList()
                            }).FirstOrDefault();

                            bool has_active = false;
                            if (_card != null)
                            {
                                double balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);
                                if (balance >= 0 || _card.CustomerType == CustomerType.Juridical)
                                {
                                    switch (_card.Card.CardStatus)
                                    {
                                        case CardStatus.Paused:
                                            {
                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnPausePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));

                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnPausePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));
                                            }
                                            break;

                                        case CardStatus.FreeDays:
                                            {
                                                string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                if (_card.Card.FinishDate > today)
                                                {
                                                    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                                    //if (dateForFinishDate != null)
                                                    //{
                                                    //    _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                    //}

                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }


                                            }
                                            break;
                                        case CardStatus.Rent:
                                            int pay_countrent = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                            double min_pricerent = pay_countrent == 1 ? _card.SubscribAmount : _card.MinPrice;

                                            if (_card.CustomerType != CustomerType.Juridical)
                                            {
                                                min_pricerent -= (min_pricerent * (double)_card.Card.Discount / 100);
                                                {
                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    if (balance >= min_pricerent)
                                                    {
                                                        has_active = true;
                                                        _card.Card.Mode = 0;
                                                        _card.Card.CardStatus = CardStatus.Active;

                                                        _db.Entry(_card.Card).State = EntityState.Modified;

                                                        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                        _db.CardLogs.Add(_log);

                                                        //original code
                                                        //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                        //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                        DateTime dtnow = pay.Tdate;
                                                        DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                        TimeSpan diffSpan = finDate - dtnow;
                                                        int hours_to_add = 1;

                                                        if (pay.Tdate.Minute >= 30)
                                                            hours_to_add = diffSpan.Hours;
                                                        else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            hours_to_add = diffSpan.Hours + 1;
                                                        decimal amount = 0;
                                                        if (_card.SubscribAmount != 12)
                                                        {
                                                            amount = (decimal)((_card.SubscribAmount - 3) / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                        }
                                                        else
                                                        {
                                                            amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);

                                                        }
                                                        amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        amount *= hours_to_add;
                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        _db.SaveChanges();

                                                        //msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        //if (msg != null)
                                                        //    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        //msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        //if (msg_geo != null)
                                                        //    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));

                                                        if (pay_countrent == 1)
                                                        {
                                                          //  if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.Now))
                                                            {
                                                                //throw new Exception("პირველ გადახდაზე ვერ მოხერხდა ბარათის სტატუსის შეცვლა:" + _card.Card.AbonentNum);
                                                            }
                                                        }
                                                    }

                                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                    if (_card.Card.CardStatus == CardStatus.Active )
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    }
                                                    
                                                    if (_card.Card.CardStatus == CardStatus.Closed || _card.Card.CardStatus == CardStatus.Rent)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (balance >= 0)
                                                {
                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    _card.Card.CardStatus = CardStatus.Active;
                                                    _db.Entry(_card.Card).State = EntityState.Modified;

                                                    CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                    _db.CardLogs.Add(_log);

                                                    _db.SaveChanges();

                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                }
                                                else
                                                {
                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                }
                                            }
                                            break;
                                        case CardStatus.Closed:
                                            int pay_count = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                            double min_price = pay_count == 1 ? _card.SubscribAmount : _card.MinPrice;

                                            if (_card.CustomerType != CustomerType.Juridical)
                                            {
                                                min_price -= (min_price * (double)_card.Card.Discount / 100);
                                                {
                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    if (balance >= min_price)
                                                    {
                                                        has_active = true;
                                                        _card.Card.Mode = 0;
                                                        _card.Card.CardStatus = CardStatus.Active;

                                                        _db.Entry(_card.Card).State = EntityState.Modified;

                                                        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                        _db.CardLogs.Add(_log);

                                                        //original code
                                                        //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                        //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                        DateTime dtnow = pay.Tdate;
                                                        DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                        TimeSpan diffSpan = finDate - dtnow;
                                                        int hours_to_add = 1;

                                                        if (pay.Tdate.Minute >= 30)
                                                            hours_to_add = diffSpan.Hours;
                                                        else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            hours_to_add = diffSpan.Hours + 1;
                                                        decimal amount = 0;
                                                        if (_card.SubscribAmount != 12)
                                                        {
                                                            amount = (decimal)((_card.SubscribAmount - 3) / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                        }
                                                        else
                                                        {
                                                            amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);

                                                        }
                                                        amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        amount *= hours_to_add;
                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        _db.SaveChanges();

                                                        //msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        //if (msg != null)
                                                        //    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        //msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        //if (msg_geo != null)
                                                        //    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));

                                                        if (pay_count == 1)
                                                        {
                                                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.Now))
                                                            {
                                                                //throw new Exception("პირველ გადახდაზე ვერ მოხერხდა ბარათის სტატუსის შეცვლა:" + _card.Card.AbonentNum);
                                                            }
                                                        }
                                                    }

                                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                    if (_card.Card.CardStatus == CardStatus.Active)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    }

                                                    if (_card.Card.CardStatus == CardStatus.Closed)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (balance >= 0)
                                                {
                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    _card.Card.CardStatus = CardStatus.Active;
                                                    _db.Entry(_card.Card).State = EntityState.Modified;

                                                    CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                    _db.CardLogs.Add(_log);

                                                    _db.SaveChanges();

                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                }
                                                else
                                                {
                                                    if (Math.Round(balance - (double)pay.Amount, 2) >= 0)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    }
                                                    else
                                                    {
                                                        DateTime dfrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                                                        DateTime dTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
                                                        decimal _balance = Math.Round(_card.PaymentAmount - _card.ChargeAmount, 2);
                                                        decimal CardCahrge_Balance = 0;
                                                        var error = _db.CardCharges.Where(c => c.CardId == pay.CardId && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).ToList();
                                                        if (error.Count() != 0)
                                                        {
                                                            CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == pay.CardId && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).Sum();

                                                        }
                                                        decimal _blance = _balance + CardCahrge_Balance;
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(_blance, 2));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(_blance,2 ));
                                                    } 
                                                    //msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                    //if (msg != null)
                                                    //    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    //msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                    //if (msg_geo != null)
                                                    //    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                }
                                            }
                                            break;

                                        case CardStatus.Active:
                                            {
                                                if (_card.CustomerType != CustomerType.Juridical)
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), false))
                                                    {
                                                        //throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + _card.Card.CardNum);
                                                    }
                                                    //miniSMS
                                                    sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(_card.Card.CardNum), _card.Card.Id, _card.CasIds.ToArray(), _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), (int)CardStatus.Active, false, (int)StatusMiniSMS.Payment);

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }
                                                else
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.FinishDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), false))
                                                    //{
                                                    //    //throw new Exception("ბარათის პაკეტები ვერ შაიშალა:" + _card.Card.CardNum);
                                                    //}

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }
                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                //if (_card.Card.FinishDate <= today)
                                                //    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                //else
                                                //{
                                                //    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                //    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Hours);
                                                //    if (dateForFinishDate != null)
                                                //    {
                                                //        _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                //    }
                                                //    else
                                                //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                //}
                                            }
                                            break;
                                        case CardStatus.Blocked:
                                            {
                                                //if (_card.CardLogs.Count > 0 && _card.CardLogs.Last().Status != CardLogStatus.Blocked)
                                                {
                                                    min_price = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count() == 1 ? _card.SubscribAmount : _card.MinPrice;
                                                    min_price -= (min_price * (double)_card.Card.Discount / 100);

                                                    double blocked_commision = -1;
                                                    double.TryParse(_db.Params.Where(p => p.Name == "ActivateBlockedAmmount").FirstOrDefault().Value, out blocked_commision);
                                                    if (blocked_commision != -1)
                                                    {

                                                        balance = Math.Round((double)Utils.Utils.GetBalance(_db.Payments.Where(p => p.CardId == _card.Card.Id).Select(p => p.Amount).Sum(), _db.CardCharges.Where(c => c.CardId == _card.Card.Id).Select(c => c.Amount).Sum()), 2);

                                                        if (balance >= (min_price + blocked_commision))
                                                        {
                                                            _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = _card.Card.Id, Status = CardChargeStatus.Pen, Amount = (decimal)blocked_commision });
                                                            _db.SaveChanges();

                                                            has_active = true;
                                                            _card.Card.Mode = 0;
                                                            _card.Card.CardStatus = CardStatus.Active;

                                                            _db.Entry(_card.Card).State = EntityState.Modified;

                                                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                            _db.CardLogs.Add(_log);

                                                            //original code
                                                            //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                            //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                            DateTime dtnow = pay.Tdate;
                                                            DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                            TimeSpan diffSpan = finDate - dtnow;
                                                            int hours_to_add = 1;

                                                            if (pay.Tdate.Minute >= 30)
                                                                hours_to_add = diffSpan.Hours;
                                                            else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                                hours_to_add = diffSpan.Hours + 1;


                                                            decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                            amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            amount *= hours_to_add;
                                                            _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            _db.SaveChanges();

                                                            //string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                                            //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                                            //_socket.Connect();
                                                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                            {
                                                                throw new Exception();
                                                            }
                                                            //_socket.Disconnect();

                                                            Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                        }

                                                        if (_card.Card.CardStatus == CardStatus.Active)
                                                        {
                                                            msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                            if (msg != null)
                                                                onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                            msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                            if (msg_geo != null)
                                                                onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        }

                                                        if (_card.Card.CardStatus == CardStatus.Blocked)
                                                        {
                                                            msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                            if (msg != null)
                                                                onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round((min_price + blocked_commision) - (double)balance, 2));
                                                            msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                            if (msg_geo != null)
                                                                onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        }
                                                    }
                                                    //var blocked_cards = _db.Customers.Where(c => c.Cards.Any(cc => cc.Id == _card.Card.Id)).SelectMany(c => c.Cards.Where(cc => cc.CardStatus == CardStatus.Blocked)).ToList();
                                                    //foreach (var crd in blocked_cards)
                                                    //{
                                                    //    CardLog last_log = _db.CardLogs.Where(c => c.CardId == crd.Id).OrderByDescending(c => c.Date).Skip(0).Take(1).FirstOrDefault();
                                                    //    if (last_log != null)
                                                    //    {
                                                    //        crd.CardStatus = last_log.CardStatus;
                                                    //        _db.Entry(_card.Card).State = EntityState.Modified;

                                                    //        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.CloseBlock, UserId = user_id };
                                                    //        _db.CardLogs.Add(_log);
                                                    //    }
                                                    //}
                                                }
                                                break;
                                            }
                                        case CardStatus.Discontinued:
                                            {
                                                //if (_card.CardLogs.Count > 0 && _card.CardLogs.Last().Status != CardLogStatus.Blocked)
                                                {
                                                    min_price = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count() == 1 ? _card.SubscribAmount : _card.MinPrice;
                                                    min_price -= (min_price * (double)_card.Card.Discount / 100);

                                                    double blocked_commision = -1;
                                                    double.TryParse(_db.Params.Where(p => p.Name == "ActivateBlockedAmmount").FirstOrDefault().Value, out blocked_commision);
                                                    if (blocked_commision != -1)
                                                    {

                                                        balance = Math.Round((double)Utils.Utils.GetBalance(_db.Payments.Where(p => p.CardId == _card.Card.Id).Select(p => p.Amount).Sum(), _db.CardCharges.Where(c => c.CardId == _card.Card.Id).Select(c => c.Amount).Sum()), 2);

                                                        if (balance >= (min_price + blocked_commision))
                                                        {
                                                            _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = _card.Card.Id, Status = CardChargeStatus.Pen, Amount = (decimal)blocked_commision });
                                                            _db.SaveChanges();

                                                            has_active = true;
                                                            _card.Card.Mode = 0;
                                                            _card.Card.CardStatus = CardStatus.Active;

                                                            _db.Entry(_card.Card).State = EntityState.Modified;

                                                            CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                            _db.CardLogs.Add(_log);

                                                            //original code
                                                            //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                            //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                            DateTime dtnow = pay.Tdate;
                                                            DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                            TimeSpan diffSpan = finDate - dtnow;
                                                            int hours_to_add = 1;

                                                            if (pay.Tdate.Minute >= 30)
                                                                hours_to_add = diffSpan.Hours;
                                                            else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                                hours_to_add = diffSpan.Hours + 1;


                                                            decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                            amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            amount *= hours_to_add;
                                                            _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            _db.SaveChanges();

                                                            //string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                                            //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                                            //_socket.Connect();
                                                            if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                            {
                                                                throw new Exception();
                                                            }
                                                            //_socket.Disconnect();

                                                            Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                        }

                                                        if (_card.Card.CardStatus == CardStatus.Active)
                                                        {
                                                            msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                            if (msg != null)
                                                                onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                            msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                            if (msg_geo != null)
                                                                onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        }

                                                        if (_card.Card.CardStatus == CardStatus.Discontinued)
                                                        {
                                                            msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                            if (msg != null)
                                                                onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round((min_price + blocked_commision) - (double)balance, 2));
                                                            msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                            if (msg_geo != null)
                                                                onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        }
                                                    }
                                                    //var blocked_cards = _db.Customers.Where(c => c.Cards.Any(cc => cc.Id == _card.Card.Id)).SelectMany(c => c.Cards.Where(cc => cc.CardStatus == CardStatus.Blocked)).ToList();
                                                    //foreach (var crd in blocked_cards)
                                                    //{
                                                    //    CardLog last_log = _db.CardLogs.Where(c => c.CardId == crd.Id).OrderByDescending(c => c.Date).Skip(0).Take(1).FirstOrDefault();
                                                    //    if (last_log != null)
                                                    //    {
                                                    //        crd.CardStatus = last_log.CardStatus;
                                                    //        _db.Entry(_card.Card).State = EntityState.Modified;

                                                    //        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.CloseBlock, UserId = user_id };
                                                    //        _db.CardLogs.Add(_log);
                                                    //    }
                                                    //}
                                                }
                                                break;
                                            }
                                    }

                                    //original source
                                    //if (has_active)
                                    //{
                                    //    _card.Card.CasDate = DateTime.Now;
                                    //    _db.Entry(_card.Card).State = EntityState.Modified;
                                    //}

                                    //if (_card.Card.CardStatus != CardStatus.Montage)
                                    //{
                                    //    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                    //    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    //    if (_card.Card.FinishDate <= today)
                                    //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    else
                                    //    {
                                    //        TimeSpan diffSpan = _card.Card.FinishDate - today;
                                    //        DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                    //        if(dateForFinishDate != null)
                                    //        {
                                    //            _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                    //        }
                                    //        else
                                    //            Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    }
                                    //}
                                }
                                else
                                {
                                    if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                                    {
                                        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    }
                                }

                                phone = _card.Card.Customer.Phone1;
                                cardnum = _card.Card.CardNum;

                                //string phone = _db.Customers.Where(c => c.Id = _card.Card.Customer.Phone1)
                                //string message = "Tqveni TVMobile angarishi Sheivso " + Math.Round(pay.Amount, 2).ToString() + " - larit, tqveni mimdinare balansia: " + Math.Round(balance, 2).ToString();
                                if (cards.Select(s => s.Subscribtions.First().SubscriptionPackages.Any(a => a.PackageId == 304086)).FirstOrDefault())
                                {
                                   
                                }
                                else
                                {
                                    Task.Run(async () => { await Utils.Utils.sendMessage(phone, onPayMsg); }).Wait();
                                }
                               if (!_socket.SendOSDRequest(Convert.ToInt32(cardnum), onPayMsg_geo, DateTime.Now.AddHours(-4), 0))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }

                            }

                            if (has_active)
                            {
                                _card.Card.CasDate = DateTime.Now;
                                _db.Entry(_card.Card).State = EntityState.Modified;

                               if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }
                                sendMiniSMS.SaveMiniSMSData(Convert.ToInt32(_card.Card.CardNum), _card.Card.Id, _card.CasIds.ToArray(), _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), (int)-2, true, (int)StatusMiniSMS.Payment);
                            }
                            _db.SaveChanges();
                        }

                        _socket.Disconnect();
                        tran.Commit();

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        string ms = ex.Message;
                        if (ex.InnerException != null)
                            ms = ex.InnerException.InnerException.Message;
                        ms += ms + "  Date:" + DateTime.Now + " from: SYSTEM";
                        List<string> l = new List<string>();
                        l.Add(ms);
                        System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);

                        tran.Rollback();


                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        _socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                Card = c,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            }).FirstOrDefault();

                            _socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false);
                        }

                        _socket.Disconnect();
                    }
                }
                return 5;
            }
        }

        [NonAction]
        public int SavePayment(DataContext _db, PaymentData pay_data, int user_id, bool fromPay = false, string posted_file = null)
        {
            //using (DataContext _db = new DataContext())
            {
                List<Param> _params = _db.Params.ToList();
                List<Payment> payments = new List<Payment>();
                List<PayType> _payTypes = _db.PayTypes.ToList();
                List<Card> cards = null;
                MessageTemplate msg = null;
                MessageTemplate msg_geo = null;
                string onPayMsg = "";
                string onPayMsg_geo = "";
                string phone = "";
                string cardnum = "";

                try
                {
                    cards = _db.Cards.Include("Customer").Include("Subscribtions").Where(c => pay_data.Cards.Contains(c.Id)).ToList();
                    if (cards.Count == 0)
                        return 5;
                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                    if (ex.InnerException != null)
                        ms = ex.InnerException.InnerException.Message;
                    ms += ms + "  Date:" + DateTime.Now + " from: System";
                    List<string> l = new List<string>();
                    l.Add(ms);
                    System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);
                }

                if (fromPay)
                {
                    pay_data.PayType = 2;
                }

                decimal coeff = pay_data.Amount / (decimal)cards.SelectMany(c => c.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * c.Discount) / 100)).Sum();
                long pay_transaction_id = 0;
                DateTime dt = DateTime.Now;

                string fileName = "";
                if (posted_file != null)
                {
                    //try
                    //{
                    //    fileName = Path.GetFileName(posted_file.FileName);
                    //    fileName = Utils.Utils.GenerateFileName(cards.Select(c => c.AbonentNum).First(), fileName.Substring(fileName.IndexOf('.')));
                    //    Utils.Utils.UploadFile(posted_file.InputStream, _params.First(p => p.Name == "FTPHost").Value, _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, fileName);
                    //}
                    //catch
                    //{
                    //    fileName = "";
                    //}
                }

                foreach (Card _card in cards)
                {
                    var paym = new Payment
                    {
                        CardId = _card.Id,
                        UserId = user_id,
                        Tdate = dt,
                        FileAttach = fileName,
                        Amount = Math.Round((decimal)_card.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * _card.Discount) / 100).Sum() * coeff, 3),
                        LogCard = _card.Customer.Name + " " + _card.Customer.LastName + " ის ბარათზე - " + _card.CardNum,
                        LogCardNum = _card.CardNum,
                        LogPayType = _payTypes.FirstOrDefault(p => p.Id == pay_data.PayType).Name,
                        PayTypeId = pay_data.PayType
                    };
                    payments.Add(paym);
                }

                string[] address = _params.First(c => c.Name == "CASAddress").Value.Split(':');
                _db.Database.CommandTimeout = 6000;
                //using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (fromPay)
                        {
                            if (!_db.PayTransactions.Any(t => t.TransactionId == pay_data.TransactionId))
                            {
                                PayTransaction pay_tran = new PayTransaction
                                {
                                    Amount = pay_data.Amount,
                                    Tdate = dt,
                                    TransactionId = pay_data.TransactionId,
                                    PayTransactionCards = pay_data.Cards.Select(c => new PayTransactionCard { CardId = c }).ToList()
                                };
                                _db.PayTransactions.Add(pay_tran);
                                _db.SaveChanges();
                                pay_transaction_id = pay_tran.Id;
                            }
                            else
                            {
                                return 2;
                            }
                        }

                        int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                        decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        //_socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            pay.PayTransaction = pay_transaction_id;
                            _db.Payments.Add(pay);
                            _db.SaveChanges();

                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                Card = c,
                                IsBudget = c.Customer.IsBudget,
                                CustomerType = c.Customer.Type,
                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                CardLogs = c.CardLogs.ToList()
                            }).FirstOrDefault();

                            bool has_active = false;
                            if (_card != null)
                            {
                                double balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);
                                if (balance >= 0 || _card.CustomerType == CustomerType.Juridical)
                                {
                                    switch (_card.Card.CardStatus)
                                    {
                                        case CardStatus.Paused:
                                            {
                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnPausePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));

                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnPausePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));
                                            }
                                            break;

                                        case CardStatus.FreeDays:
                                            {
                                                string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                if (_card.Card.FinishDate > today)
                                                {
                                                    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                                    //if (dateForFinishDate != null)
                                                    //{
                                                    //    _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                    //}

                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }


                                            }
                                            break;

                                        case CardStatus.Closed:
                                            int pay_count = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                            double min_price = pay_count == 1 ? _card.SubscribAmount : _card.MinPrice;

                                            if (_card.CustomerType != CustomerType.Juridical)
                                            {
                                                min_price -= (min_price * (double)_card.Card.Discount / 100);
                                                {
                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    if (balance >= min_price)
                                                    {
                                                        has_active = true;
                                                        _card.Card.Mode = 0;
                                                        _card.Card.CardStatus = CardStatus.Active;

                                                        _db.Entry(_card.Card).State = EntityState.Modified;

                                                        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                        _db.CardLogs.Add(_log);

                                                        //original code
                                                        //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                        //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                        DateTime dtnow = pay.Tdate;
                                                        DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                        TimeSpan diffSpan = finDate - dtnow;
                                                        int hours_to_add = 1;

                                                        if (pay.Tdate.Minute >= 30)
                                                            hours_to_add = diffSpan.Hours;
                                                        else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            hours_to_add = diffSpan.Hours + 1;


                                                        decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                        amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        amount *= hours_to_add;
                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        _db.SaveChanges();

                                                        //msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        //if (msg != null)
                                                        //    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        //msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        //if (msg_geo != null)
                                                        //    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));

                                                        if (pay_count == 1)
                                                        {
                                                            //if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.Now))
                                                            {
                                                                //throw new Exception("პირველ გადახდაზე ვერ მოხერხდა ბარათის სტატუსის შეცვლა:" + _card.Card.AbonentNum);
                                                            }
                                                        }
                                                    }

                                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                    if (_card.Card.CardStatus == CardStatus.Active)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    }

                                                    if (_card.Card.CardStatus == CardStatus.Closed)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (balance >= 0)
                                                {
                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    _card.Card.CardStatus = CardStatus.Active;
                                                    _db.Entry(_card.Card).State = EntityState.Modified;

                                                    CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                    _db.CardLogs.Add(_log);

                                                    _db.SaveChanges();

                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                }
                                                else
                                                {
                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                }
                                            }
                                            break;

                                        case CardStatus.Active:
                                            {
                                                if (_card.CustomerType != CustomerType.Juridical)
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }
                                                else
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }

                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                //if (_card.Card.FinishDate <= today)
                                                //    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                //else
                                                //{
                                                //    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                //    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Hours);
                                                //    if (dateForFinishDate != null)
                                                //    {
                                                //        _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                //    }
                                                //    else
                                                //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                //}
                                            }
                                            break;
                                        case CardStatus.Blocked:
                                            {
                                                if (_card.CardLogs.Count > 0 && _card.CardLogs.Last().Status != CardLogStatus.Blocked)
                                                {
                                                    min_price = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count() == 1 ? _card.SubscribAmount : _card.MinPrice;
                                                    min_price -= (min_price * (double)_card.Card.Discount / 100);

                                                    double blocked_commision = -1;
                                                    double.TryParse(_db.Params.Where(p => p.Name == "ActivateBlockedAmmount").FirstOrDefault().Value, out blocked_commision);
                                                    if (blocked_commision != -1 && balance >= blocked_commision)
                                                    {
                                                        _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = _card.Card.Id, Status = CardChargeStatus.Pen, Amount = (decimal)blocked_commision });
                                                        _db.SaveChanges();
                                                        balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);

                                                        if (balance >= min_price)
                                                        {
                                                            //has_active = true;
                                                            //_card.Card.Mode = 0;
                                                            //_card.Card.CardStatus = CardStatus.Active;

                                                            //_db.Entry(_card.Card).State = EntityState.Modified;

                                                            //CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                            //_db.CardLogs.Add(_log);

                                                            ////original code
                                                            ////decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                            ////amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            ////_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            //string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                            //DateTime dtnow = pay.Tdate;
                                                            //DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                            //TimeSpan diffSpan = finDate - dtnow;
                                                            //int hours_to_add = 1;

                                                            //if (pay.Tdate.Minute >= 30)
                                                            //    hours_to_add = diffSpan.Hours;
                                                            //else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            //    hours_to_add = diffSpan.Hours + 1;


                                                            //decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                            //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            //amount *= hours_to_add;
                                                            //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            //_db.SaveChanges();

                                                            //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }

                                    //original source
                                    //if (has_active)
                                    //{
                                    //    _card.Card.CasDate = DateTime.Now;
                                    //    _db.Entry(_card.Card).State = EntityState.Modified;
                                    //}

                                    //if (_card.Card.CardStatus != CardStatus.Montage)
                                    //{
                                    //    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                    //    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    //    if (_card.Card.FinishDate <= today)
                                    //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    else
                                    //    {
                                    //        TimeSpan diffSpan = _card.Card.FinishDate - today;
                                    //        DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                    //        if(dateForFinishDate != null)
                                    //        {
                                    //            _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                    //        }
                                    //        else
                                    //            Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    }
                                    //}
                                }
                                else
                                {
                                    if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                                    {
                                        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    }
                                }

                                phone = _card.Card.Customer.Phone1;
                                cardnum = _card.Card.CardNum;

                                //string phone = _db.Customers.Where(c => c.Id = _card.Card.Customer.Phone1)
                                //string message = "Tqveni TVMobile angarishi Sheivso " + Math.Round(pay.Amount, 2).ToString() + " - larit, tqveni mimdinare balansia: " + Math.Round(balance, 2).ToString();

                                //Task.Run(async () => { await Utils.Utils.sendMessage(phone, onPayMsg); }).Wait();
                                //if (!_socket.SendOSDRequest(Convert.ToInt32(cardnum), onPayMsg_geo, DateTime.Now.AddHours(-4), 0))
                                //{
                                //    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                //}

                            }

                            if (has_active)
                            {
                                _card.Card.CasDate = DateTime.Now;
                                _db.Entry(_card.Card).State = EntityState.Modified;

                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }
                            }
                            _db.SaveChanges();
                        }

                        // _socket.Disconnect();
                        //tran.Commit();

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        string ms = ex.Message;
                        if (ex.InnerException != null)
                            ms = ex.InnerException.InnerException.Message;
                        ms += ms + "  Date:" + DateTime.Now + " from: SYSTEM";
                        List<string> l = new List<string>();
                        l.Add(ms);
                        System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);

                        //tran.Rollback();


                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        _socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                Card = c,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            }).FirstOrDefault();

                            _socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false);
                        }

                        _socket.Disconnect();
                    }
                }
                return 5;
            }
        }
        [NonAction]
        public int SavePaymentReturned(DataContext _db, PaymentData pay_data, int user_id, bool fromPay = false, string posted_file = null)
        {
            //using (DataContext _db = new DataContext())
            {
                List<Param> _params = _db.Params.ToList();
                List<Payment> payments = new List<Payment>();
                List<PayType> _payTypes = _db.PayTypes.ToList();
                List<Card> cards = null;
                MessageTemplate msg = null;
                MessageTemplate msg_geo = null;
                string onPayMsg = "";
                string onPayMsg_geo = "";
                string phone = "";
                string cardnum = "";

                try
                {
                    cards = _db.Cards.Include("Customer").Include("Subscribtions").Where(c => pay_data.Cards.Contains(c.Id)).ToList();
                    if (cards.Count == 0)
                        return 5;
                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                    if (ex.InnerException != null)
                        ms = ex.InnerException.InnerException.Message;
                    ms += ms + "  Date:" + DateTime.Now + " from: System";
                    List<string> l = new List<string>();
                    l.Add(ms);
                    System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);
                }

                if (fromPay)
                {
                    pay_data.PayType = 2;
                }

                decimal coeff = pay_data.Amount / (decimal)cards.SelectMany(c => c.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * c.Discount) / 100)).Sum();
                long pay_transaction_id = 0;
                DateTime dt = DateTime.Now;

                string fileName = "";
                if (posted_file != null)
                {
                    //try
                    //{
                    //    fileName = Path.GetFileName(posted_file.FileName);
                    //    fileName = Utils.Utils.GenerateFileName(cards.Select(c => c.AbonentNum).First(), fileName.Substring(fileName.IndexOf('.')));
                    //    Utils.Utils.UploadFile(posted_file.InputStream, _params.First(p => p.Name == "FTPHost").Value, _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, fileName);
                    //}
                    //catch
                    //{
                    //    fileName = "";
                    //}
                }

                foreach (Card _card in cards)
                {
                    var paym = new Payment
                    {
                        CardId = _card.Id,
                        UserId = user_id,
                        Tdate = dt,
                        FileAttach = fileName,
                        Amount = Math.Round((decimal)_card.Subscribtions.Where(s => s.Status).Select(s => s.Amount - (s.Amount * _card.Discount) / 100).Sum() * coeff, 3),
                        LogCard = _card.Customer.Name + " " + _card.Customer.LastName + " ის ბარათზე - " + _card.CardNum,
                        LogCardNum = _card.CardNum,
                        LogPayType = _payTypes.FirstOrDefault(p => p.Id == pay_data.PayType).Name,
                        PayTypeId = pay_data.PayType
                    };
                    payments.Add(paym);
                }

                string[] address = _params.First(c => c.Name == "CASAddress").Value.Split(':');
                _db.Database.CommandTimeout = 6000;
                //using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        if (fromPay)
                        {
                            if (!_db.PayTransactions.Any(t => t.TransactionId == pay_data.TransactionId))
                            {
                                PayTransaction pay_tran = new PayTransaction
                                {
                                    Amount = pay_data.Amount,
                                    Tdate = dt,
                                    TransactionId = pay_data.TransactionId,
                                    PayTransactionCards = pay_data.Cards.Select(c => new PayTransactionCard { CardId = c }).ToList()
                                };
                                _db.PayTransactions.Add(pay_tran);
                                _db.SaveChanges();
                                pay_transaction_id = pay_tran.Id;
                            }
                            else
                            {
                                return 2;
                            }
                        }

                        int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                        decimal jurid_limit_months = int.Parse(_params.First(c => c.Name == "JuridLimitMonths").Value);
                        //CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        //_socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            pay.PayTransaction = pay_transaction_id;
                            _db.Payments.Add(pay);
                            _db.SaveChanges();

                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                PaymentAmount = c.Payments.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                Card = c,
                                IsBudget = c.Customer.IsBudget,
                                CustomerType = c.Customer.Type,
                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                CardLogs = c.CardLogs.ToList()
                            }).FirstOrDefault();

                            bool has_active = false;
                            if (_card != null)
                            {
                                double balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);
                                if (balance >= 0 || _card.CustomerType == CustomerType.Juridical)
                                {
                                    switch (_card.Card.CardStatus)
                                    {
                                        case CardStatus.Paused:
                                            {
                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnPausePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));

                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnPausePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.PauseDate.ToString("dd/MM/yyyy"));
                                            }
                                            break;

                                        case CardStatus.FreeDays:
                                            {
                                                string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                if (_card.Card.FinishDate > today)
                                                {
                                                    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                                    //if (dateForFinishDate != null)
                                                    //{
                                                    //    _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                    //}

                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }


                                            }
                                            break;

                                        case CardStatus.Closed:
                                            int pay_count = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count();
                                            double min_price = pay_count == 1 ? _card.SubscribAmount : _card.MinPrice;

                                            if (_card.CustomerType != CustomerType.Juridical)
                                            {
                                                min_price -= (min_price * (double)_card.Card.Discount / 100);
                                                {
                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    if (balance >= min_price)
                                                    {
                                                        has_active = true;
                                                        _card.Card.Mode = 0;
                                                        _card.Card.CardStatus = CardStatus.Active;

                                                        _db.Entry(_card.Card).State = EntityState.Modified;

                                                        CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                        _db.CardLogs.Add(_log);

                                                        //original code
                                                        //decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                        //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                        DateTime dtnow = pay.Tdate;
                                                        DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                        TimeSpan diffSpan = finDate - dtnow;
                                                        int hours_to_add = 1;

                                                        if (pay.Tdate.Minute >= 30)
                                                            hours_to_add = diffSpan.Hours;
                                                        else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            hours_to_add = diffSpan.Hours + 1;


                                                        decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                        amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                        amount *= hours_to_add;
                                                        _db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                        _db.SaveChanges();

                                                        //msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        //if (msg != null)
                                                        //    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        //msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        //if (msg_geo != null)
                                                        //    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));

                                                        if (pay_count == 1)
                                                        {
                                                            //if (!_socket.SendCardStatus(Convert.ToInt32(_card.Card.CardNum), true, DateTime.Now))
                                                            {
                                                                //throw new Exception("პირველ გადახდაზე ვერ მოხერხდა ბარათის სტატუსის შეცვლა:" + _card.Card.AbonentNum);
                                                            }
                                                        }
                                                    }

                                                    //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                    if (_card.Card.CardStatus == CardStatus.Active)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    }

                                                    if (_card.Card.CardStatus == CardStatus.Closed)
                                                    {
                                                        msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                        if (msg != null)
                                                            onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                        msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                        if (msg_geo != null)
                                                            onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (balance >= 0)
                                                {
                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                    _card.Card.CardStatus = CardStatus.Active;
                                                    _db.Entry(_card.Card).State = EntityState.Modified;

                                                    CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                    _db.CardLogs.Add(_log);

                                                    _db.SaveChanges();

                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                }
                                                else
                                                {
                                                    msg = _db.MessageTemplates.Where(m => m.Name == "OnLessPay").FirstOrDefault();
                                                    if (msg != null)
                                                        onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                    msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnLessPay_GEO").FirstOrDefault();
                                                    if (msg_geo != null)
                                                        onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), Math.Round(balance - (double)pay.Amount, 2));
                                                }
                                            }
                                            break;

                                        case CardStatus.Active:
                                            {
                                                if (_card.CustomerType != CustomerType.Juridical)
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                   // Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }
                                                else
                                                {
                                                    has_active = true;
                                                    if (_card.Card.Mode != 0)
                                                    {
                                                        _card.Card.Mode = 0;
                                                        _db.Entry(_card.Card).State = EntityState.Modified;
                                                    }

                                                    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);

                                                    Utils.Utils.SetJuridFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                }

                                                msg = _db.MessageTemplates.Where(m => m.Name == "OnActivePay").FirstOrDefault();
                                                if (msg != null)
                                                    onPayMsg = String.Format(msg.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                msg_geo = _db.MessageTemplates.Where(m => m.Name == "OnActivePay_GEO").FirstOrDefault();
                                                if (msg_geo != null)
                                                    onPayMsg_geo = String.Format(msg_geo.Desc, _card.Card.AbonentNum, Math.Round(pay.Amount, 2), _card.Card.FinishDate.ToString("dd/MM/yyyy"));
                                                //if (_card.Card.FinishDate <= today)
                                                //    Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                //else
                                                //{
                                                //    TimeSpan diffSpan = _card.Card.FinishDate - today;

                                                //    DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Hours);
                                                //    if (dateForFinishDate != null)
                                                //    {
                                                //        _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                                //    }
                                                //    else
                                                //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);

                                                //}
                                            }
                                            break;
                                        case CardStatus.Blocked:
                                            {
                                                if (_card.CardLogs.Count > 0 && _card.CardLogs.Last().Status != CardLogStatus.Blocked)
                                                {
                                                    min_price = _db.Payments.Where(p => p.CardId == _card.Card.Id).Count() == 1 ? _card.SubscribAmount : _card.MinPrice;
                                                    min_price -= (min_price * (double)_card.Card.Discount / 100);

                                                    double blocked_commision = -1;
                                                    double.TryParse(_db.Params.Where(p => p.Name == "ActivateBlockedAmmount").FirstOrDefault().Value, out blocked_commision);
                                                    if (blocked_commision != -1 && balance >= blocked_commision)
                                                    {
                                                        _db.CardCharges.Add(new CardCharge { Tdate = DateTime.Now, CardId = _card.Card.Id, Status = CardChargeStatus.Pen, Amount = (decimal)blocked_commision });
                                                        _db.SaveChanges();
                                                        balance = Math.Round((double)Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount), 2);

                                                        if (balance >= min_price)
                                                        {
                                                            //has_active = true;
                                                            //_card.Card.Mode = 0;
                                                            //_card.Card.CardStatus = CardStatus.Active;

                                                            //_db.Entry(_card.Card).State = EntityState.Modified;

                                                            //CardLog _log = new CardLog() { CardId = _card.Card.Id, Date = dt, Status = CardLogStatus.Open, UserId = user_id };
                                                            //_db.CardLogs.Add(_log);

                                                            ////original code
                                                            ////decimal amount = (decimal)(_card.SubscribAmount / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                                            ////amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            ////_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            //string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                                                            //DateTime dtnow = pay.Tdate;
                                                            //DateTime finDate = Utils.Utils.GenerateFinishDate(charge_time);
                                                            //TimeSpan diffSpan = finDate - dtnow;
                                                            //int hours_to_add = 1;

                                                            //if (pay.Tdate.Minute >= 30)
                                                            //    hours_to_add = diffSpan.Hours;
                                                            //else if (pay.Tdate.Minute < 30 && pay.Tdate.Minute >= 0)
                                                            //    hours_to_add = diffSpan.Hours + 1;


                                                            //decimal amount = (decimal)(_card.SubscribAmount / service_days/*DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)*/ / Utils.Utils.divide_card_charge_interval / 24);
                                                            //amount -= (amount * (decimal)_card.Card.Discount / 100);
                                                            //amount *= hours_to_add;
                                                            //_db.CardCharges.Add(new CardCharge() { CardId = _card.Card.Id, Amount = amount, Tdate = pay.Tdate.AddSeconds(1), Status = CardChargeStatus.PreChange });

                                                            //_db.SaveChanges();

                                                            //Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }

                                    //original source
                                    //if (has_active)
                                    //{
                                    //    _card.Card.CasDate = DateTime.Now;
                                    //    _db.Entry(_card.Card).State = EntityState.Modified;
                                    //}

                                    //if (_card.Card.CardStatus != CardStatus.Montage)
                                    //{
                                    //    string[] charge_vals = _params.First(c => c.Name == "CardCharge").Value.Split(':');
                                    //    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge_vals[0]), int.Parse(charge_vals[1]), 0);
                                    //    if (_card.Card.FinishDate <= today)
                                    //        Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    else
                                    //    {
                                    //        TimeSpan diffSpan = _card.Card.FinishDate - today;
                                    //        DateTime? dateForFinishDate = Utils.Utils.getFinishDate(_db, jurid_limit_months, _card.Card.Id, diffSpan.Days);
                                    //        if(dateForFinishDate != null)
                                    //        {
                                    //            _card.Card.FinishDate = (DateTime)dateForFinishDate;
                                    //        }
                                    //        else
                                    //            Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    //    }
                                    //}
                                }
                                else
                                {
                                    if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                                    {
                                       // Utils.Utils.SetFinishDate(_db, jurid_limit_months, _card.Card.Id);
                                    }
                                }

                                phone = _card.Card.Customer.Phone1;
                                cardnum = _card.Card.CardNum;

                                //string phone = _db.Customers.Where(c => c.Id = _card.Card.Customer.Phone1)
                                //string message = "Tqveni TVMobile angarishi Sheivso " + Math.Round(pay.Amount, 2).ToString() + " - larit, tqveni mimdinare balansia: " + Math.Round(balance, 2).ToString();

                                //Task.Run(async () => { await Utils.Utils.sendMessage(phone, onPayMsg); }).Wait();
                                //if (!_socket.SendOSDRequest(Convert.ToInt32(cardnum), onPayMsg_geo, DateTime.Now.AddHours(-4), 0))
                                //{
                                //    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                //}

                            }

                            if (has_active)
                            {
                                _card.Card.CasDate = DateTime.Now;
                                _db.Entry(_card.Card).State = EntityState.Modified;

                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), _card.Card.CasDate.AddHours(-4), _card.Card.FinishDate.AddHours(-4), true))
                                //if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), true))
                                {
                                    //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
                                }
                            }
                            _db.SaveChanges();
                        }

                        // _socket.Disconnect();
                        //tran.Commit();

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        string ms = ex.Message;
                        if (ex.InnerException != null)
                            ms = ex.InnerException.InnerException.Message;
                        ms += ms + "  Date:" + DateTime.Now + " from: SYSTEM";
                        List<string> l = new List<string>();
                        l.Add(ms);
                        System.IO.File.AppendAllLines(@"C:\DigitalTV\paylog.txt", l);

                        //tran.Rollback();


                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                        _socket.Connect();

                        foreach (Payment pay in payments)
                        {
                            CardDetailData _card = _db.Cards.Where(c => c.Id == pay.CardId).Select(c => new CardDetailData
                            {
                                Card = c,
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId),
                            }).FirstOrDefault();

                            _socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), _card.CasIds.ToArray(), DateTime.SpecifyKind(_card.Card.CasDate, DateTimeKind.Utc), false);
                        }

                        _socket.Disconnect();
                    }
                }
                return 5;
            }
        }
        [HttpPost]
        public JsonResult GetReturnMoney(int id)
        {
            using (DataContext _db = new DataContext())
            {
                Payment pay = _db.Payments.Include(c => c.Card).Where(p => p.Id == id).FirstOrDefault();
                if (pay != null)
                {
                    //ბალანსი ჩარიცხვამდე
                    decimal balance = (_db.Payments.Where(p => p.CardId == pay.CardId).Where(p => p.Tdate < pay.Tdate).Sum(p => (decimal?)p.Amount) ?? 0) -
                        (_db.CardCharges.Where(p => p.CardId == pay.CardId).Where(p => p.Tdate < pay.Tdate).Sum(p => (decimal?)p.Amount) ?? 0);

                    //შეცდომით ჩარიცხული თანხა
                    decimal amount = pay.Amount;

                    //შეცდომით ჩარიცხული თარიღიდან დარიცხვები
                    decimal charge_amount = _db.CardCharges.Where(p => p.CardId == pay.CardId).Where(p => p.Tdate >= pay.Tdate).Sum(p => (decimal?)p.Amount) ?? 0;

                    //შეცდომით ჩარიცხული თარიღიდან გადახდები
                    decimal after_payment_amount = _db.Payments.Where(p => p.CardId == pay.CardId).Where(p => p.Tdate > pay.Tdate).Sum(p => (decimal?)p.Amount) ?? 0;

                    //შეცდომით თარიღიდან, პირველი სწორი გადახდიდან დარიცხვები 
                    //პირველი ჩარიცხული თარიღი
                    DateTime dt_to = DateTime.Now;
                    var first_true_payment = _db.Payments.Where(c => c.CardId == pay.CardId).Where(p => p.Tdate > pay.Tdate).FirstOrDefault();
                    if (first_true_payment != null)
                        dt_to = first_true_payment.Tdate;
                    decimal true_charges_amount = _db.CardCharges.Where(p => p.CardId == pay.CardId).Where(p => p.Tdate > dt_to).Sum(p => (decimal?)p.Amount) ?? 0;

                    decimal false_res = balance + amount - charge_amount;
                    decimal true_res = after_payment_amount - true_charges_amount;

                    decimal res = false_res + true_res;

                    decimal return_amount = res > 0 ? false_res : 0;

                    return Json(return_amount);
                }
                return Json(0);
            }
        }

        [HttpPost]
        public async Task<JsonResult> FilterPayments(string letter, string column, int page, string info)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);


            string where = column + " LIKE N'" + letter + "%'";
            if (column == "p.pay_type_id")
                where = column + "=" + letter;
            where = where.Replace("+", "+' '+");

            //original code
            //string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id, d.cust_name+' '+d.lastname AS AbonentName,d.abonent_num AS AbonentNum,d.card_num AS CardNum,d.name AS PayType,d.amount AS Amount,d.tdate AS Date, d.file_attach AS FileName
            //             FROM (SELECT row_number() over(ORDER BY p.tdate DESC) AS row_num,c.name AS cust_name, c.lastname,cr.abonent_num,cr.card_num,p.id,p.amount,p.tdate,pt.name, p.file_attach FROM doc.Payments AS p
            //              INNER JOIN book.Cards AS cr ON cr.id= p.card_id
            //              INNER JOIN book.Customers AS c ON c.id=cr.customer_id
            //              INNER JOIN book.PayTypes AS pt ON pt.id=p.pay_type_id
            //             WHERE  p.tdate BETWEEN @date_from AND @date_to AND " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

            string sql = @"SELECT TOP(" + pageSize + @") d.id AS Id, d.cust_name+' '+d.lastname AS AbonentName,d.abonent_num AS AbonentNum,d.card_num AS CardNum,d.name AS PayType, d.UserName, d.amount AS Amount,d.tdate AS Date, d.file_attach AS FileName
                         FROM (SELECT row_number() over(ORDER BY p.tdate DESC) AS row_num,c.name AS cust_name, c.lastname,cr.abonent_num,cr.card_num,p.id,p.amount,p.tdate,pt.name, p.file_attach, usr.name as UserName FROM doc.Payments AS p
                          INNER JOIN book.Cards AS cr ON cr.id= p.card_id
                          INNER JOIN book.Customers AS c ON c.id=cr.customer_id
                          INNER JOIN book.PayTypes AS pt ON pt.id=p.pay_type_id
						  INNER JOIN book.Users AS usr ON usr.id=p.user_id
                         WHERE  p.tdate BETWEEN @date_from AND @date_to AND p.file_attach LIKE N'"+info+"%' AND " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);

            System.Web.Mvc.HtmlHelper helper = new System.Web.Mvc.HtmlHelper(
                             new ViewContext(), new ViewPage());

            using (DataContext _db = new DataContext())
            {
                int count = await _db.Database.SqlQuery<int>(@"SELECT COUNT(p.id) FROM doc.Payments AS p
                      INNER JOIN book.Cards AS cr ON cr.id= p.card_id
                      INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE p.tdate BETWEEN @date_from AND @date_to AND " + where,
                                                                                    new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).FirstOrDefaultAsync();
                var findList = await _db.Database.SqlQuery<PaymentList>(sql, new SqlParameter("date_from", dateFrom), new SqlParameter("date_to", dateTo)).ToRawPagedListAsync(count, page, pageSize);
                return Json(new
                {
                    Payments = findList,
                    Paging = PagedList.Mvc.HtmlHelper.PagedListPager(helper, findList, p => p.ToString(), PagedListRenderOptions.PageNumbersOnly).ToHtmlString(),
                    FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First()
                });
            }
        }

        public async Task<ActionResult> CanceledPayments(int page = 1)
        {
            DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {
                return View(await _db.CanceledPayments.AsNoTracking().Where(p => p.Tdate >= dateFrom && p.Tdate <= dateTo).Select(m => new CanceledPaymentList
                {
                    Id = m.Id,
                    Date = m.Tdate,
                    Amount = m.Amount,
                    Code = m.Card.Customer.Code,
                    CustomerName = m.Card.Customer.Name + " " + m.Card.Customer.LastName,
                    FromCard = m.Card.AbonentNum
                }).OrderByDescending(c => c.Date).ToPagedListAsync(page, 30));
            }
        }

        public PartialViewResult CancelPayment(int card_id, decimal amount)
        {
            CancelPayment pay = new DigitalTVBilling.CancelPayment
            {
                Amount = amount,
                CardId = card_id,
                Cards = new List<int>(),
                Type = 0
            };

            return PartialView("~/Views/Payment/_CancelPayment.cshtml", pay);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CancelPayment(CancelPayment cancel, HttpPostedFileBase fl)
        {
            if(ModelState.IsValid)
            {
                if (cancel.Amount == 0)
                    return Json(0);
                if (fl != null && fl.ContentLength > 0)
                {
                    if (fl.ContentType == "image/jpeg" || fl.ContentType == "application/pdf")
                    {
                        using (DataContext _db = new DataContext())
                        {
                            using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                            {
                                try
                                {
                                    DateTime _date = DateTime.Now;
                                    int user_id = ((User)Session["CurrentUser"]).Id;
                                    string desc = "{0} ბარათიდან - " + _db.Cards.Where(c => c.Id == cancel.CardId).Select(c => c.AbonentNum).FirstOrDefault() + " {1}";

                                    PaymentData paydata = null;

                                    switch (cancel.Type)
                                    {
                                        case 1:
                                            desc = String.Format(desc, "გატანა", "");
                                            paydata = new PaymentData
                                            {
                                                PayType = 4,
                                                Amount = -cancel.Amount,
                                                Cards = new List<int>() { cancel.CardId },
                                            };
                                            break;
                                        case 2:
                                            desc = String.Format(desc, "ძველ ბილინგში გადატანა", "");
                                            paydata = new PaymentData
                                            {
                                                PayType = 5,
                                                Amount = -cancel.Amount,
                                                Cards = new List<int>() { cancel.CardId },
                                            };
                                            break;
                                        case 3:
                                            desc = String.Format(desc, "აბონენტის ბარათზე", "გადატანა ბარათებზე -" + String.Join(",", _db.Cards.Where(c => cancel.Cards.Contains(c.Id)).Select(c => c.AbonentNum).ToList()));
                                            //to
                                            paydata = new PaymentData
                                            {
                                                PayType = 6,
                                                Amount = cancel.Amount,
                                                Cards = cancel.Cards
                                            };
                                            SavePayment(paydata, user_id, false);

                                            paydata = new PaymentData
                                            {
                                                PayType = 6,
                                                Amount = -cancel.Amount,
                                                Cards = new List<int>() { cancel.CardId },
                                            };
                                            break;
                                        case 4:
                                            desc = String.Format(desc, "კომპანიის ანგარიშზე", "გადატანა");
                                            CanceledPayment c_p = new CanceledPayment
                                            {
                                                CardId = cancel.CardId,
                                                Amount = cancel.Amount,
                                                Tdate = _date
                                            };
                                            _db.CanceledPayments.Add(c_p);
                                            _db.SaveChanges();

                                            paydata = new PaymentData
                                            {
                                                PayType = 7,
                                                Amount = -cancel.Amount,
                                                Cards = new List<int>() { cancel.CardId },
                                            };
                                            break;
                                    }

                                    //if (SavePayment(paydata, user_id, false, fl) != 1)
                                    if (SavePayment(paydata, user_id, false, null) != 1)
                                    {
                                        tran.Rollback();
                                        return Json(0);
                                    }

                                    this.AddLoging(_db,
                                                     LogType.Card,
                                                     LogMode.CardDeal,
                                                     user_id,
                                                     cancel.CardId,
                                                     desc,
                                                     new List<LoggingData>()
                                                  );

                                    _db.SaveChanges();

                                    tran.Commit();
                                }
                                catch
                                {
                                    tran.Rollback();
                                }
                            }
                        }
                    }
                }
            }

            return Redirect("/Payment");
        }

        [HttpPost]
        public JsonResult getPayTypes()
        {
            using (DataContext _db = new DataContext())
            {
                var pay = _db.PayTypes.ToList();
                return Json(pay, JsonRequestBehavior.DenyGet);
            }

            return null;
        }
    }
}