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
    public class CityRegionList
    {
        private readonly string name;
        private readonly UserRegionGoOrder userCityGo;
        private readonly UserRegionGoDamage userRegionGoDamage;
        private readonly UserRegionGoCancellation userRegionGoCancellation;

        public CityRegionList(UserRegionGoOrder userCityGo,UserRegionGoDamage userRegionGoDamage,UserRegionGoCancellation userRegionGoCancellation)
        {
            this.userCityGo = userCityGo;
            this.userRegionGoDamage = userRegionGoDamage;
            this.userRegionGoCancellation = userRegionGoCancellation;
        }
        public List<RegionView> Result(int user_id)
        {
            List<RegionView> RegionAdd = new List<RegionView>();
            List<RegionNameID> regionNames = new List<RegionNameID>();
            List<RegionNameID> regionNamesCancels = new List<RegionNameID>();

            regionNames.AddRange(userCityGo.Result(user_id));
            regionNames.AddRange(userRegionGoDamage.Result(user_id));
            regionNamesCancels.AddRange(userRegionGoCancellation.Result(user_id));

            foreach (var item in regionNamesCancels)
            {
                var counts = 1;
                var sp = Newtonsoft.Json.JsonConvert.DeserializeObject<CancellationCardNum>(item.data);
                XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/city_xml.xml"));
                if (doc != null && sp != null && sp.Customer_City != null)
                {
                    var nameRegion = doc.Descendants("place").Where(c => c.Element("city").Value.StartsWith(sp.Customer_City)).Select(c => c.Element("city").Value + " - " + c.Element("raion").Value).FirstOrDefault();
                    if (nameRegion == null)
                    {
                        var reg = RegionAdd.Where(c => c.name == user_id + sp.Customer_City).FirstOrDefault();
                        if (reg != null)
                        {
                            counts = reg.count;
                            counts++;
                            RegionAdd.Remove(reg);
                        }
                        RegionAdd.Add(new RegionView
                        {

                            name = user_id + sp.Customer_City,
                            id = item.id,
                            count = counts
                        });
                    }
                    else
                    {
                        if (RegionAdd.Select(s => s.name).Contains(user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2)))
                        {
                            var reg = RegionAdd.Where(c => c.name == user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2)).FirstOrDefault();
                            counts = reg.count;
                            counts++;
                            RegionAdd.Remove(reg);
                            RegionAdd.Add(new RegionView
                            {
                                name = user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2),
                                id = item.id,
                                count = counts

                            });
                        }
                        else
                        {
                            RegionAdd.Add(new RegionView
                            {
                                name = user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2),
                                id = item.id,
                                count = 1
                            });
                        }
                    }
                }
            }

            foreach (var item in regionNames)
            {
                var counts = 1;
                var sp = Newtonsoft.Json.JsonConvert.DeserializeObject<Abonent>(item.data);
                XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/city_xml.xml"));
                if (doc != null && sp.Customer!=null &&  sp.Customer.City != null)
                {
                    var nameRegion = doc.Descendants("place").Where(c => c.Element("city").Value.StartsWith(sp.Customer.City)).Select(c => c.Element("city").Value + " - " + c.Element("raion").Value).FirstOrDefault();
                    if (nameRegion == null)
                    {
                        var reg = RegionAdd.Where(c => c.name == user_id + sp.Customer.City).FirstOrDefault();
                        if (reg != null)
                        {
                            counts = reg.count;
                            counts++;
                            RegionAdd.Remove(reg);
                        }
                        RegionAdd.Add(new RegionView
                        {

                            name = user_id + sp.Customer.City,
                            id = item.id,
                            count = counts
                        });
                    }
                    else
                    {
                        if (RegionAdd.Select(s => s.name).Contains(user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2)))
                        {
                            var reg = RegionAdd.Where(c => c.name == user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2)).FirstOrDefault();
                            counts = reg.count;
                            counts++;
                            RegionAdd.Remove(reg);
                            RegionAdd.Add(new RegionView
                            {
                                name = user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2),
                                id = item.id,
                                count= counts

                            });
                        }
                        else
                        {
                            RegionAdd.Add(new RegionView
                            {
                                name = user_id + nameRegion.Substring(nameRegion.IndexOf("-") + 2),
                                id = item.id,
                                count=1
                            });
                        }
                    }
                }
            }
            return RegionAdd;
            //return RegionAdd.GroupBy(p => p.name)
            //                .Select(g => g.First())
            //                .ToList();
        }
    }
}