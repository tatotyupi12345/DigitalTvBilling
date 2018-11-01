using DigitalTVBilling.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalTVBilling.Docs.Contracts
{
    public class RenderViewString
    {
        private readonly string controllerName;
        private readonly string viewName;
        private readonly object model;
        private readonly bool isFromDiller;

        public RenderViewString(string controllerName, string viewName, object model, bool isFromDiller = false) {
            this.controllerName = controllerName;
            this.viewName = viewName;
            this.model = model;
            this.isFromDiller = isFromDiller;
        }
        public string Result()
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(fakeControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    fakeControllerContext,
                    viewResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    stringWriter
                    );
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }
    }
}