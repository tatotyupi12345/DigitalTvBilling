using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DigitalTVBilling.Helpers
{
    public static class CoreHelper
    {
        public static MvcHtmlString GetCustomerTypeDesc(this HtmlHelper helper, CustomerType type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString GetSellerTypeDesc(this HtmlHelper helper, SellerType type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString GetCardJuridStatusTypeDesc(this HtmlHelper helper, CardJuridicalVerifyStatus type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }
        public static MvcHtmlString GetCardBlockStatusTypeDesc(this HtmlHelper helper, CardBlockedCardsVerifictionStatus type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString GetAbonentStatusDesc(this HtmlHelper helper, AbonentVerifyStatus type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString GetLogTypeDesc(this HtmlHelper helper, LogType type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString GetLogModeDesc(this HtmlHelper helper, LogMode type)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(type));
        }

        public static MvcHtmlString SetJuriticalDisplay(this HtmlHelper helper, CustomerType type)
        {
            string res = "display: none;";
            if (type == CustomerType.Juridical)
                res = "display: block;";

            return new MvcHtmlString(res);
        }

        public static MvcHtmlString SetTempCasCardColor(this HtmlHelper helper, DateTime end_date)
        {
            if(DateTime.Now > end_date)
                return new MvcHtmlString("class=\"danger\"");

            return new MvcHtmlString("");
        }

        public static MvcHtmlString SetPackageNames(this HtmlHelper helper, List<string> packages)
        {
            if (packages == null)
                return new MvcHtmlString("");

            return new MvcHtmlString(String.Join("+", packages));
        }

        public static MvcHtmlString SetMessageTypes(this HtmlHelper helper, string type)
        {
            return new MvcHtmlString(String.Join(",", type.Split(',').Select(c => Utils.Utils.GetEnumDescription((MessageType)int.Parse(c.ToString())))));
        }

        public static MvcHtmlString SetDateDisplay(this HtmlHelper helper, DateTime date)
        {
            return new MvcHtmlString(date.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
        }

        public static MvcHtmlString SetDateDisplayWithSeconds(this HtmlHelper helper, DateTime date)
        {
            return new MvcHtmlString(date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
        }

        public static MvcHtmlString SetPacketStatusDisplay(this HtmlHelper helper, bool sign)
        {
            return new MvcHtmlString(sign ? "აქტიური" : "");
        }

        public static MvcHtmlString GetCardLogStatus(this HtmlHelper helper, CardLogStatus status)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(status));
        }

        public static MvcHtmlString GetCardVerifyStatus(this HtmlHelper helper, CardVerifyStatus status)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(status));
        }

        public static MvcHtmlString GetCardStatus(this HtmlHelper helper, CardStatus status)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(status));
        }
        public static MvcHtmlString GetSubscribAddStatus(this HtmlHelper helper, DateTime subscrib_last_date, int days)
        {
            return new MvcHtmlString((DateTime.Now - subscrib_last_date).Days >= days ? "true" : "false");
        }

        public static MvcHtmlString GetCardDamageStatus(this HtmlHelper helper, CardDamageStatus status)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(status));
        }

        public static MvcHtmlString GetCardChargeStatus(this HtmlHelper helper, CardChargeStatus status)
        {
            return new MvcHtmlString(Utils.Utils.GetEnumDescription(status));
        }

        public static MvcHtmlString RoundDecimal(this HtmlHelper helper, decimal value)
        {
            return new MvcHtmlString(Math.Round(value, 3).ToString());
        }

        public static MvcHtmlString SetGridMessageText(this HtmlHelper helper, string text)
        {
            return new MvcHtmlString(/*text.Length > 100 ? text.Substring(0, 130) + "..." : */text);
        }

        public static MvcHtmlString GetCoeff(this HtmlHelper helper, double discount, decimal subs_amount)
        {
            int service_days = 30;
            using (DataContext _db = new DataContext())
            {
                service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
            }

            int coeff = service_days;//DateTime.DaysInMonth(DateTime.Now.AddDays(0).Year, DateTime.Now.AddDays(0).Month);
            decimal dayly_amount = subs_amount / coeff / Utils.Utils.divide_card_charge_interval; 
            dayly_amount -= (dayly_amount * (decimal)discount / 100);

            return new MvcHtmlString(Math.Round(dayly_amount, 4).ToString());
        }
        public static MvcHtmlString RentCoeff(this HtmlHelper helper)
        {
            int service_days = 30;
            decimal amount = 0;
            using (DataContext _db = new DataContext())
            {
                service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                amount= decimal.Parse(_db.Params.First(p => p.Name == "Rent").Value);
            }
            return new MvcHtmlString(Math.Round(amount/service_days, 4).ToString());
        }
        public static MvcHtmlString GetLogTypeRequestValue(this HtmlHelper helper)
        {
            return new MvcHtmlString(HttpContext.Current.Request["type"] ?? "");
        }

        public static MvcHtmlString GetCloseCardAmount(this HtmlHelper helper, DateTime closeDate, int card_id)
        {
            using(DataContext _db = new DataContext())
            {
                return new MvcHtmlString(Convert.ToString(_db.CardCharges.Where(c => c.CardId == card_id).Where(c => c.Status == CardChargeStatus.Pen || c.Status == CardChargeStatus.PenDaily).Where(c => c.Tdate >= closeDate).Select(c => (decimal?)c.Amount).Sum() ?? 0));
            }
        }

        public static MvcHtmlString GetAutoTemplateUsers(this HtmlHelper helper, string where)
        {
            using (DataContext _db = new DataContext())
            {
                string sql = @"SELECT DISTINCT COUNT(*) FROM book.Cards AS cr 
                                INNER JOIN book.Customers AS c ON c.id=cr.customer_id WHERE cr.status!=4 " + where;

                return new MvcHtmlString(_db.Database.SqlQuery<int>(sql).First().ToString());
            }
        }

        public static MvcHtmlString DatetimeTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes = null)
        {
            var mvcHtmlString = System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes ?? new { });
            var xDoc = XDocument.Parse(mvcHtmlString.ToHtmlString());
            var xElement = xDoc.Element("input");
            var valueAttribute = xElement.Attribute("value");
            DateTime _date = DateTime.Now;
            if(!DateTime.TryParse(valueAttribute.Value, out _date))
            {
                _date = DateTime.Now;
            }
            valueAttribute.Value = new DateTime(_date.Year, _date.Month, _date.Day).ToString(format, CultureInfo.InvariantCulture);
            return new MvcHtmlString(xDoc.ToString());
        }

    }
}