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

namespace DigitalTVBilling.Controllers
{
    public class AccessoryController : Controller
    {
        // GET: Accessory
        public ActionResult Index(int?page,int?accessory_filter_id, int? user_id)
        {
            if (!Utils.Utils.GetPermission("ASSESSOTY_RETURNED_SHOW"))
            {
                return new RedirectResult("/Accessory");
            }

            List<ReturnedCardAttachment> attachs;

            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now; ;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);

            using (DataContext _db = new DataContext())
            {
                attachs = _db.ReturnedCardAttachments.Include("ReturnedCard.Card.Customer").Where(c => c.ReturnedCard.Tdate >= dateFrom && c.ReturnedCard.Tdate <= dateTo).ToList();
                if(accessory_filter_id != 0 && accessory_filter_id != null)
                {
                    attachs = attachs.Where(c => c.ReceiverAttachmentsID == accessory_filter_id).ToList();
                }
                if (user_id!=0 && user_id!=null)
                {
                    attachs = attachs.Where(a => a.ReturnedCard.Card.Customer.UserId == user_id).ToList();
                }
                ViewBag.attachments = _db.ReceiverAttachments.ToList();
                ViewBag.Users = _db.Users.ToList();
                ViewBag.selectedFilter = accessory_filter_id;
                ViewBag.selectedUserFilter = user_id;

            }
            return View(attachs.ToPagedList(page ?? 1, 40));
        }
        
        public PartialViewResult GetAccessoryInfo(int ReturnedCardsID)
        {
            bool privilegies_return = true;
            if (!Utils.Utils.GetPermission("RETURNED_ACCESSORY_CHANGE"))
            {
                privilegies_return = false;
            }
            ViewBag.ReturnedActive = privilegies_return;

            List<ReturnedCardAttachment> access;
            using (DataContext _db = new DataContext())
            {

                access= _db.ReturnedCardAttachments.Include("ReturnedCard.Card.Customer").Where(c=>c.ReturnedCardsID==ReturnedCardsID).OrderByDescending(c => c.ReturnedCardsID).ToList();
                ViewBag.attachmenlist = _db.ReceiverAttachments.ToList();
                ViewBag.ReturnedCardID= ReturnedCardsID;
            }
             return PartialView("~/Views/Accessory/_AccessoryInfo.cshtml", access);
        }

        [HttpPost]
        public JsonResult UpdateEntry(List<ReturnedData > data, int code)
        {
            if (!Utils.Utils.GetPermission("ASSESSOTY_RETURNED_SHOW"))
            {
                return Json(0);
            }

            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                            List<ReturnedCardAttachment> cur_attachs = _db.ReturnedCardAttachments.Where(a => a.ReturnedCardsID == code).ToList();

                        foreach (var item in data)
                            {
                            if (item.check == true)
                            {
                                if (cur_attachs.Any(a => a.ReceiverAttachmentsID == item.AccesoryID && a.ReturnedCardsID==code))
                                {
                                   
                                }
                                else
                                {
                                    _db.ReturnedCardAttachments.Add(new ReturnedCardAttachment
                                    {
                                        ReceiverAttachmentsID = item.AccesoryID,
                                        ReturnedCardsID = code
                                    });
                                    _db.SaveChanges();
                                }
                            }

                            else if (item.check == false)
                            {
                                if (cur_attachs.Any(a => a.ReceiverAttachmentsID == item.AccesoryID && a.ReturnedCardsID == code))
                                {
                                    ReturnedCardAttachment attach = cur_attachs.Where(a => a.ReceiverAttachmentsID == item.AccesoryID).FirstOrDefault();
                                    _db.ReturnedCardAttachments.Remove(attach);
                                    _db.SaveChanges();
                                }
                            }
                        }

                            _db.SaveChanges();

                            tran.Commit();
                            return Json(1);
                        }
                    //}
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(0);
                    }
                }
            }
            return Json(0);
        }

    }
}
public class ReturnedData
{
    public bool check { get; set; }
    public int AccesoryID { get; set; }
}
