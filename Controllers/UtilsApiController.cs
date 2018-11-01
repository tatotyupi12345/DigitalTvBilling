using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace DigitalTVBilling.Controllers
{
    [RoutePrefix("api/utils")]
    public class UtilsApiController : ApiController
    {
        [Route("getbalancebycard/{card_id:int}")]
        public object GetBalanceByCard(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                var data = _db.Cards.Where(c => c.Id == card_id).Select(c => new
                {
                    status = c.CardStatus,
                    balance = (c.Payments.Sum(p => (decimal?)p.Amount) ?? 0) - (c.CardCharges.Sum(p => (decimal?)p.Amount) ?? 0)
                }).FirstOrDefault();

                return data;
            }
        }

        [Route("getJuridicalCards")]
        public object GetJuridicalCards(int type, string s)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Customers.Where(c => type == 1 ? c.Code == s : (c.Name + " " + c.LastName).Contains(s))
                    .Where(c => c.Type == CustomerType.Juridical)
                    .Select(c => new
                    {
                        abonent_name = c.Name,
                        cards = c.Cards.Select(cc => new
                        {
                            Id = cc.Id,
                            Name = cc.AbonentNum,
                            FinishDate = cc.FinishDate
                        }).ToList()
                    }).FirstOrDefault();
            }
        }

    }
}
