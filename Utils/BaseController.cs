using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace DigitalTVBilling.Utils
{

    public abstract class BaseController : Controller
    {

        protected override JsonResult Json(object data, string contentType,Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                JsonRequestBehavior = behavior,
                Settings = { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat }
            };
        }

        public long AddLoging(DataContext _db, LogType type, LogMode mode, int user_id, long type_id, string type_value, List<LoggingData> items)
        {
            Logging _logging = new Logging
            {
                Tdate = DateTime.Now,
                Type = type,
                UserId = user_id,
                Mode = mode,
                TypeId = type_id,
                TypeValue = type_value
            };
            _db.Loggings.Add(_logging);
            _db.SaveChanges();

            _db.LoggingItems.AddRange(items.Where(c=>c.field != null).Select(c => new LoggingItem
            {
                LoggingId = _logging.Id,
                ColumnDisplay = c.field.Replace(':', ' ').Trim(),
                NewValue = c.new_val,
                OldValue = c.old_val
            }));

            return _logging.Id;
        }
    }
}