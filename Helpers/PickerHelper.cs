using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Helpers
{
    public static class PickerHelper
    {
        public static MvcHtmlString GetFromDate(this HtmlHelper helper, string year)
        {
            string dt_from = HttpContext.Current.Request["dt_from"];
            return new MvcHtmlString((!string.IsNullOrEmpty(dt_from) ? Utils.Utils.GetRequestDate(dt_from, true, year) : Utils.Utils.GetRequestDate("hh", true, year)).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
        }

        public static MvcHtmlString GetToDate(this HtmlHelper helper, string year)
        {
            string dt_from = HttpContext.Current.Request["dt_to"];
            return new MvcHtmlString((!string.IsNullOrEmpty(dt_from) ? Utils.Utils.GetRequestDate(dt_from, false, year) : Utils.Utils.GetRequestDate("hh", false, year)).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
        }

        public static MvcHtmlString GetUrl(this HtmlHelper helper)
        {
            return new MvcHtmlString(HttpContext.Current.Request.Url.PathAndQuery);
        }

        public static MvcHtmlString GetLocalUrl(this HtmlHelper helper)
        {
            string local_path = HttpContext.Current.Request.Url.LocalPath;
            string type = !string.IsNullOrEmpty(HttpContext.Current.Request["type"]) ? "?type=" + HttpContext.Current.Request["type"] : "";

            return new MvcHtmlString((local_path.EndsWith("/") ? local_path : local_path + "/") + type);
        }

        public static string PagerUrl(this HtmlHelper helper, int page)
        {
            string path = HttpContext.Current.Request.Url.LocalPath;
            string path_and_query = HttpContext.Current.Request.Url.PathAndQuery;

            if (HttpContext.Current.Request["dt_from"] != null || HttpContext.Current.Request["type"] != null)
            {
                if (path_and_query.IndexOf("page") != -1)
                    return path_and_query.Substring(0, path_and_query.IndexOf("page") - 1) + "&page=" + page;
                else
                    return path_and_query.Substring(path_and_query.IndexOf("page") + 1) + "&page=" + page;
            }
            else
                return path + "?page=" + page;
        }

    }
}