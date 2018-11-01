using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using PagedList;
using DigitalTVBilling.Models;
using PagedList.Mvc;
using DigitalTVBilling.ListModels;
using System.Xml.Linq;
using System.Data;
using DigitalTVBilling.Filters;
using System.IO;

namespace DigitalTVBilling.Controllers
{
    [ValidateUserFilter]
    public class BooksController : BaseController
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Users(int page = 1)
        {
            if (!Utils.Utils.GetPermission("USER_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(await _db.Users.AsNoTracking().Where(c => c.Id > 1).Select(u => new UserList { Id = u.Id, Name = u.Name, Type = u.UserType.Name }).OrderBy(c => c.Name).ToPagedListAsync<UserList>(page, 20));
            }
        }

        [HttpPost]
        public JsonResult FilterUsers(string letter)
        {
            using (DataContext _db = new DataContext())
            {
                List<UserList> findList = _db.Users.AsNoTracking().Where(c => c.Name.Contains(letter)).Where(c => c.Id > 1).Select(c => new UserList()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.UserType.Name
                }).OrderBy(c => c.Name).ToList();

                return Json(new
                {
                    Users = findList,
                });
            }
        }

        [HttpGet]
        public PartialViewResult NewUser(int id)
        {
            User _user = new User();
            using (DataContext _db = new DataContext())
            {
                ViewBag.UserTypes = _db.UserTypes.Select(c => new IdName { Name = c.Name, Id = c.Id }).ToList();
                ViewBag.sellers = _db.Seller.ToList();
                if (id > 0)
                {
                    _user = _db.Users.Where(u => u.Id == id).FirstOrDefault();
                    _user.Password = "";
                }
            }

            return PartialView("~/Views/Books/_NewUser.cshtml", _user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser(User user)
        {
            if (user.Id > 0)
            {
                if (ModelState.ContainsKey("Password"))
                    ModelState["Password"].Errors.Clear();
                if (ModelState.ContainsKey("Login"))
                    ModelState["Login"].Errors.Clear();
            }
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        if (user.Id == 0)
                        {
                            user.TypeName = _db.UserTypes.FirstOrDefault(u => u.Id == user.Type).Name;
                            user.Password = Utils.Utils.GetMd5(user.Password);
                            _db.Users.Add(user);
                            _db.SaveChanges();
                            this.AddLoging(_db,
                                LogType.User,
                                LogMode.Add,
                                user_id,
                                user.Id,
                                user.Name,
                                Utils.Utils.GetAddedData(typeof(User), user)
                            );
                        }
                        else
                        {
                            ConvertImage convert_image = new ConvertImage();
                            User _user = _db.Users.Where(c => c.Id == user.Id).FirstOrDefault();
                            if (_user != null)
                            {
                                if (user.Login != null)
                                    _user.Login = user.Login;
                                if (!string.IsNullOrEmpty(user.Password))
                                    _user.Password = Utils.Utils.GetMd5(user.Password);
                                _user.Name = user.Name;
                                if (user.Type != 0)
                                    _user.Type = user.Type;
                                _user.Email = user.Email;
                                _user.Phone = user.Phone;
                                if (((User)Session["CurrentUser"]).Type == 1)
                                    _user.HardAutorize = user.HardAutorize;

                                if (user.@object != null)
                                    _user.@object = user.@object;
                                if (user.CodeWord != null)
                                    _user.CodeWord = user.CodeWord;
                                if (user.Picture != null)
                                {
                                    var filename = Path.GetFileName(user.Picture.FileName);
                                    var path = Path.Combine(Server.MapPath("~/Static/Images"), filename);

                                    user.Picture.SaveAs(path);
                                    _user.image = convert_image.ImageConvert(user.Picture.FileName, "~/Static/Images"); // imigis stringad convert 
                                }
                                _db.Entry(_user).State = System.Data.Entity.EntityState.Modified;

                                List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(user.Logging);
                                if (logs != null)
                                {
                                    this.AddLoging(_db,
                                        LogType.User,
                                        LogMode.Change,
                                        user_id,
                                        user.Id,
                                        user.Name,
                                        logs
                                    );
                                }
                            }
                        }
                        _db.SaveChanges();
                        tran.Commit();

                        return new RedirectResult("/Books/Users");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }

            return new RedirectResult("/Books/Users");
        }

        [HttpGet]
        public ActionResult Cities(int page = 1)
        {
            if (!Utils.Utils.GetPermission("USER_ADD"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(_db.Cities.ToList());
            }
        }

        [HttpGet]
        public ActionResult NewCity(int id)
        {
            City _city = new City();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _city = _db.Cities.Where(u => u.ID == id).FirstOrDefault();
                }
            }

            return PartialView("~/Views/Books/_NewCity.cshtml", _city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewCity(City city)
        {
            if (city.ID > 0)
            {
                //if (ModelState.ContainsKey("Password"))
                //    ModelState["Password"].Errors.Clear();
                //if (ModelState.ContainsKey("Login"))
                //    ModelState["Login"].Errors.Clear();
            }
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        if (city.ID == 0)
                        {

                            _db.Cities.Add(city);
                            _db.SaveChanges();
                            //this.AddLoging(_db,
                            //    LogType.City,
                            //    LogMode.Add,
                            //    user_id,
                            //    city.ID,
                            //    city.Name,
                            //    Utils.Utils.GetAddedData(typeof(City), city)
                            //);
                        }
                        else
                        {
                            City _city = _db.Cities.Where(c => c.ID == city.ID).FirstOrDefault();
                            if (_city != null)
                            {
                                if (city.Name != null)
                                    _city.Name = city.Name;

                                _db.Entry(_city).State = System.Data.Entity.EntityState.Modified;

                                //List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(_city.Logging);
                                //if (logs != null)
                                //{
                                //    this.AddLoging(_db,
                                //        LogType.User,
                                //        LogMode.Change,
                                //        user_id,
                                //        city.ID,
                                //        city.Name,
                                //        logs
                                //    );
                                //}
                            }
                        }
                        _db.SaveChanges();
                        tran.Commit();

                        return Json(true);
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                }
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult Sellers(int page = 1)
        {

            if (!Utils.Utils.GetPermission("USER_ADD"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                List<SellerObject> seller = _db.Seller.ToList();
                return View(_db.Seller.ToList());
            }
        }

        [HttpGet]
        public ActionResult NewSeller(int id)
        {
            SellerObject _seller = new SellerObject();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _seller = _db.Seller.Where(u => u.ID == id).FirstOrDefault();
                }
            }

            return PartialView("~/Views/Books/_NewSeller.cshtml", _seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewSeller(SellerObject seller)
        {
            if (seller.ID > 0)
            {
                //if (ModelState.ContainsKey("Password"))
                //    ModelState["Password"].Errors.Clear();
                //if (ModelState.ContainsKey("Login"))
                //    ModelState["Login"].Errors.Clear();
            }
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        int user_id = ((User)Session["CurrentUser"]).Id;
                        if (seller.ID == 0)
                        {

                            _db.Seller.Add(seller);
                            _db.SaveChanges();
                            //this.AddLoging(_db,
                            //    LogType.City,
                            //    LogMode.Add,
                            //    user_id,
                            //    city.ID,
                            //    city.Name,
                            //    Utils.Utils.GetAddedData(typeof(City), city)
                            //);
                        }
                        else
                        {
                            SellerObject _seller = _db.Seller.Where(c => c.ID == seller.ID).FirstOrDefault();
                            if (_seller != null)
                            {
                                if (seller.name != null)
                                    _seller.name = seller.name;
                                //if (_seller.type != null)
                                _seller.type = seller.type;

                                if (seller.city != null)
                                    _seller.city = seller.city;
                                if (seller.address != null)
                                    _seller.address = seller.address;
                                if (seller.region != null)
                                    _seller.region = seller.region;
                                if (seller.ident_code != null)
                                    _seller.ident_code = seller.ident_code;
                                if (seller.info != null)
                                    _seller.info = seller.info;
                                if (seller.hostname != null)
                                    _seller.hostname = seller.hostname;
                                if (seller.phone != null)
                                    _seller.phone = seller.phone;

                                _db.Entry(_seller).State = System.Data.Entity.EntityState.Modified;

                                //List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(_city.Logging);
                                //if (logs != null)
                                //{
                                //    this.AddLoging(_db,
                                //        LogType.User,
                                //        LogMode.Change,
                                //        user_id,
                                //        city.ID,
                                //        city.Name,
                                //        logs
                                //    );
                                //}
                            }
                        }
                        _db.SaveChanges();
                        tran.Commit();

                        return Json(true);
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                }
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult Types()
        {
            if (!Utils.Utils.GetPermission("GROUP_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(_db.UserTypes.ToList());
            }
        }
        public JsonResult DeleteGroup(int id)
        {
            using (DataContext _db=new DataContext())
            {
                try
                {
                    _db.Database.ExecuteSqlCommand($"DELETE FROM [book].[UserPermissions] WHERE type ={id} ");
                    _db.Database.ExecuteSqlCommand($"DELETE FROM [book].[UserTypes] WHERE id ={id} ");
                }
                catch
                {
                    return Json(0);

                }
            }
                return Json(1);
        }

        [HttpGet]
        public PartialViewResult NewType(int id)
        {
            UserType _type = new UserType() { UserPermissions = new List<UserPermission>() };
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _type = _db.UserTypes.Include(c => c.UserPermissions).Where(u => u.Id == id).FirstOrDefault();
                }

                XDocument doc = XDocument.Load(Server.MapPath("~/App_Data/privilegies.xml"));
                foreach (XElement el in doc.Root.Elements("privilegy"))
                {
                    UserPermission _perm = _type.UserPermissions.Where(p => p.Tag == el.Element("tag").Value).FirstOrDefault();
                    if (_perm == null)
                    {
                        _type.UserPermissions.Add(new UserPermission { Name = el.Element("name").Value, Sign = false, Tag = el.Element("tag").Value, Type = id });
                    }
                }

                return PartialView("~/Views/Books/_NewType.cshtml", _type);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewType(UserType type)
        {
            if (ModelState.IsValid)
            {
                using (DataContext _db = new DataContext())
                {
                    using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                    {
                        try
                        {
                            long logging_id = 0;
                            int user_id = ((User)Session["CurrentUser"]).Id;
                            if (type.Id == 0)
                            {
                                _db.UserTypes.Add(type);
                                _db.SaveChanges();
                                logging_id = this.AddLoging(_db,
                                    LogType.UserType,
                                    LogMode.Add,
                                    user_id,
                                    type.Id,
                                    type.Name,
                                    Utils.Utils.GetAddedData(typeof(UserType), type)
                                );
                            }
                            else
                            {
                                UserType _type = _db.UserTypes.Where(c => c.Id == type.Id).FirstOrDefault();
                                if (_type != null)
                                {
                                    _type.Name = type.Name;
                                    _db.Entry(_type).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(type.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        logging_id = this.AddLoging(_db,
                                            LogType.UserType,
                                            LogMode.Change,
                                            user_id,
                                            type.Id,
                                            type.Name,
                                            logs
                                        );
                                    }
                                }
                            }

                            if (type.UserPermissions != null)
                            {
                                List<LoggingItem> p_logs = new List<LoggingItem>();
                                foreach (UserPermission _perm in type.UserPermissions)
                                {
                                    if (_perm.Id != 0)
                                    {
                                        UserPermission _p = _db.UserPermissions.Where(p => p.Id == _perm.Id).FirstOrDefault();
                                        if (_p != null)
                                        {
                                            if (logging_id == 0)
                                            {
                                                if (_p.Sign != _perm.Sign)
                                                {
                                                    p_logs.Add(new LoggingItem
                                                    {
                                                        ColumnDisplay = _p.Name,
                                                        LoggingId = logging_id,
                                                        NewValue = _p.Sign ? "ჩართული" : "გამორთული",
                                                        OldValue = _p.Sign ? "გამორთული" : "ჩართული",
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                p_logs.Add(new LoggingItem
                                                {
                                                    ColumnDisplay = _perm.Name,
                                                    LoggingId = logging_id,
                                                    NewValue = _perm.Sign ? "ჩართული" : "გამორთული",
                                                    OldValue = string.Empty
                                                });
                                            }
                                            _p.Sign = _perm.Sign;
                                            _db.Entry(_p).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                    else
                                    {
                                        _perm.Type = type.Id;
                                        _db.UserPermissions.Add(_perm);

                                        p_logs.Add(new LoggingItem
                                        {
                                            ColumnDisplay = _perm.Name,
                                            LoggingId = logging_id,
                                            NewValue = _perm.Sign ? "ჩართული" : "გამორთული",
                                            OldValue = string.Empty
                                        });
                                    }
                                }

                                if (logging_id == 0)
                                {
                                    long l_id = this.AddLoging(_db,
                                            LogType.UserType,
                                            LogMode.Change,
                                            user_id,
                                            type.Id,
                                            type.Name,
                                            new List<LoggingData>()
                                        );
                                    p_logs.ForEach(l => l.LoggingId = l_id);
                                    _db.LoggingItems.AddRange(p_logs);
                                }
                                else
                                {
                                    _db.LoggingItems.AddRange(p_logs);
                                }

                            }
                            _db.SaveChanges();
                            tran.Commit();

                            if (((User)Session["CurrentUser"]).Type == type.Id)
                                Session["UserPermissions"] = Utils.Utils.GetPrivilegies(_db, type.Id);

                            return Json(true);

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult PayTypes()
        {
            if (!Utils.Utils.GetPermission("REC_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(_db.PayTypes.ToList());
            }
        }

        [HttpGet]
        public PartialViewResult NewPayType(int id)
        {
            PayType _type = new PayType();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _type = _db.PayTypes.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewPayType.cshtml", _type);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewPayType(PayType type)
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
                            if (type.Id == 0)
                            {
                                _db.PayTypes.Add(type);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.PayType,
                                    LogMode.Add,
                                    user_id,
                                    type.Id,
                                    type.Name,
                                    Utils.Utils.GetAddedData(typeof(PayType), type)
                                );
                            }
                            else
                            {
                                PayType _type = _db.PayTypes.Where(c => c.Id == type.Id).FirstOrDefault();
                                if (_type != null)
                                {
                                    _type.Name = type.Name;
                                    _db.Entry(_type).State = EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(type.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.PayType,
                                            LogMode.Change,
                                            user_id,
                                            type.Id,
                                            type.Name,
                                            logs
                                        );
                                    }
                                }
                            }

                            _db.SaveChanges();
                            tran.Commit();

                            return Json(true);

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }


        [HttpGet]
        public ActionResult Receivers()
        {
            if (!Utils.Utils.GetPermission("REC_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            using (DataContext _db = new DataContext())
            {
                return View(_db.Receivers.OrderBy(c => c.Name).ToList());
            }
        }

        [HttpGet]
        public PartialViewResult NewReceiver(int id)
        {
            Receiver _type = new Receiver();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _type = _db.Receivers.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewReceiver.cshtml", _type);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NewReceiver(Receiver type)
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
                            if (type.Id == 0)
                            {
                                _db.Receivers.Add(type);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.Receiver,
                                    LogMode.Add,
                                    user_id,
                                    type.Id,
                                    type.Name,
                                    Utils.Utils.GetAddedData(typeof(Receiver), type)
                                );
                            }
                            else
                            {
                                Receiver _type = _db.Receivers.Where(c => c.Id == type.Id).FirstOrDefault();
                                if (_type != null)
                                {
                                    _type.Name = type.Name;
                                    _db.Entry(_type).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(type.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Receiver,
                                            LogMode.Change,
                                            user_id,
                                            type.Id,
                                            type.Name,
                                            logs
                                        );
                                    }
                                }
                            }

                            _db.SaveChanges();
                            tran.Commit();

                            return Json(true);

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }


        [HttpGet]
        public ActionResult SellAttachments()
        {
            //if (!Utils.Utils.GetPermission("REC_SHOW"))
            //{
            //    return new RedirectResult("/Main");
            //}

            using (DataContext _db = new DataContext())
            {
                return View(_db.SellAttachments.OrderBy(c => c.Name).ToList());
            }
        }
        [HttpGet]
        public ActionResult ReceiverAttachments()
        {
            //if (!Utils.Utils.GetPermission("REC_SHOW"))
            //{
            //    return new RedirectResult("/Main");
            //}

            using (DataContext _db = new DataContext())
            {
                return View(_db.ReceiverAttachments.OrderBy(c => c.Name).ToList());
            }
        }

        [HttpGet]
        public PartialViewResult NewReceiverAttachment(int id)
        {
            ReceiverAttachment attachment = new ReceiverAttachment();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    attachment = _db.ReceiverAttachments.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewReciverAttachment.cshtml", attachment);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReceiverAttachment(ReceiverAttachment attachment)
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
                            if (attachment.Id == 0)
                            {
                                if (attachment.Picture != null)
                                    if (attachment.Picture.ContentLength > 0)
                                    {
                                        if (Utils.Utils.isValitContentType(attachment.Picture.ContentType))
                                        {
                                            if (Utils.Utils.isValidLenght(attachment.Picture.ContentLength))
                                            {
                                                var filename = Path.GetFileName(attachment.Picture.FileName);
                                                var path = Path.Combine(Server.MapPath("~/Static/Images"), filename);
                                                attachment.ImagePath = "/Static/Images/" + filename;
                                                attachment.Picture.SaveAs(path);

                                                Utils.Utils.UploadFileOnFTP(path, @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", "digitalt", "1lGfnI46q9");
                                            }
                                            else
                                            {
                                                ViewBag.Error = "invalid file length. should be less than 1MB";
                                            }
                                        }
                                        else
                                        {
                                            ViewBag.Error = "invalid file type";
                                        }
                                    }


                                _db.ReceiverAttachments.Add(attachment);
                                _db.SaveChanges();


                                //this.AddLoging(_db,
                                //    LogType.Receiver,
                                //    LogMode.Add,
                                //    user_id,
                                //    attachment.Id,
                                //    attachment.Name,
                                //    Utils.Utils.GetAddedData(typeof(Receiver), attachment)
                                //);
                            }
                            else
                            {
                                ReceiverAttachment _attachment = _db.ReceiverAttachments.Where(c => c.Id == attachment.Id).FirstOrDefault();
                                if (_attachment != null)
                                {
                                    if (attachment.Picture != null)
                                        if (attachment.Picture.ContentLength > 0)
                                        {
                                            if (Utils.Utils.isValitContentType(attachment.Picture.ContentType))
                                            {
                                                if (Utils.Utils.isValidLenght(attachment.Picture.ContentLength))
                                                {
                                                    if (_attachment.ImagePath != null && _attachment.ImagePath != "")
                                                    {
                                                        var old_path = Path.Combine(Server.MapPath("~/Static/Images"), Path.GetFileName(_attachment.ImagePath));
                                                        System.IO.File.Delete(old_path);
                                                        var serverPath = Path.Combine(@"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", Path.GetFileName(_attachment.ImagePath));
                                                        serverPath = @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images/" + Path.GetFileName(_attachment.ImagePath);
                                                        Utils.Utils.DeleteFileOnFtpServer(new Uri("ftp://" + serverPath), "digitalt", "1lGfnI46q9");
                                                    }

                                                    var filename = Path.GetFileName(attachment.Picture.FileName);
                                                    var path = Path.Combine(Server.MapPath("~/Static/Images"), filename);
                                                    attachment.ImagePath = "/Static/Images/" + filename;
                                                    attachment.Picture.SaveAs(path);


                                                    Utils.Utils.UploadFileOnFTP(path, @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", "digitalt", "1lGfnI46q9");

                                                }
                                                else
                                                {
                                                    ViewBag.Error = "invalid file length. should be less than 1MB";
                                                }
                                            }
                                            else
                                            {
                                                ViewBag.Error = "invalid file type";
                                            }
                                        }

                                    _attachment.Name = attachment.Name;
                                    _attachment.ImagePath = attachment.ImagePath;
                                    _attachment.Price = attachment.Price;
                                    _db.Entry(_attachment).State = System.Data.Entity.EntityState.Modified;

                                    //List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(_attachment.Logging);
                                    //if (logs != null && logs.Count > 0)
                                    //{
                                    //    this.AddLoging(_db,
                                    //        LogType.Receiver,
                                    //        LogMode.Change,
                                    //        user_id,
                                    //        attachment.Id,
                                    //        attachment.Name,
                                    //        logs
                                    //    );
                                    //}
                                }
                            }

                            _db.SaveChanges();
                            tran.Commit();

                            return new RedirectResult("/Books/ReceiverAttachments");
                            //return Json(true);

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            ViewBag.Error = "მონაცემების შენახვის დროს მოხდა შეცდომა!";
                            RedirectToAction("ReceiverAttachments");
                        }
                    }
                }
            }

            return new RedirectResult("/Books/ReceiverAttachments");
            //return Json(false);
        }


        [HttpGet]
        public PartialViewResult NewSellAttachment(int id)
        {
            SellAttachment attachment = new SellAttachment();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    attachment = _db.SellAttachments.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewAttachment.cshtml", attachment);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSellAttachment(SellAttachment attachment)
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
                            if (attachment.Id == 0)
                            {
                                if (attachment.Picture != null)
                                    if (attachment.Picture.ContentLength > 0)
                                    {
                                        if (Utils.Utils.isValitContentType(attachment.Picture.ContentType))
                                        {
                                            if (Utils.Utils.isValidLenght(attachment.Picture.ContentLength))
                                            {
                                                var filename = Path.GetFileName(attachment.Picture.FileName);
                                                var path = Path.Combine(Server.MapPath("~/Static/Images"), filename);
                                                attachment.ImagePath = "/Static/Images/" + filename;
                                                attachment.Picture.SaveAs(path);

                                                Utils.Utils.UploadFileOnFTP(path, @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", "digitalt", "1lGfnI46q9");
                                            }
                                            else
                                            {
                                                ViewBag.Error = "invalid file length. should be less than 1MB";
                                            }
                                        }
                                        else
                                        {
                                            ViewBag.Error = "invalid file type";
                                        }
                                    }

                                _db.SellAttachments.Add(attachment);
                                _db.SaveChanges();

                                _db.SellAttachmentsLoggings.Add(new SellAttachmentsLogging
                                {
                                    SellattachmentId = attachment.Id,
                                    SellattachmentName = attachment.Name,
                                    Tdate = DateTime.Now,
                                    user_id = ((User)Session["CurrentUser"]).Id,
                                    Price = attachment.Price

                                });
                                _db.SaveChanges();

                                //this.AddLoging(_db,
                                //    LogType.Receiver,
                                //    LogMode.Add,
                                //    user_id,
                                //    attachment.Id,
                                //    attachment.Name,
                                //    Utils.Utils.GetAddedData(typeof(Receiver), attachment)
                                //);
                            }
                            else
                            {
                                SellAttachment _attachment = _db.SellAttachments.Where(c => c.Id == attachment.Id).FirstOrDefault();
                                if (_attachment != null)
                                {
                                    if (attachment.Picture != null)
                                        if (attachment.Picture.ContentLength > 0)
                                        {
                                            if (Utils.Utils.isValitContentType(attachment.Picture.ContentType))
                                            {
                                                if (Utils.Utils.isValidLenght(attachment.Picture.ContentLength))
                                                {
                                                    if (_attachment.ImagePath != null && _attachment.ImagePath != "")
                                                    {
                                                        var old_path = Path.Combine(Server.MapPath("~/Static/Images"), Path.GetFileName(_attachment.ImagePath));
                                                        System.IO.File.Delete(old_path);
                                                        var serverPath = Path.Combine(@"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", Path.GetFileName(_attachment.ImagePath));
                                                        serverPath = @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images/" + Path.GetFileName(_attachment.ImagePath);
                                                        Utils.Utils.DeleteFileOnFtpServer(new Uri("ftp://" + serverPath), "digitalt", "1lGfnI46q9");
                                                    }

                                                    var filename = Path.GetFileName(attachment.Picture.FileName);
                                                    var path = Path.Combine(Server.MapPath("~/Static/Images"), filename);
                                                    attachment.ImagePath = "/Static/Images/" + filename;
                                                    attachment.Picture.SaveAs(path);


                                                    Utils.Utils.UploadFileOnFTP(path, @"188.93.95.4/public_html/wp-content/themes/hoshi/elite/Static/Images", "digitalt", "1lGfnI46q9");

                                                }
                                                else
                                                {
                                                    ViewBag.Error = "invalid file length. should be less than 1MB";
                                                }
                                            }
                                            else
                                            {
                                                ViewBag.Error = "invalid file type";
                                            }
                                        }

                                    _attachment.Name = attachment.Name;
                                    if (attachment.ImagePath!=null)
                                    _attachment.ImagePath = attachment.ImagePath;
                                    _attachment.Price = attachment.Price;
                                    _db.Entry(_attachment).State = System.Data.Entity.EntityState.Modified;

                                    _db.SellAttachmentsLoggings.Add(new SellAttachmentsLogging
                                    {
                                        SellattachmentId = attachment.Id,
                                        SellattachmentName = attachment.Name,
                                        Tdate = DateTime.Now,
                                        user_id = ((User)Session["CurrentUser"]).Id,
                                        Price = attachment.Price
                                    });

                                    //List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(_attachment.Logging);
                                    //if (logs != null && logs.Count > 0)
                                    //{
                                    //    this.AddLoging(_db,
                                    //        LogType.Receiver,
                                    //        LogMode.Change,
                                    //        user_id,
                                    //        attachment.Id,
                                    //        attachment.Name,
                                    //        logs
                                    //    );
                                    //}
                                }
                            }

                            _db.SaveChanges();
                            tran.Commit();

                            return new RedirectResult("/Books/SellAttachments");
                            //return Json(true);

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            ViewBag.Error = "მონაცემების შენახვის დროს მოხდა შეცდომა!";
                            RedirectToAction("SellAttachments");
                        }
                    }
                }
            }

            return new RedirectResult("/Books/SellAttachments");
            //return Json(false);
        }

        [HttpGet]
        public ActionResult Towers()
        {
            if (!Utils.Utils.GetPermission("REC_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(_db.Towers.OrderBy(t => t.Name).ToList());
            }
        }

        [HttpGet]
        public PartialViewResult NewTower(int id)
        {
            Tower _type = new Tower();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _type = _db.Towers.Where(u => u.Id == id).FirstOrDefault();
                    if (_type.Range != null)
                    {
                        _type.Ranges = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(_type.Range);
                        if (_type.Ranges.Count == 0)
                            _type.Ranges = new List<string>() { "" };
                    }
                    else
                    {
                        _type.Ranges = new List<string>() { "" };
                    }
                }
                else
                {
                    _type.Ranges = new List<string>() { "" };
                }

                return PartialView("~/Views/Books/_NewTower.cshtml", _type);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewTower(Tower type)
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
                            if (type.Id == 0)
                            {
                                type.Range = Newtonsoft.Json.JsonConvert.SerializeObject(type.Ranges.Where(c => c != ""));
                                _db.Towers.Add(type);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.Tower,
                                    LogMode.Add,
                                    user_id,
                                    type.Id,
                                    type.Name,
                                    Utils.Utils.GetAddedData(typeof(Tower), type)
                                );
                            }
                            else
                            {
                                Tower _type = _db.Towers.Where(c => c.Id == type.Id).FirstOrDefault();
                                if (_type != null)
                                {

                                    _type.Name = type.Name;
                                    _type.Range = Newtonsoft.Json.JsonConvert.SerializeObject(type.Ranges.Where(c => c != ""));
                                    _type.towerLat = type.towerLat;
                                    _type.towerLon = type.towerLon;
                                    _db.Entry(_type).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(type.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Tower,
                                            LogMode.Change,
                                            user_id,
                                            type.Id,
                                            type.Name,
                                            logs
                                        );
                                    }
                                }
                            }
                            _db.SaveChanges();
                            tran.Commit();

                            return Json(true);

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }

        [HttpGet]
        public async Task<ActionResult> Services(int page = 1)
        {
            if (!Utils.Utils.GetPermission("REC_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(await _db.Services.ToListAsync());
            }
        }

        [HttpGet]
        public PartialViewResult NewService(int id)
        {
            Service _type = new Service();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    _type = _db.Services.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewService.cshtml", _type);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewService(Service service)
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
                            if (service.Id == 0)
                            {
                                _db.Services.Add(service);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.Service,
                                    LogMode.Add,
                                    user_id,
                                    service.Id,
                                    service.Name,
                                    Utils.Utils.GetAddedData(typeof(Service), service)
                                );
                            }
                            else
                            {
                                Service _service = _db.Services.Where(c => c.Id == service.Id).FirstOrDefault();
                                if (_service != null)
                                {
                                    _service.Name = service.Name;
                                    _service.Amount = service.Amount;
                                    _service.IsEdit = service.IsEdit;
                                    _db.Entry(_service).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(service.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Service,
                                            LogMode.Change,
                                            user_id,
                                            _service.Id,
                                            _service.Name,
                                            logs
                                        );
                                    }
                                }
                            }
                            _db.SaveChanges();
                            tran.Commit();

                            return Json(true);
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult Reasons()
        {
            if (!Utils.Utils.GetPermission("REC_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(_db.Reasons.Where(r => (int)r.ReasonType != -1).ToList());
            }
        }

        [HttpGet]
        public PartialViewResult NewReason(int id)
        {
            Reason reason = new Reason();
            using (DataContext _db = new DataContext())
            {
                if (id > 0)
                {
                    reason = _db.Reasons.Where(u => u.Id == id).FirstOrDefault();
                }

                return PartialView("~/Views/Books/_NewReason.cshtml", reason);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult NewReason(Reason reason)
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
                            if (reason.Id == 0)
                            {
                                _db.Reasons.Add(reason);
                                _db.SaveChanges();
                                this.AddLoging(_db,
                                    LogType.Reason,
                                    LogMode.Add,
                                    user_id,
                                    reason.Id,
                                    reason.Name,
                                    Utils.Utils.GetAddedData(typeof(Reason), reason)
                                );
                            }
                            else
                            {
                                Reason _reason = _db.Reasons.Where(c => c.Id == reason.Id).FirstOrDefault();
                                if (_reason != null)
                                {
                                    _reason.Name = reason.Name;
                                    _reason.ReasonType = reason.ReasonType;
                                    _db.Entry(_reason).State = System.Data.Entity.EntityState.Modified;

                                    List<LoggingData> logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoggingData>>(reason.Logging);
                                    if (logs != null && logs.Count > 0)
                                    {
                                        this.AddLoging(_db,
                                            LogType.Reason,
                                            LogMode.Change,
                                            user_id,
                                            _reason.Id,
                                            _reason.Name,
                                            logs
                                        );
                                    }
                                }
                            }
                            _db.SaveChanges();
                            tran.Commit();

                            return Json(true);
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return Json(false);
        }


        [HttpGet]
        public ActionResult TempCasCards()
        {
            if (((User)Session["CurrentUser"]).Type != 1)
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                return View(_db.TempCasCards.OrderBy(t => t.EndDate).ToList());
            }
        }

        [HttpPost]
        public JsonResult DeleteTempCasIds(int id)
        {
            bool res = true;
            using (DataContext _db = new DataContext())
            {
                TempCasCard temp_card = _db.TempCasCards.Where(t => t.Id == id).FirstOrDefault();
                if (temp_card != null)
                {
                    string[] address = _db.Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
                    CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
                    _socket.Connect();
                    try
                    {
                        var cass = temp_card.CasIds.Split(',').Select(cc => Convert.ToInt16(cc.Trim())).ToArray();
                        DateTime dt = new DateTime(temp_card.EndDate.Year, temp_card.EndDate.Month, temp_card.EndDate.Day, 0, 0, 0, DateTimeKind.Utc);
                        if (!_socket.SendEntitlementRequestTemp(Convert.ToInt32(temp_card.CardNum), cass, DateTime.SpecifyKind(dt, DateTimeKind.Utc), false))
                        {
                        }
                        _db.TempCasCards.Remove(temp_card);
                        _db.Entry(temp_card).State = EntityState.Deleted;
                        _db.SaveChanges();
                    }
                    catch
                    {
                        res = false;
                    }
                    finally
                    {
                        _socket.Disconnect();
                    }
                }
            }

            return Json(res);
        }

        [HttpGet]
        public PartialViewResult getUserDetails(string filter)
        {
            using (DataContext _db = new DataContext())
            {
                List<UserDetails> details = new List<UserDetails>();

                var users = _db.Users.Where(u => u.CodeWord == filter).ToList();

                if (users != null)
                    foreach (var item in users)
                    {
                        UserDetails detail = new UserDetails();
                        detail.user = item;
                        detail.userType = _db.UserTypes.Where(t => t.Id == detail.user.Type).FirstOrDefault();
                        detail.sellerObj = _db.Seller.Where(s => s.ID == detail.user.@object).FirstOrDefault();

                        details.Add(detail);
                    }

                //int user_id = ((User)Session["CurrentUser"]).Id;
                if (users.Count > 0)
                {
                    int userid = users.FirstOrDefault().Id;
                    UserType user_type = _db.UserTypes.Where(t => t.Id == _db.Users.Where(u => u.Id == userid).FirstOrDefault().Type).First();
                    ViewBag.UserType = user_type;
                    ViewBag.attachs = _db.SellAttachments.ToList();
                }
                else
                {
                    ViewBag.UserType = null;
                    ViewBag.attachs = null;
                }
                return PartialView("~/Views/Books/_UserDetails.cshtml", details);
            }
        }
        [HttpGet]
        public PartialViewResult getUserDetailsAttachment(string filter)
        {
            using (DataContext _db = new DataContext())
            {
                List<UserDetails> details = new List<UserDetails>();

                var users = _db.Users.Where(u => u.CodeWord == filter).ToList();

                if (users != null)
                    foreach (var item in users)
                    {
                        UserDetails detail = new UserDetails();
                        detail.user = item;
                        detail.userType = _db.UserTypes.Where(t => t.Id == detail.user.Type).FirstOrDefault();
                        detail.sellerObj = _db.Seller.Where(s => s.ID == detail.user.@object).FirstOrDefault();

                        details.Add(detail);
                    }

                //int user_id = ((User)Session["CurrentUser"]).Id;
                if (users.Count > 0)
                {
                    int userid = users.FirstOrDefault().Id;
                    UserType user_type = _db.UserTypes.Where(t => t.Id == _db.Users.Where(u => u.Id == userid).FirstOrDefault().Type).First();
                    ViewBag.UserType = user_type;
                    ViewBag.attachs = _db.SellAttachments.ToList();
                }
                else
                {
                    ViewBag.UserType = null;
                    ViewBag.attachs = null;
                }
                return PartialView("~/Views/Books/_userDetailsAttach.cshtml", details);
            }
        }
        [HttpGet]
        public PartialViewResult getUserEditDetails(string filter, int custumer_id)
        {
            using (DataContext _db = new DataContext())
            {
                List<UserDetails> details = new List<UserDetails>();

                var users = _db.Users.Where(u => u.CodeWord == filter).ToList();

                if (users != null)
                    foreach (var item in users)
                    {
                        UserDetails detail = new UserDetails();
                        detail.user = item;
                        detail.userType = _db.UserTypes.Where(t => t.Id == detail.user.Type).FirstOrDefault();
                        detail.sellerObj = _db.Seller.Where(s => s.ID == detail.user.@object).FirstOrDefault();

                        details.Add(detail);
                    }

                //int user_id = ((User)Session["CurrentUser"]).Id;
                if (users.Count > 0)
                {
                    int userid = users.FirstOrDefault().Id;
                    UserType user_type = _db.UserTypes.Where(t => t.Id == _db.Users.Where(u => u.Id == userid).FirstOrDefault().Type).First();
                    ViewBag.UserType = user_type;
                    ViewBag.attachs = _db.SellAttachments.ToList();
                    ViewBag.card_list = _db.Cards.Where(c => c.CustomerId == custumer_id).ToList();
                }
                else
                {
                    ViewBag.UserType = null;
                    ViewBag.attachs = null;
                }
                return PartialView("~/Views/Books/_UserEditDetails.cshtml", details);
            }
        }
    }
}