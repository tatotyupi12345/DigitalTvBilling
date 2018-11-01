using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class JuridicalFilter
    {
        public string Filter(string _filter,string where ,DateTime dateFrom,DateTime dateTo,int page,int pageSize) {
            string str= str = @"SELECT  TOP(" + pageSize + @") d.id AS Id,d.tdate as Tdate,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.Usname as UsName,d.Ustype as UsType,d.juridical_verification as JuridicalVerifical,d.city AS City, d.addR AS Address, d.abonent_num AS Abonent_Num, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status,d.user_id as user_id, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,ty.name as Ustype,us.name as Usname,cr.tdate,cr.id,c.name,c.lastname,c.code,c.[type],c.city,c.address AS addR,c.phone1, cr.doc_num, cr.abonent_num,cr.card_num,cr.juridical_verification, cr.status,cr.user_id,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
						 inner join book.Users as us on us.id=cr.user_id
						 inner join book.UserTypes as ty on ty.id=us.type
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 where c.type!=2 and cr.tdate between '" + dateFrom + "' and '" + dateTo + "' " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);
            if (_filter == "jl.name")
            {
                str = @"SELECT DISTINCT TOP(" + pageSize + @") d.id AS Id,d.tdate as Tdate,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.Usname as UsName,d.Ustype as UsType,d.juridical_verification as JuridicalVerifical,d.city AS City, d.addR AS Address, d.abonent_num AS Abonent_Num, d.phone1 AS Phone,d.abonent_num AS Num, d.card_num AS CardNum,d.status AS Status,d.user_id as user_id, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,ty.name as Ustype,us.name as Usname,cr.tdate,cr.id,c.name,c.lastname,c.code,c.[type],c.city,c.address AS addR,c.phone1, cr.doc_num, cr.abonent_num,cr.card_num,cr.juridical_verification, cr.status,cr.user_id,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
						 inner join book.Users as us on us.id=cr.user_id
						 inner join book.UserTypes as ty on ty.id=us.type
					     left join dbo.JuridicalLogging as jl on jl.card_id=cr.id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 where c.type!=2 and cr.tdate between '" + dateFrom + "' and '" + dateTo + "' " + where + ") AS d WHERE d.row_num > " + (page == 1 ? 0 : (page - 1) * pageSize);
            }
                
            return str;
        }
        public string FilterCount(string _filter, string where, DateTime dateFrom, DateTime dateTo)
        {
            string str = @"SELECT   COUNT(cr.id) FROM book.Cards AS cr 
                       INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                       INNER JOIN book.Users as us on us.id=cr.user_id
					   INNER JOIN book.UserTypes as ty on ty.id=us.type
                       LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.tdate between '" + dateFrom + "' and '" + dateTo + "' " + where + ""; ;
            if (_filter == "jl.name")
            {
                str = @"SELECT DISTINCT  COUNT(cr.id) FROM book.Cards AS cr 
                       INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                       INNER JOIN book.Users as us on us.id=cr.user_id
					   INNER JOIN book.UserTypes as ty on ty.id=us.type
					    left join dbo.JuridicalLogging as jl on jl.card_id=cr.id
                       LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.tdate between '" + dateFrom + "' and '" + dateTo + "' " + where + "";
            }
               
            return str;
        }
    }
}