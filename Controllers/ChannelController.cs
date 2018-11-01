using DigitalTVBilling.Filters;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class ChannelController : BaseController
    {
        public ActionResult Index()
        {
            if (!Utils.Utils.GetPermission("PACKAGE_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(_db.Packages.ToList());
            }
        }

        public PartialViewResult Package(int id)
        {
            Models.Package pack;
            if (id == 0)
                pack = new Models.Package();
            else
            {
                using (DataContext _db = new DataContext())
                {
                    pack = _db.Packages.Where(p => p.Id == id).FirstOrDefault();
                }
            }
            using (DataContext _db = new DataContext())
            {
                ViewBag.package_list = _db.Packages.ToList();
            }
            
            return PartialView("~/Views/Channel/_Package.cshtml", pack);
        }

        [HttpPost]
        public JsonResult Package(Models.Package package)
        {
            if (ModelState.IsValid)
            {
                using (DataContext _db = new DataContext())
                {
                    using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        try
                        {
                            int user_id = ((User)Session["CurrentUser"]).Id;
                            if (package.Id == 0)
                            {
                                package.UserId = user_id;
                                _db.Packages.Add(package);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.Package,
                                    LogMode.Add,
                                    user_id,
                                    package.Id,
                                    package.Name,
                                    Utils.Utils.GetAddedData(typeof(Package), package)
                                );
                            }
                            else
                            {
                                Models.Package pack = _db.Packages.Where(p => p.Id == package.Id).FirstOrDefault();
                                if (pack != null)
                                {
                                    double old_price = pack.Price;
                                    double old_jurid_price = pack.JuridPrice;
                                    int old_cas_id = pack.CasId;
                                    pack.Name = package.Name;
                                    pack.Price = package.Price;
                                    pack.CasId = package.CasId;
                                    pack.IsDefault = package.IsDefault;
                                    pack.JuridPrice = package.JuridPrice;
                                    pack.MinPrice = package.MinPrice;
                                    pack.RentType = package.RentType;
                                    _db.Entry(pack).State = System.Data.Entity.EntityState.Modified;
                                    _db.SaveChanges();
                                    if (old_price != package.Price || old_jurid_price != package.JuridPrice)
                                    {
                                        var _cards = _db.Cards.Where(q => q.Subscribtions.Where(c => c.Status).Any())
                                            .Where(q => q.Subscribtions.Where(c => c.Status).Any(c => c.SubscriptionPackages.Any(p => p.PackageId == pack.Id)))
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

                                        var Params = _db.Params.ToList();
                                        string charge_time = Params.First(p => p.Name == "CardCharge").Value;
                                        decimal jurid_limit_months = decimal.Parse(Params.First(p => p.Name == "JuridLimitMonths").Value);
                                        foreach (CardDetailData _card in _cards)
                                        {

                                            if (_card.Subscribtion != null)
                                            {
                                                _card.Subscribtion.Amount = _db.SubscriptionPackages.Include("Package").Where(s => s.SubscriptionId == _card.Subscribtion.Id).Sum(p => _card.CustomerType == CustomerType.Juridical ? p.Package.JuridPrice : p.Package.Price);
                                                _db.Entry(_card.Subscribtion).State = System.Data.Entity.EntityState.Modified;

                                                if (_card.Card.CardStatus != CardStatus.FreeDays && !(_card.Card.CardStatus == CardStatus.Active && _card.Card.Mode == 1))
                                                {
                                                    Utils.Utils.ChangeFinishDateByPackage(_db, _card, charge_time, jurid_limit_months, (decimal)_card.Subscribtion.Amount);
                                                }
                                            }
                                        }
                                    }

                                    if (old_cas_id != package.CasId)
                                    {
                                        string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                                        CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                                        _socket.Connect();

                                        var _cards = _db.Cards.Where(q => q.Subscribtions.Where(c => c.Status).Any(c => c.SubscriptionPackages.Any(p => p.PackageId == pack.Id)))
                                            .Where(c => c.CardStatus != CardStatus.Canceled).ToList();
                                        foreach (Card _card in _cards)
                                        {
                                            int card_num = int.Parse(_card.CardNum);
                                            if (!_socket.SendEntitlementRequest(card_num, new short[] { (short)old_cas_id }, DateTime.SpecifyKind(_card.CasDate, DateTimeKind.Utc), false))
                                            {
                                                throw new Exception();
                                            }

                                            _card.CasDate = DateTime.Now;
                                            _db.Entry(_card).State = EntityState.Modified;
                                            if (!_socket.SendEntitlementRequest(card_num, new short[] { (short)package.CasId }, DateTime.SpecifyKind(_card.CasDate, DateTimeKind.Utc), true))
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        _socket.Disconnect();
                                        _db.SaveChanges();
                                    }

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(package.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Package,
                                            LogMode.Change,
                                            user_id,
                                            package.Id,
                                            package.Name,
                                            logs
                                            );
                                    }
                                }
                            }
                            _db.SaveChanges();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return Json("0");
                        }
                    }
                }
                return Json("1");
            }
            return Json("0");
        }

        public ViewResult Channels(int? id)
        {
            ViewBag.Package = id;
            if (id.HasValue)
            {
                using (DataContext _db = new DataContext())
                {
                    return View(_db.PackageChannels.Where(p => p.PackageId == id.Value).Select(c => c.Channel).ToList());
                }
            }
            return View(new List<Channel>());
        }

        public PartialViewResult Channel(int id)
        {
            Channel channel;
            if (id == 0)
                channel = new Channel();
            else
            {
                using (DataContext _db = new DataContext())
                {
                    channel = _db.PackageChannels.Where(p => p.ChannelId == id).Select(p => p.Channel).FirstOrDefault();
                }
            }

            ViewBag.Package = Request["package_id"];
            return PartialView("~/Views/Channel/_Channel.cshtml", channel);
        }

        [HttpPost]
        public JsonResult Channel(Channel channel)
        {
            if (ModelState.IsValid)
            {
                using (DataContext _db = new DataContext())
                {
                    using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        try
                        {
                            int user_id = ((User)Session["CurrentUser"]).Id;
                            if (channel.Id == 0)
                            {
                                channel.UserId = ((User)Session["CurrentUser"]).Id;
                                _db.Channels.Add(channel);
                                _db.SaveChanges();

                                PackageChannel pChannel = new PackageChannel()
                                {
                                    ChannelId = channel.Id,
                                    PackageId = Utils.Utils.TryIntParse(Request["package_id"]),
                                };
                                _db.PackageChannels.Add(pChannel);

                                this.AddLoging(_db,
                                    LogType.Channel,
                                    LogMode.Add,
                                    user_id,
                                    channel.Id,
                                    channel.Name,
                                    Utils.Utils.GetAddedData(typeof(Channel), channel)
                                );
                            }
                            else
                            {
                                Channel chan = _db.PackageChannels.Where(p => p.ChannelId == channel.Id).Select(c => c.Channel).FirstOrDefault();
                                if (chan != null)
                                {
                                    chan.Name = channel.Name;
                                    _db.Entry(chan).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(channel.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Package,
                                            LogMode.Change,
                                            user_id,
                                            channel.Id,
                                            channel.Name,
                                            logs
                                            );
                                    }
                                }
                            }

                            _db.SaveChanges();
                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            return Json("0");
                        }
                    }
                }
                return Json("1");
            }
            return Json("0");
        }

        [HttpPost]
        public JsonResult DeleteChannel(int id, int package_id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    PackageChannel p_ch = _db.PackageChannels.Include(p => p.Channel).Where(p => p.ChannelId == id && p.PackageId == package_id).FirstOrDefault();
                    if (p_ch != null)
                    {
                        this.AddLoging(_db,
                            LogType.Channel,
                            LogMode.Delete,
                            ((User)Session["CurrentUser"]).Id,
                            0,
                            p_ch.Channel.Name,
                            new List<LoggingData>() { new LoggingData 
                            { 
                                field = 
                                Utils.Utils.GetDisplayName(typeof(Models.Channel), "Name"), 
                                new_val = p_ch.Channel.Name,
                                old_val = string.Empty
                            }}
                          );
                        _db.Entry(p_ch.Channel).State = System.Data.Entity.EntityState.Deleted;
                        _db.Entry(p_ch).State = System.Data.Entity.EntityState.Deleted;

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
        public JsonResult DeletePackage(int package_id)
        {
            using (DataContext _db = new DataContext())
            {
                try
                {
                    //PackageChannel p_ch = _db.PackageChannels.Include(p => p.Channel).Where(p => p.ChannelId == id && p.PackageId == package_id).FirstOrDefault();
                    //Package package = _db.Packages.Where(p => p.Id == package_id).FirstOrDefault();
                    Package package = _db.Packages.Find(package_id);
                    if (package != null)
                    {
                        //List<PackageChannel> p_ch_l = _db.PackageChannels.Where(pch => pch.PackageId == package_id).ToList();
                        //foreach(PackageChannel ch in p_ch_l)
                        //{
                        //    _db.PackageChannels.Remove(ch);
                        //}

                        _db.Packages.Remove(package);
                        //_db.Entry(package).State = EntityState.Deleted;
                        _db.SaveChanges();
                    }

                    //if (p_ch != null)
                    //{
                    //    this.AddLoging(_db,
                    //        LogType.Channel,
                    //        LogMode.Delete,
                    //        ((User)Session["CurrentUser"]).Id,
                    //        0,
                    //        p_ch.Channel.Name,
                    //        new List<LoggingData>() { new LoggingData
                    //        {
                    //            field =
                    //            Utils.Utils.GetDisplayName(typeof(Models.Channel), "Name"),
                    //            new_val = p_ch.Channel.Name,
                    //            old_val = string.Empty
                    //        }}
                    //      );
                    //    _db.Entry(p_ch.Channel).State = System.Data.Entity.EntityState.Deleted;
                    //    _db.Entry(p_ch).State = System.Data.Entity.EntityState.Deleted;
                    //    _db.SaveChanges();
                    //}
                }
                catch(Exception ex)
                {
                    return Json(0);
                }
            }

            return Json(1);
        }

        [HttpPost]
        public RedirectResult Upload(int? id)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null && file.ContentLength > 0 && file.ContentType == "text/plain")
                {
                    using (DataContext _db = new DataContext())
                    {
                        using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                        {
                            try
                            {
                                int user_id = ((User)Session["CurrentUser"]).Id;
                                Channel channel;
                                string line;
                                using (StreamReader stream = new StreamReader(file.InputStream))
                                {
                                    while ((line = stream.ReadLine()) != null)
                                    {
                                        channel = new Channel();
                                        channel.UserId = user_id;
                                        channel.Name = line;
                                        _db.Channels.Add(channel);
                                        _db.SaveChanges();

                                        PackageChannel pChannel = new PackageChannel()
                                        {
                                            ChannelId = channel.Id,
                                            PackageId = id.Value,
                                        };
                                        _db.PackageChannels.Add(pChannel);
                                    }
                                }

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

            return Redirect("/Channel/Channels/" + id);
        }


    }
}