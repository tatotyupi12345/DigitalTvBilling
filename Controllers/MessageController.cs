using DigitalTVBilling.Filters;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DigitalTVBilling.ListModels;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Globalization;

namespace DigitalTVBilling.Controllers
{
    public class MessageFilterBy
    {
        public string where { get; set; }
        public string val { get; set; }
    }

    [ValidateUserFilter]
    public class MessageController : BaseController
    {
        public async Task<ActionResult> Index(int page = 1)
        {
            if (!Utils.Utils.GetPermission("MESSAGE_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                DateTime dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                DateTime dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

                ViewBag.SelectByData = Newtonsoft.Json.JsonConvert.SerializeObject(_db.Customers.Select(c=>new {}));

                return View(await _db.Messages.AsNoTracking().Where(p => p.Date >= dateFrom && p.Date <= dateTo).Select(m => new MessagesList
                {
                    Id = m.Id,
                    Date = m.Date,
                    MessageText = m.MessageText,
                    Type = m.MessageType
                }).OrderByDescending(c => c.Date).ToPagedListAsync(page, 30));
            }
        }

        public JsonResult GetDetails(int id)
        {
            using (DataContext _db = new DataContext())
            {
                return Json(_db.Messages.Where(m => m.Id == id).Select(m => new { text = m.MessageText, abonents = m.MessageAbonents.Select(a => a.Card.Customer.Name + " " + a.Card.Customer.LastName + " - " + a.Card.AbonentNum).ToList() }).FirstOrDefault());
            }
        }

        [HttpPost]
        public async Task<JsonResult> FindAbonents(string type, string abonent, int status, int? tower, int? receiver, string abonent_num,
            MessageFilterBy finish_date, MessageFilterBy pause_date, MessageFilterBy credit_date, MessageFilterBy balance, MessageFilterBy discount, MessageFilterBy service, MessageFilterBy status2,
            int abonent_type)
        {
            using (DataContext _db = new DataContext())
            {
                string where = type == "" ? "" : " AND " + type + "=N'" + abonent + "'";
                where = where.Replace("+", "+' '+");
                where += status == -1 ? "" : (status == 6 ? " AND cr.mode=1 AND cr.status=0" : " AND cr.status=" + status);
                where += !tower.HasValue ? "" : " AND cr.tower_id=" + tower.Value;
                where += !receiver.HasValue ? "" : " AND cr.receiver_id=" + receiver.Value;
                where += abonent_type == -1 ? "" : " AND c.type=" + abonent_type;
                where += abonent_num == "" ? "" : " AND cr.abonent_num+cr.card_num LIKE '%" + abonent_num + "%'";

                string[] charge = _db.Params.Where(p => p.Name == "CardCharge").Select(c => c.Value).First().Split(':');
                string today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(charge[0]), int.Parse(charge[1]), 0).ToString("yyyy-MM-dd HH:mm") + ":0.000";
                if (!string.IsNullOrEmpty(finish_date.val))
                {
                    where += " AND DATEDIFF(day,'" + today + "',cr.finish_date)" + finish_date.where + finish_date.val;
                }
                if (!string.IsNullOrEmpty(pause_date.val))
                {
                    string p_date = "CAST(CAST(DATEPART(YEAR,cr.pause_date) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,cr.pause_date) as VARCHAR(2))+'-'+CAST(DATEPART(DAY,cr.pause_date) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME)";
                    where += " AND DATEDIFF(day,'" + today + "'," + p_date + ")" + pause_date.where + pause_date.val;
                }
                if (!string.IsNullOrEmpty(balance.val))
                {
                    where += " AND ((SELECT ISNULL(SUM(amount),0) FROM doc.Payments WHERE card_id=cr.id) - (SELECT ISNULL(SUM(amount),0) FROM doc.CardCharges WHERE card_id=cr.id)) " + balance.where + balance.val;
                }
                if (!string.IsNullOrEmpty(credit_date.val))
                {
                    where += " AND cr.mode=1 AND DATEDIFF(day,'" + today + "',cr.finish_date)" + credit_date.where + credit_date.val;
                }
                if (!string.IsNullOrEmpty(discount.val))
                {
                    where += " AND cr.discount" + discount.where + discount.val;
                }
                if (!string.IsNullOrEmpty(service.val))
                {
                    string s_date = "(SELECT TOP(1) CAST(CAST(DATEPART(YEAR,tdate) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,tdate) as VARCHAR(2))+'-'+CAST(DATEPART(DAY, tdate) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME) FROM doc.CardServices WHERE card_id=cr.id AND is_active=1)";
                    where += " AND (CASE WHEN DATEDIFF(day, cr.finish_date, '" + today + "') < DATEDIFF(day, " + s_date + ", '" + today + "') THEN DATEDIFF(day, cr.finish_date, '" + today + "') ELSE DATEDIFF(day, " + s_date + ", '" + today + "') END)" + service.where + service.val;
                }
                if (!string.IsNullOrEmpty(status2.val))
                {
                    string res = "1=1";
                    switch (status)
                    {
                        case 0:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=0 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 1:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=1 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 2:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=2 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 3:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=3 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 5:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=6 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                    }

                    where += " AND " + res;
                }

                if (where == "")
                    return Json(new List<IdName>());

                string sql = @"SELECT cr.id AS Id, c.name+' '+c.lastname +' - ' + c.code + ' - ' + cr.abonent_num AS Name FROM book.Cards AS cr 
                                INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status!=4 " + where;

                return Json(await _db.Database.SqlQuery<IdName>(sql).ToListAsync());
            }
        }


