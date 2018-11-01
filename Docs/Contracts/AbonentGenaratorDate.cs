using Dapper;
using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace DigitalTVBilling.Docs.Contracts
{
    public interface IAbonentDocsInfo
    {
        AbonentDocsModel Result();
    }
    public class AbonentGenaratorDate : IAbonentDocsInfo
    {
        private readonly IDbConnection db;
        private readonly Abonent abonent;

        public AbonentGenaratorDate(IDbConnection db, Abonent abonent)
        {
            this.db = db;
            this.abonent = abonent;
        }



        public AbonentDocsModel Result()
        {
            return new AbonentDocsModel
            {
                abonent = (abonent.Customer.Name + " " + abonent.Customer.LastName),
                abonent_num = abonent.Cards.Select(s => s.CardNum).FirstOrDefault(),
                abonent_signature = abonent.Customer.signature,
                signature_attachment = abonent.Customer.signature_attachment,
                address = abonent.Customer.Address,
                code = abonent.Customer.Code,
                doc_num = abonent.Cards.Select(s => s.DocNum).FirstOrDefault(),
                email = abonent.Customer.Email,
                phone = abonent.Customer.Phone1,
                tdate = abonent.Customer.Tdate,
                user_name = abonent.Customer.User.Name
            };
            //var FileName = @"SELECT c.doc_num,c.tdate,cu.name+' '+cu.lastname AS abonent ,s.name AS user_name,cu.code,c.address,cu.phone1 AS phone ,cu.email,c.abonent_num,cu.signature  AS abonent_signature FROM book.Cards AS c 
            //INNER JOIN book.Customers AS cu ON cu.id = c.customer_id
            //                                    INNER JOIN book.Users AS s on s.id = c.user_id where cu.code = '" + card_Id + "'";
            //var path = Path.Combine(HostingEnvironment.MapPath("~/Static/Images"), FileName);
            //string imagepath = path;
            //FileStream fs = new FileStream(imagepath, FileMode.Open);
            //byte[] byData = new byte[fs.Length];
            //fs.Read(byData, 0, byData.Length);
            //var base64 = Convert.ToBase64String(byData);
            //var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            //var imss = db.Query<AbonentDocsModel>(@"SELECT c.doc_num,c.tdate,cu.name+' '+cu.lastname AS abonent ,s.name AS user_name,cu.code,c.address,cu.phone1 AS phone ,cu.email,c.abonent_num,cu.signature  AS abonent_signature FROM book.Cards AS c 
            //                                    INNER JOIN book.Customers AS cu ON cu.id=c.customer_id
            //                                    INNER JOIN book.Users AS s on s.id=c.user_id where cu.code='" +card_Id+ "'").ToList();
            //var ims = db.Query<AbonentDocsModel>(@"SELECT c.doc_num,c.tdate,cu.name+' '+cu.lastname AS abonent ,s.name AS user_name,cu.code,c.address,cu.phone1 AS phone ,cu.email,c.abonent_num,cu.signature  AS abonent_signature FROM book.Cards AS c 
            //                                    INNER JOIN book.Customers AS cu ON cu.id=c.customer_id
            //                                    INNER JOIN book.Users AS s on s.id=c.user_id where c.id="+ card_Id + "").FirstOrDefault();

            //return imss;
        }
    }
}
