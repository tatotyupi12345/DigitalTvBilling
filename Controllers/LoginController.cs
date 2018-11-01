using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using DigitalTVBilling.ListModels;
using System.ComponentModel;

namespace DigitalTVBilling.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            using (DataContext _db = new DataContext())
            {
                //ViewBag.Receivers = _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList();
                //viewbag.Towers = _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList();
                List<IdName> CardEnums = ((IEnumerable<CardStatus>)Enum.GetValues(typeof(CardStatus))).Select(c => new IdName() { Id = (int)c, Name = Utils.Utils.GetAttributeOfType<DescriptionAttribute>(c).Description }).ToList();
                //viewbag.CardStatus = CardEnums;
                //viewbag.Count = 1;

                DynamicViewBag viewbag = new DynamicViewBag();
                viewbag.AddValue("Receivers", _db.Receivers.Select(r => new IdName { Id = r.Id, Name = r.Name }).ToList());
                viewbag.AddValue("Towers", _db.Towers.Select(r => new IdName { Id = r.Id, Name = r.Name }).OrderBy(r => r.Name).ToList());
                viewbag.AddValue("CardStatus", CardEnums);
                viewbag.AddValue("Count", 1);

                var templateFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Views\Abonent\_Card.cshtml";// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\Views\Abonent\_Card.cshtml");
                var templateService = new TemplateService();
                string content = System.IO.File.ReadAllText(templateFilePath);
                
                //var emailHtmlBody = templateService.Parse(content, new DigitalTVBilling.Models.Abonent() { Customer = new Customer(), Cards = new List<Card>() { new Card() } }, viewbag, null);
            }



            //Utils.Utils.sendMessage("598894533", "test");
            if (Session["SMSCode"] != null)
                Session.Remove("SMSCode");
            return View(new User());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(User user)
        {
            Func<DataContext, bool> SetLog = (DataContext _db) =>
                {
                    if (Request.UserHostAddress.ToString() != "::1")
                        this.AddLoging(_db, LogType.UserAction, LogMode.Autorize, user.Id, user.Id, Request.UserHostAddress.ToString(), new List<LoggingData>());
                    return true;
                };
            ModelState.Remove("Password");
            ModelState.Remove("CodeWord");
            ModelState.Remove("Login");
            if (ModelState.ContainsKey("Name"))
                ModelState["Name"].Errors.Clear();
            if (ModelState.ContainsKey("Phone"))
                ModelState["Phone"].Errors.Clear();
            if (ModelState.ContainsKey("Email"))
                ModelState["Email"].Errors.Clear();
            if (ModelState.IsValid)
            {
                using (DataContext _db = new DataContext())
                {
                    string password = Utils.Utils.GetMd5(user.Password);
                    user.Password = password;
                    user = _db.Users.Where(u => u.Login == user.Login && u.Password == password).FirstOrDefault();
                    //user = Utils.SqlExtraQuerys.auth(user);
                    if (user != null)
                    {
                        if (user.HardAutorize)
                        {
                            string code = Utils.Utils.GetSmsCode();
                            List<Param> Params = _db.Params.ToList();
                            string sms_user = Params.First(p => p.Name == "SMSPassword").Value;
                            string sms_pass = Params.First(p => p.Name == "SMSUsername").Value;
                            Utils.Utils.OnSendAutorizeSMS(user.Phone, code, sms_user, sms_pass);
                            Session["SMSCode"] = code;

                            return Redirect("/Login/SmsAutorize/" + user.Id);
                        }

                        return OnRedirect(_db, user);
                    }
                    else
                    {
                        ViewBag.Error = "სახელი ან პაროლი არასწორია!!!";
                        this.AddLoging(_db, LogType.UserAction, LogMode.IncorrectAutorize, 1, 1, Request.UserHostAddress.ToString(), new List<LoggingData>());
                    }
                }
            }
            else
            {
                foreach (ModelState modelState in ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        //DoSomethingWith(error);
                    }
                }
            }

            return View();
        }

        [NonAction]
        private RedirectResult OnRedirect(DataContext _db, User user)
        {
            
            Session.Add("CurrentUser", user);
            Session.Add("UserPermissions", Utils.Utils.GetPrivilegies(_db, user.Type));

            if (Request.UserHostAddress.ToString() != "::1")
                this.AddLoging(_db, LogType.UserAction, LogMode.Autorize, user.Id, user.Id, Request.UserHostAddress.ToString(), new List<LoggingData>());
            //_db.Dispose();
            return new RedirectResult("/Abonent");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (Session["CurrentUser"] != null)
            {
                Session.RemoveAll();
            }

            return new RedirectResult("/Login/Index");
        }

        public ViewResult SmsAutorize(int id)
        {
            ViewBag.Minute = 2;
            ViewBag.Second = 60;
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SmsAutorize(string submitBtn, string Sms, int UserId, int Minute, int Second)
        {
            if (submitBtn == "refresh")
            {
                string code = Utils.Utils.GetSmsCode();
                using (DataContext _db = new DataContext())
                {
                    List<Param> Params = _db.Params.ToList();
                    string sms_user = Params.First(p => p.Name == "SMSPassword").Value;
                    string sms_pass = Params.First(p => p.Name == "SMSUsername").Value;
                    Utils.Utils.OnSendAutorizeSMS(_db.Users.Where(u => u.Id == UserId).FirstOrDefault().Phone, code, sms_user, sms_pass);
                    Session["SMSCode"] = code;
                }

                return new RedirectResult("/Login/SmsAutorize/" + UserId);
            }

            if(Minute == 0 && Second == 0)
            {
                Session.Remove("SMSCode");
                ViewBag.Error = "დრო ამოიწურა!";
                return View();
            }

            if(Session["SMSCode"].Equals(Sms))
            {
                Session.Remove("SMSCode");
                using (DataContext _db = new DataContext())
                {
                    User _user = _db.Users.Where(u => u.Id == UserId).FirstOrDefault();
                    return OnRedirect(_db, _user);
                }
            }
            else
            {
                ViewBag.Minute = Minute;
                ViewBag.Second = Second;
                ViewBag.UserId = UserId;
                ViewBag.Error = "შეყვანილი კოდი არასწორია!";
                return View();
            }
        }

        [NonAction]
        private void AddLoging(DataContext _db, LogType type, LogMode mode, int user_id, int type_id, string type_value, List<LoggingData> items)
        {
            Logging _logging = new Logging
            {
                Tdate = DateTime.Now,
                Type = type,
                UserId = user_id,
                Mode = mode,
                TypeId = type_id,
                TypeValue = type_value,
            };
            _db.Loggings.Add(_logging);
            _db.SaveChanges();
        }

	}
}