        [HttpPost]
        public JsonResult SaveAutoTemplate(string type, string abonent, int status, int? tower, int? receiver, string abonent_num,
            MessageFilterBy finish_date, MessageFilterBy pause_date, MessageFilterBy credit_date, MessageFilterBy balance, MessageFilterBy discount,
            MessageFilterBy service, MessageFilterBy status2, int abonent_type, string text, string types, bool is_disposable, string template_name)
        {
            string where = type == "" ? "" : " AND " + type + "=N'" + abonent + "'";
            where = where.Replace("+", "+' '+");
            where += status == -1 ? "" : " AND cr.status=" + status;
            where += !tower.HasValue ? "" : " AND cr.tower_id=" + tower.Value;
            where += abonent_type == -1 ? "" : " AND c.type=" + abonent_type;
            where += !receiver.HasValue ? "" : " AND cr.receiver_id=" + receiver.Value;
            where += abonent_num == "" ? "" : " AND cr.abonent_num+cr.card_num LIKE '%" + abonent_num + "%'";

            using (DataContext _db = new DataContext())
            {
                string[] charge = _db.Params.Where(p => p.Name == "CardCharge").Select(c => c.Value).First().Split(':');
                string today = "Convert(varchar(10), getdate(), 126)+' " + charge[0].PadLeft(2, '0') + ":" + charge[1].PadLeft(2, '0') + ":0.000'";
                if (!string.IsNullOrEmpty(finish_date.val))
                {
                    where += " AND DATEDIFF(day," + today + ",cr.finish_date)" + finish_date.where + finish_date.val;
                }
                if (!string.IsNullOrEmpty(pause_date.val))
                {
                    string p_date = "CAST(CAST(DATEPART(YEAR,cr.pause_date) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,cr.pause_date) as VARCHAR(2))+'-'+CAST(DATEPART(DAY,cr.pause_date) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME)";
                    where += " AND DATEDIFF(day," + today + "," + p_date + ")" + pause_date.where + pause_date.val;
                }
                if (!string.IsNullOrEmpty(balance.val))
                {
                    where += " AND ((SELECT ISNULL(SUM(amount),0) FROM doc.Payments WHERE card_id=cr.id) - (SELECT ISNULL(SUM(amount),0) FROM doc.CardCharges WHERE card_id=cr.id)) " + balance.where + balance.val;
                }
                if (!string.IsNullOrEmpty(credit_date.val))
                {
                    where += " AND cr.mode=1 AND DATEDIFF(day," + today + ",cr.finish_date)" + credit_date.where + credit_date.val;
                }
                if (!string.IsNullOrEmpty(discount.val))
                {
                    where += " AND cr.discount" + discount.where + discount.val;
                }
                if (!string.IsNullOrEmpty(service.val))
                {
                    string s_date = "(SELECT TOP(1) CAST(CAST(DATEPART(YEAR,tdate) as VARCHAR(4))+'-'+CAST(DATEPART(MONTH,tdate) as VARCHAR(2))+'-'+CAST(DATEPART(DAY, tdate) as VARCHAR(2))+' " + charge[0] + ":" + charge[1] + ":00.000' as DATETIME) FROM doc.CardServices WHERE card_id=cr.id AND is_active=1)";
                    where += " AND (CASE WHEN DATEDIFF(day, cr.finish_date, " + today + ") < DATEDIFF(day, " + s_date + "," + today + ") THEN DATEDIFF(day, cr.finish_date, " + today + ") ELSE DATEDIFF(day, " + s_date + ", " + today + ") END)" + service.where + service.val;
                }
                if (!string.IsNullOrEmpty(status2.val))
                {
                    string res = "1=1";
                    switch (status)
                    {
                        case 0:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=0 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 1:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=1 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 2:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=2 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 3:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=3 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                        case 5:
                            res = "DATEDIFF(day,(SELECT TOP(1) close_tdate FROM doc.CardLogs WHERE status=6 AND card_id=cr.id ORDER BY id DESC),'" + today + "')" + status2.where + status2.val;
                            break;
                    }

                    where += " AND " + res;
                }

                if (where == "")
                    return Json(0);

                try
                {
                    _db.AutoMessageTemplates.AddRange(types.Split(',').Select(c => new AutoMessageTemplate { MessageText = text, MessageType = (MessageType)int.Parse(c), Query = where, IsDisposable = is_disposable, Name = template_name }));
                    _db.SaveChanges();
                }
                catch
                {
                    return Json(0);
                }
            }

            return Json(1);
        }

