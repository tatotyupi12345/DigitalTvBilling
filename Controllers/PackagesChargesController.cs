using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalTVBilling.Models;
using System.Data.Entity;
using System.Data;
using PagedList;
using DigitalTVBilling.ListModels;
using System.Globalization;
using System.Runtime.InteropServices;
using NAudio;
using NAudio.Wave;
using System.IO;
using System.Media;
using System.Reflection;
using System.Web.Services;
using System.Web.Script.Services;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Dapper;

namespace DigitalTVBilling.Controllers
{
    public class PackagesChargesController : Controller
    {

        // GET: PackagesCharges
        public ActionResult Index(int? page, string name, string dt_from, string dt_to, int? drp_filter, int? _filter)
        {

            if (!Utils.Utils.GetPermission("PACKAGES_CHARGES_SHOW"))
            {
                return new RedirectResult("/Abonent");
            }
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            using (DataContext _db = new DataContext())
            {
                List<Subscribtion> PackChargesID = new List<Subscribtion>();
                //List<Subscribtion> PackChargesID12 = new List<Subscribtion>();
                List<Package> PackCharges = _db.Packages.ToList();
                List<PackagesChargesList> Pack_Charges = new List<PackagesChargesList>();
                var StaticCardCahrges = _db.Subscribtions.Include("SubscriptionPackages").ToList();
                var static_8_back_active = StaticCardCahrges.Where(c => c.Status == false && c.Tdate < dateFrom && c.SubscriptionPackages.Any(a => a.PackageId == 304085)).Select(s => s.CardId).ToList();
                var static_8_false = StaticCardCahrges.Where(c => c.Status == false && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304085)).Select(s => s.CardId).ToList();
                var static_8_back = StaticCardCahrges.Where(c => c.Status == false && c.Tdate < dateFrom && c.SubscriptionPackages.Any(a => a.PackageId == 304084)).Select(s => s.CardId).ToList();
                var static_8 = StaticCardCahrges.Where(c => c.Status == false && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304084)).Select(s => s.CardId).ToList();
                var static_15_active = StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304071)).ToList();
                var static_12_active = StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304070)).ToList();
                var static_6_active = StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 303242)).ToList();

                var static_promo_back_active = StaticCardCahrges.Where(c => c.Status == false && c.Tdate < dateFrom && c.SubscriptionPackages.Any(a => a.PackageId == 304086)).Select(s => s.CardId).ToList();
                var static_promo_false = StaticCardCahrges.Where(c => c.Status == false && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304086)).Select(s => s.CardId).ToList();

                static_promo_back_active.AddRange(static_promo_false);

                static_15_active.AddRange(static_12_active);
                static_15_active.AddRange(static_6_active);

                static_8_back_active.AddRange(static_8_back);
                static_8_false.AddRange(static_8);
                int count15 = 0; int countpromo15 = 0, countpromo8 = 0;
                foreach (var item in static_15_active)
                {
                    if (static_8_back_active.Contains(item.CardId) || static_8_false.Contains(item.CardId))
                    {
                        PackChargesID.Add(item);
                        count15++;
                    }
                }
                //foreach (var item in static_15_active)
                //{
                //    if (static_promo_back_active.Contains(item.CardId))
                //    {
                //        PackChargesID.Add(item);
                //        countpromo15++;
                //    }
                //}
                foreach (var item in StaticCardCahrges.Where(c => c.Status == true && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.SubscriptionPackages.Any(a => a.PackageId == 304085)).ToList())
                {
                    if (static_promo_back_active.Contains(item.CardId))
                    {
                        PackChargesID.Add(item);
                        countpromo8++;
                    }
                }
                //var PackCardLogging = new List<Card>();
                //string card_custumer = @"  SELECT * FROM [DigitalTVBilling].[book].[Cards] AS s
                //           LEFT JOIN [DigitalTVBilling].[book].[Customers] AS sub ON sub.id=s.customer_id ";
                //using (var db = new SqlConnection("Data Source=localhost;Initial Catalog=DigitalTVBilling;User ID=sa;Password=tyupi123;MultipleActiveResultSets=true;"))
                //{
                //    //db.Open();
                //    PackCardLogging = db.Query<Card, Customer, Card>(card_custumer, (card, custumer) =>
                //    {


                //        card.Customer = custumer;

                //        return card;

                //    }).Distinct().ToList();
                //}
                var PackCardLogging = _db.Cards.Include("Customer").ToList();
                List<RecordAudioFile> Record_File = _db.RecordAudioFiles.ToList();

                if (_filter != 2 && _filter!=4)
                {
                    foreach (var item in PackChargesID)
                    {

                        PackagesChargesList pack_logg = new PackagesChargesList();
                        pack_logg.Card_Id = item.CardId;
                        pack_logg.Tdate = item.Tdate;
                        pack_logg.ChangeDate = item.Tdate;
                        pack_logg.Packages = PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Name).FirstOrDefault();
                        pack_logg.Name = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Name).FirstOrDefault();
                        pack_logg.LastName = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.LastName).FirstOrDefault();
                        pack_logg.Code = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Code).FirstOrDefault();
                        pack_logg.Phone = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Phone1).FirstOrDefault();
                        pack_logg.City = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.City).FirstOrDefault();
                        pack_logg.user_id = item.UserId;
                        pack_logg.verify_status = Record_File.Where(c => c.card_id == item.CardId).Select(s => s.verify_status).FirstOrDefault();
                        
                        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304071)
                        {
                            pack_logg.ChargesType = PackagesChargesType.CardPackageCharges;
                        }
                        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304070)
                        {
                            pack_logg.ChargesType = PackagesChargesType.PackAgesCharges12;
                        }
                        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 303242)
                        {
                            pack_logg.ChargesType = PackagesChargesType.PackAgesCharges6;
                        }
                        if (StaticCardCahrges.Where(c => c.CardId == item.CardId && c.Status == false && c.SubscriptionPackages.Any(a => a.PackageId == 304086)).Select(s=>s).FirstOrDefault()!=null)
                        {
                            if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304085)
                            {
                                pack_logg.ChargesType = PackagesChargesType.CardPackageChargesPromo8;
                            }

                        }

                        Pack_Charges.Add(pack_logg);

                    }
                }
                if (_filter == 4)
                {
                    foreach (var item in PackChargesID)
                    {

                        PackagesChargesList pack_logg = new PackagesChargesList();
                        pack_logg.Card_Id = item.CardId;
                        pack_logg.Tdate = item.Card.Tdate;
                        pack_logg.ChangeDate = item.Tdate;
                        pack_logg.Packages = PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Name).FirstOrDefault();
                        pack_logg.Name = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Name).FirstOrDefault();
                        pack_logg.LastName = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.LastName).FirstOrDefault();
                        pack_logg.Code = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Code).FirstOrDefault();
                        pack_logg.Phone = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Phone1).FirstOrDefault();
                        pack_logg.City = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.City).FirstOrDefault();
                        pack_logg.user_id = item.UserId;
                        pack_logg.verify_status = Record_File.Where(c => c.card_id == item.CardId).Select(s => s.verify_status).FirstOrDefault();
                        if (StaticCardCahrges.Where(c => c.CardId == item.CardId && c.Status == false && c.SubscriptionPackages.Any(a => a.PackageId == 304086)).Select(s => s).FirstOrDefault() != null)
                        {
                            if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304085)
                            {
                                pack_logg.ChargesType = PackagesChargesType.CardPackageChargesPromo8;
                                Pack_Charges.Add(pack_logg);
                            }

                        }

                       

                    }
                }
                //if (_filter != 4)
                //{
                //    foreach (var item in PackChargesID)
                //    {

                //        PackagesChargesList pack_logg = new PackagesChargesList();
                //        pack_logg.Card_Id = item.CardId;
                //        pack_logg.Tdate = item.Tdate;
                //        pack_logg.ChangeDate = item.Tdate;
                //        pack_logg.Packages = PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Name).FirstOrDefault();
                //        pack_logg.Name = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Name).FirstOrDefault();
                //        pack_logg.LastName = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.LastName).FirstOrDefault();
                //        pack_logg.Code = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Code).FirstOrDefault();
                //        pack_logg.Phone = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Phone1).FirstOrDefault();
                //        pack_logg.City = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.City).FirstOrDefault();
                //        pack_logg.user_id = item.UserId;
                //        pack_logg.verify_status = Record_File.Where(c => c.card_id == item.CardId).Select(s => s.verify_status).FirstOrDefault();
                //        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304071)
                //        {
                //            pack_logg.ChargesType = PackagesChargesType.CardPackageCharges;
                //        }
                //        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 304070)
                //        {
                //            pack_logg.ChargesType = PackagesChargesType.PackAgesCharges12;
                //        }
                //        if (PackCharges.Where(c => c.Id == item.SubscriptionPackages.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Id).FirstOrDefault() == 303242)
                //        {
                //            pack_logg.ChargesType = PackagesChargesType.PackAgesCharges6;
                //        }


                //        Pack_Charges.Add(pack_logg);

                //    }
                //}
                if (_filter != 1 && _filter!=4)
                {
                    var CardLogging = _db.CardLogs.Where(cc => cc.Date >= dateFrom && cc.Date <= dateTo && cc.Status == CardLogStatus.Pause).Select(s => s).ToList();
                    var PackCard = CardLogging.SelectMany(s => s.Card.Subscribtions).ToList();
                    var Pack_name = PackCard.SelectMany(s => s.SubscriptionPackages).ToList();
                    foreach (var item in CardLogging)
                    {
                        PackagesChargesList pack_logg = new PackagesChargesList();

                        pack_logg.Card_Id = item.CardId;
                        pack_logg.Packages = PackCharges.Where(c => c.Id == Pack_name.Select(s => s.PackageId).FirstOrDefault()).Select(s => s.Name).FirstOrDefault();
                        pack_logg.Tdate = item.Date;
                        pack_logg.ChangeDate = item.Date;
                        pack_logg.Name = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Name).FirstOrDefault();
                        pack_logg.LastName = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.LastName).FirstOrDefault();
                        pack_logg.Code = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Code).FirstOrDefault();
                        pack_logg.Phone = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.Phone1).FirstOrDefault();
                        pack_logg.City = PackCardLogging.Where(c => c.Id == item.CardId).Select(cu => cu.Customer.City).FirstOrDefault();
                        pack_logg.user_id = item.UserId;
                        pack_logg.ChargesType = PackagesChargesType.CardPaused;
                        pack_logg.verify_status = Record_File.Where(c => c.card_id == item.CardId).Select(s => s.verify_status).FirstOrDefault();

                        Pack_Charges.Add(pack_logg);

                    }
                }

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
                                    Pack_Charges = Pack_Charges.OrderByDescending(o => o.Card_Id).Where(c => (c.Name.Contains(fname) && c.LastName.Contains(lname)) || (c.Name.Contains(lname) && c.LastName.Contains(fname))).ToList();
                                break;
                            case 2:
                                Pack_Charges = Pack_Charges.OrderByDescending(o => o.Card_Id).Where(c => c.Code.Contains(name)).ToList();
                                break;
                            case 3:
                                Pack_Charges = Pack_Charges.OrderByDescending(o => o.Card_Id).Where(c => c.Phone.Contains(name)).ToList();
                                break;

                            default:
                                break;
                        }

                }
                if (_filter == 3)
                {
                    Pack_Charges = Pack_Charges.Where(c => c.verify_status != 1).Select(s => s).ToList();
                }
                ViewBag.Selected = _filter;
                ViewBag._Packages = Pack_Charges.OrderByDescending(s => s.Tdate).ToList();
                ViewBag.User = _db.Users.ToList();
                ViewBag.PackagesCahrges = "Active";

                return View(Pack_Charges.OrderByDescending(s => s.Tdate).ToPagedList(page ?? 1, 20));
            }
        }
        [HttpPost]
        public JsonResult RecordApprove(int card_id, int user_id, string logging_date, PlayersUploaad record_audio, string comment, int status)
        {
            DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
            DateTime date_logg = Convert.ToDateTime(logging_date, usDtfi);
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int __user_id = ((User)Session["CurrentUser"]).Id;
                        _db.Loggings.Add(new Logging()
                        {
                            Tdate = DateTime.Now,
                            UserId = __user_id,
                            Type = LogType.Card,
                            Mode = LogMode.Verify,
                            TypeValue = card_id.ToString(),
                            TypeId = card_id

                        });
                        _db.SaveChanges();

                        var RecordAudioDelete = _db.RecordAudioFiles.Where(c => c.card_id == card_id && c.Status == (PackagesChargesStatus)status).FirstOrDefault();
                        if (RecordAudioDelete != null) // განმეორებადი მონაცემების წაშლა
                        {
                            if (System.IO.File.Exists(RecordAudioDelete.audio_name))
                            {
                                System.IO.File.Delete(RecordAudioDelete.audio_name);
                            }
                            _db.RecordAudioFiles.Remove(RecordAudioDelete);
                            _db.SaveChanges();
                        }
                        _db.RecordAudioFiles.Add(new RecordAudioFile    // ახალი ჩანაწერის დამამტება
                        {
                            card_id = card_id,
                            audio_name = record_audio.addres_path,
                            info = comment,
                            Status = (PackagesChargesStatus)status,
                            verify_status = 1
                        });
                        _db.SaveChanges();

                        tran.Commit();
                        return Json(1);
                        //  }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }
        [HttpPost]
        public PartialViewResult GetInfoPlayer(int card_id, int status)
        {
            using (DataContext _db = new DataContext())
            {
                var Record = _db.RecordAudioFiles.Where(c => c.card_id == card_id && c.Status == (PackagesChargesStatus)status).FirstOrDefault();
                if (Record != null)
                {
                    string[] lines = Record.audio_name.Split('\\');
                    Record.audio_name = "/FileSource/PackagesChargesRecord/" + lines[lines.Length - 1];

                    ViewBag.RecordAudio = Record;
                }
                else
                {
                    ViewBag.RecordAudio = null;
                }
            }
            return PartialView("~/Views/PackagesCharges/_InfoPlayer.cshtml", 0);
        }

        public static string addres = "";

        [HttpPost]
        //[WebMethod]
        //[WebMethod]
        //[ScriptMethod]
        public JsonResult BrowsUploadSave(UploadFile type)
        {
            PlayersUploaad plays = new PlayersUploaad();
            try
            {

                string pathname = "";
                string _FileName = Path.GetFileName(type.typess.FileName);
                string _path = Path.Combine(Server.MapPath("~/FileSource/PackagesChargesRecord/"), _FileName);
                //pathname = "MibFiles/" + _FileName;
                addres = _path;
                type.typess.SaveAs(_path);
            }
            catch
            {
                return Json(addres);
            }
            string[] lines = addres.Split('\\');
            string line_sddres = "~/FileSource/PackagesChargesRecord/";
            plays.addres_path = addres;
            plays.play_name = "/FileSource/PackagesChargesRecord/" + lines[lines.Length - 1];
            return Json(plays);
        }
        public class UploadFile
        {
            public HttpPostedFileBase typess { get; set; }
        }
        public class PlayersUploaad
        {
            public string addres_path { get; set; }
            public string play_name { get; set; }
        }

    }
}