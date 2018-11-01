using Dapper;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class RegionGoName
    {
        private readonly IDbConnection db;
        private readonly int id;
        private readonly string sqlTable;
        private readonly int user_Id;

        public RegionGoName(IDbConnection db, int id, string sqlTable,int user_id)
        {
            this.db = db;
            this.id = id;
            this.sqlTable = sqlTable;
            user_Id = user_id;
        }
        public string Result()
        {
            try
            {
                XDocument document = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/city_xml.xml"));

                var orders = db.Query<RegionNameID>($"Select data, id from  {sqlTable} where id={id}").FirstOrDefault();
                if (sqlTable == "dbo.Cancellation")
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CancellationCardNum>(orders.data);
                    var nameRegion = document.Descendants("place").Where(c => c.Element("city").Value.StartsWith(data.Customer_City)).Select(c => c.Element("city").Value + " - " + c.Element("raion").Value).FirstOrDefault();
                    if (nameRegion == null)
                    {
                        return nameRegion = user_Id + data.Customer_City;
                    }
                    else
                    {

                        return nameRegion = user_Id + nameRegion.Substring(nameRegion.IndexOf("-") + 1).Substring(1).Replace(" ", "_");

                    }
                }
                else
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(orders.data);
                    var nameRegion = document.Descendants("place").Where(c => c.Element("city").Value.StartsWith(data.Customer.City)).Select(c => c.Element("city").Value + " - " + c.Element("raion").Value).FirstOrDefault();
                    if (nameRegion == null)
                    {
                        return nameRegion = user_Id + data.Customer.City;
                    }
                    else
                    {

                        return nameRegion = user_Id + nameRegion.Substring(nameRegion.IndexOf("-") + 1).Substring(1).Replace(" ", "_");

                    }
                }
            }
            catch
            {
                return "";
            }
        }
    }
}