        public PartialViewResult NewMessage()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Towers = _db.Towers.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
                ViewBag.Receivers = _db.Receivers.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();

                return PartialView("~/Views/Message/_NewMessage.cshtml", new Message() { Templates = _db.MessageTemplates.ToList() });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetDataSelectBy(string type, string query)
        {
            using (DataContext _db = new DataContext())
            {
                switch (type)
                {
                    case "city":
                        return Json(await _db.Customers.Where(c => c.City.StartsWith(query)).Select(c => c.City).Distinct().ToListAsync());
                    case "village":
                        return Json(await _db.Customers.Where(c => c.Village.StartsWith(query)).Select(c => c.Village).Distinct().ToListAsync());
                    case "region":
                        return Json(await _db.Customers.Where(c => c.Region.StartsWith(query)).Select(c => c.Region).Distinct().ToListAsync());
                    case "name":
                        return Json(await _db.Customers.Where(c => (c.Name + " " + c.LastName).StartsWith(query)).Select(c => (c.Name + " " + c.LastName)).ToListAsync());
                    default:
                        return Json(new List<string>());
                }
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewMessage(Message message)
        {
            if (ModelState.IsValid)
            {
                using (DataContext _db = new DataContext())
                {
                    _db.Database.CommandTimeout = 6000;
                    using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        try
                        {
                            int user_id = ((User)Session["CurrentUser"]).Id;

                            message.UserId = user_id;
                            message.Date = DateTime.Now;
                            message.MessageAbonents = message.AbonentIds.Select(a => new MessageAbonent { CardId = a }).ToList();
                            _db.Messages.Add(message);
                            _db.SaveChanges();
                
                            List<string> abonents = _db.Customers.Where(c=>message.AbonentIds.Contains(c.Id)).Select(c=>c.Name + " " + c.LastName).ToList();
                            this.AddLoging(_db,
                                    LogType.Message,
                                    LogMode.Add,
                                    user_id,
                                    message.Id,
                                    string.IsNullOrEmpty(message.TemplateType) ? "ერთჯერადი" : message.TemplateType,
                                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(message.Logging).Where(c => c.field != null || c.field != "").ToList()
                              );
                            _db.SaveChanges();

                            int[] ids = message.AbonentIds.Distinct().ToArray();
                            List<Card> cards = _db.Cards.Include(c => c.Customer).Include(c => c.Tower).Include(c => c.Receiver)
                                    .Where(c=>c.CardStatus != CardStatus.Canceled)
                                    .Where(c => ids.Contains(c.Id))
                                    .Select(c => c).ToList();

                            List<Param> Params = _db.Params.ToList();

                            int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                            string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                            string username = Params.First(p => p.Name == "SMSPassword").Value;
                            string password = Params.First(p => p.Name == "SMSUsername").Value;
                            string[] types = message.MessageType.Split(',');

                            if (types.Contains("0") || types.Contains("1"))
                            {
                                CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                _socket.Connect();

                                foreach (Card card in cards)
                                {
                                    message.MessageText = Utils.Utils.ReplaceMessageTags(message.MessageText, card, _db);
                                    foreach (string type in types)
                                    {
                                        switch (type)
                                        {
                                            case "0": //osd
                                                {
                                                    if (!_socket.SendOSDRequest(int.Parse(card.CardNum), message.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                                    {
                                                        _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                        {
                                                            CardId = card.Id,
                                                            MessageId = message.Id,
                                                            MessageType = MessageType.OSD,
                                                        });
                                                    }
                                                }
                                                break;
                                            case "1":
                                                {
                                                    if (!_socket.SendEmailRequest(int.Parse(card.CardNum), message.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                    {
                                                        _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                        {
                                                            CardId = card.Id,
                                                            MessageId = message.Id,
                                                            MessageType = MessageType.Email,
                                                        });
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                _db.SaveChanges();
                                _socket.Disconnect();
                            }

                            if (types.Contains("2")) //sms
                            {
                               Utils.Utils.OnSendSMS(cards, message.MessageText, username, password, _db, message.Id);
                            }

                            tran.Commit();
                            return Json("1");
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json("0");
        }

        [HttpPost]
        public PartialViewResult GetAutoTemplates()
        {
            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/Message/_AutoMessages.cshtml", _db.AutoMessageTemplates.ToList());
            }
        }

        [HttpPost]
        public JsonResult DeleteAutoTemplate(int id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    AutoMessageTemplate _tpl = _db.AutoMessageTemplates.Where(c => c.Id == id).FirstOrDefault();
                    if(_tpl != null)
                    {
                        _db.AutoMessageTemplates.Remove(_tpl);
                        _db.Entry(_tpl).State = EntityState.Deleted;
                        _db.SaveChanges();

                        return Json(1);
                    }
                }
                catch
                {
                    return Json(0);
                }
            }
            return Json(0);
        }

        [HttpPost]
        public PartialViewResult GetTemplates()
        {
            using (DataContext _db = new DataContext())
            {
                return PartialView("~/Views/Message/_Templates.cshtml", _db.MessageTemplates.ToList());
            }
        }
        [HttpPost]
        public PartialViewResult GetMessage()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.MessageTemplates = _db.MessageTemplates.Where(m => m.message_status == MessageStatus.Problematic).Select(s => s).ToList();
                return PartialView("~/Views/Message/_Messages.cshtml", _db.Towers.ToList());
            }
        }
        [HttpPost]
        public JsonResult SendMessage(List<SendData> data)
        {
            var alert_message = "";
            using (DataContext _db = new DataContext())
            {
                _db.Database.CommandTimeout = 6000;
                try
                {
                    DateTime dateFrom, dateTo;
                    if (data.Select(s => s.selected).FirstOrDefault() == true)
                    {
                        var smsid = data[0].smsId;
                        var smstype = data[0].smsType;
                        var dtfrom = data[0].dateFrom;
                        var dtto = data[0].dateTo;
                        data.Clear();
                        var sendData = _db.Towers.ToList();
                       for(int j=0; j< sendData.Count();j++)
                        {
                            SendData send = new SendData();
                            send.Id = sendData[j].Id;
                            send.smsId = smsid;
                            send.manual = true;
                            send.selected = true;
                            send.smsType = smstype;
                            send.dateFrom = dtfrom;
                            send.dateTo = dtto;
                            data.Add(send);
                        }
                    }
                    int user_id = ((User)Session["CurrentUser"]).Id;
                    if (data != null)
                    {
                        var groupedCardList = data
                              .GroupBy(u => u.smsId)
                              .Select(grp => grp.ToList())
                              .ToList();

                        foreach(var dataitem in groupedCardList)
                        {
                            var send_text = "";
                            var smstyp = dataitem.Select(s => s.smsType).FirstOrDefault();
                            if (dataitem.Select(s => s.manual).FirstOrDefault() == true)
                            {
                                send_text= dataitem.Select(s => s.smsId).FirstOrDefault();
                            }
                            else
                            {
                                var id =Convert.ToInt32(dataitem.Select(s => s.smsId).FirstOrDefault());
                                send_text = _db.MessageTemplates.Where(a => a.Id == id).Select(s => s.Desc).FirstOrDefault();
                            }
                                var messge_indicator = 0;
                                Message message = new Message();
                                message.UserId = user_id;
                                message.Date = DateTime.Now;
                                message.MessageType = smstyp.ToString();
                                message.MessageText = send_text;

                            foreach (var item in dataitem)
                            {   
                                dateFrom = Convert.ToDateTime(item.dateFrom);
                                dateTo = Convert.ToDateTime(item.dateTo);
                                var cards = _db.Cards.Where(c => c.TowerId == item.Id && c.CardStatus != CardStatus.Canceled).Select(s => s).ToList();
                                List<Customer> custumer = _db.Customers.ToList();
                                List<int> abonent_id = new List<int>();
                                //List<int> cardID = new List<int>(); /*abonent.Select(s => s.CustomerId).Distinct().ToList();*/
                               
                                List<MessageAbonent> messageabonent = new List<MessageAbonent>();

                                if (messge_indicator == 0 && cards.Count() != 0)
                                {
                                     messge_indicator = 1;
                                    _db.Messages.Add(message);
                                    _db.SaveChanges();
                                }

                                foreach (var i in cards)
                                {
                                    if (abonent_id.Contains(i.CustomerId))
                                    {

                                    }
                                    else
                                    {
                                        abonent_id.Add(i.CustomerId);
                                        //cardID.Add(i.Id);
                                        messageabonent.Add(new MessageAbonent
                                        {
                                            MessageId = _db.Messages.OrderByDescending(o => o.Id).Select(s => s.Id).FirstOrDefault(),
                                            CardId =i.Id,
                                           
                                        });
                                    }
                                }
                                if (cards.Count != 0)
                                {
                                    _db.MessageAbonents.AddRange(messageabonent);
                                    _db.SaveChanges();

                                }
                                string[] typess = message.MessageType.Split(',');
                                if (typess.Contains("2")) //sms
                                {
                                    for (int i = 0; i < abonent_id.Count(); i++)
                                    {

                                        string onRegMsg = "";

                                        if (send_text != null)
                                        {

                                            onRegMsg = String.Format( send_text,  item.dateFrom, item.dateTo);
                                            alert_message = "შეტყობინების გაგზავნა წარმატებით შესრულდა!";
                                            var phone = custumer.Where(c => c.Id == abonent_id[i]).Select(s => s.Phone1).FirstOrDefault();
                                            Task.Run(async () => { await Utils.Utils.sendMessage(phone, onRegMsg); }).Wait();


                                        }
                                    }
                                }

                                string[] types = message.MessageType.Split(',');

                                if (types.Contains("0") || types.Contains("1"))
                                {
                                    int[] ids = cards.Select(s => s.Id).Distinct().ToArray();
                                    List<Card> _cards = _db.Cards.Include(c => c.Customer).Include(c => c.Tower).Include(c => c.Receiver)
                                            .Where(c => c.CardStatus != CardStatus.Canceled)
                                            .Where(c => ids.Contains(c.Id))
                                            .Select(c => c).ToList();

                                    List<Param> Params = _db.Params.ToList();

                                    int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
                                    string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                    string username = Params.First(p => p.Name == "SMSPassword").Value;
                                    string password = Params.First(p => p.Name == "SMSUsername").Value;
                                    
                                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                    _socket.Connect();

                                    foreach (Card card in _cards)
                                    {
                                        message.MessageText = Utils.Utils.ReplaceMessageTags(message.MessageText, card, _db);
                                        foreach (string type in types)
                                        {
                                            switch (type)
                                            {
                                                case "0": //osd
                                                    {
                                                        if (!_socket.SendOSDRequest(int.Parse(card.CardNum), message.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
                                                        {
                                                            _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                            {
                                                                CardId = card.Id,
                                                                MessageId = message.Id,
                                                                MessageType = MessageType.OSD,
                                                            });
                                                        }
                                                    }
                                                    break;
                                                case "1":
                                                    {
                                                        if (!_socket.SendEmailRequest(int.Parse(card.CardNum), message.MessageText, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)))
                                                        {
                                                            _db.MessageNotSendLogs.Add(new MessageNotSendLog
                                                            {
                                                                CardId = card.Id,
                                                                MessageId = message.Id,
                                                                MessageType = MessageType.Email,
                                                            });
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    _db.SaveChanges();
                                    _socket.Disconnect();
                                }
                            }
                      
                        }
                        
                    }

                        

                }
                catch(Exception ex)
                {
                    var error = ex;
                    return Json(alert_message);
                }
                return Json(alert_message);
            }
        }
        [HttpPost]
        public PartialViewResult GetTemplate(int id)
        {
            MessageTemplate _template = new MessageTemplate();
            if (id > 0)
            {
                using (DataContext _db = new DataContext())
                {
                    _template = _db.MessageTemplates.Where(m => m.Id == id).FirstOrDefault();
                }
            }
            return PartialView("~/Views/Message/_MessageTemplateModal.cshtml", _template);
        }

        [HttpPost]
        public JsonResult DeleteTemplate(int id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    MessageTemplate _t = _db.MessageTemplates.Where(c => c.Id == id).FirstOrDefault();
                    if (_t != null)
                    {
                        _db.Entry(_t).State = System.Data.Entity.EntityState.Deleted;
                        _db.MessageTemplates.Remove(_t);
                        _db.SaveChanges();
                    }
                }
                catch
                {
                    return Json(0);
                }
            }
            return Json(1);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddTemplate(MessageTemplate template)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (DataContext _db = new DataContext())
                    {
                        if (template.Id > 0)
                        {
                            MessageTemplate _t = _db.MessageTemplates.Where(c => c.Id == template.Id).FirstOrDefault();
                            if (_t != null)
                            {
                                _t.Name = template.Name;
                                _t.Desc = template.Desc;
                                _t.message_status = template.message_status;
                                _db.Entry(_t).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        else
                        {
                            _db.MessageTemplates.Add(template);
                        }

                        _db.SaveChanges();
                    }
                }
                catch
                {
                    return Json(null);
                }
            }
            return Json(template);
        }

	}
}
public class SendData
{
    public int Id { get; set; }
    public string smsId { get; set; }
    public string dateFrom { get; set; }
    public string dateTo { get; set; }
    public int smsType { get; set; }
    public bool selected { get; set; }
    public bool manual { get; set; }
}