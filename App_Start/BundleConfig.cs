using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DigitalTVBilling
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(

                                "~/Static/Scripts/bootstrap-datepicker.js",

                                "~/Static/Scripts/bootstrap-datepicker.ka.js",

                                "~/Static/Scripts/fina.datepickers.js",

                                "~/Static/Scripts/jquery.unobtrusive-ajax.min.js",

                                "~/Static/Scripts/highcharts.js",

                                "~/Static/Scripts/highcharts-3d.js",

                                "~/Static/Scripts/View/main2.js"
                              ));



            bundles.Add(new StyleBundle("~/Static/Styles").Include(

                                             "~/Static/Styles/datepicker3.css"
                                         ));
            BundleTable.EnableOptimizations = true;
        }
    } 
}