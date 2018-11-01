using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Filters
{
    public class ValidateUserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["CurrentUser"] == null || filterContext.HttpContext.Session["UserPermissions"] == null)
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new RedirectResult("/Login");
                else
                {

                }
            }
        }
    }
}