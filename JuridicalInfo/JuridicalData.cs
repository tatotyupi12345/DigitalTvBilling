using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace DigitalTVBilling.Juridical
{
    public class JuridicalData
    {
        //public JuridicalData()
        //{

        //}
        public async System.Threading.Tasks.Task<JuridicalModel> ReturnResultJuridical(JuridicalFilters juridicalFilter, JuridicalWhere juridicalWhereQuery)
        {
            using (DataContext _db = new DataContext())
            {

                var _counts = await _db.Database.SqlQuery<int>(FilterCount(juridicalWhereQuery)).FirstOrDefaultAsync();
                return new JuridicalModel
                {

                    JuridicalStatus = _db.Database.SqlQuery<JuridicalStatus>("SELECT * FROM dbo.JuridicalStatus").ToList(),
                    count = _counts,
                    juridicalLists = await _db.Database.SqlQuery<JuridicalResult>(Filter(juridicalWhereQuery)).ToRawPagedListAsync(_counts, juridicalFilter.page, juridicalWhereQuery.pageSize),
                    page = juridicalFilter.page,
                    drpfiltet = juridicalFilter.drp_filter,
                    filterText = juridicalFilter.name,
                    totalItemsCount = _counts

                };


            }
        }
        public List<JuridicalStatus> ReturnStatusInfo(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<JuridicalStatus>($"SELECT * FROM dbo.JuridicalStatus where card_id={card_id}").ToList();
            }
        }
        public List<JuridicalLogging> juridicalLoggings(int card_id)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<JuridicalLogging>($"SELECT * FROM dbo.JuridicalLogging where card_id={card_id}").ToList();
            }
        }
        public ReturnJson SaveLogging(StatusInfo statusInfo, int user_id,CardNumID card)
        {
            using (DataContext _db = new DataContext())
            {
                using (DbContextTransaction tran = _db.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        _db.Loggings.Add(new Logging()
                        {
                            Tdate = DateTime.Now,
                            UserId = user_id,
                            Type = LogType.Card,
                            Mode = LogMode.JuridVerify,
                            TypeValue = card.card_num,
                            TypeId = card.id
                        });

                        _db.SaveChanges();
                        tran.Commit();
                        return new ReturnJson
                        {
                            Status = _db.Database.SqlQuery<int>($"SELECT s.status FROM dbo.JuridicalStatus s where card_id={statusInfo.id}").ToList(),
                            ID = 1
                        };
                    }
                    catch
                    {
                        return new ReturnJson
                        {
                            Status = null,
                            ID = 0
                        };
                    }
                }
            }
        }
        public void UpdateCardVerifuStatus(StatusInfo statusInfo)
        {
            using (DataContext _db = new DataContext())
            {
                _db.Database.ExecuteSqlCommand("UPDATE [book].[Cards] SET [juridical_verification] ='" + statusInfo.statusArray[0] + "' where id=" + statusInfo.id);
                _db.SaveChanges();
            }
        }
        public CardNumID CardInfoData(int id)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<CardNumID>($"SELECT c.id,c.card_num FROM book.Cards c where c.id={id}").FirstOrDefault();
            }
        }
        public void DeleteStatus(StatusInfo statusInfo)
        {
            using (DataContext _db = new DataContext())
            {
                _db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[JuridicalStatus] where card_id=" + statusInfo.id + "");
                _db.Database.ExecuteSqlCommand("UPDATE [book].[Cards] SET [juridical_verify_status] ='" + String.Join(",", statusInfo.statusArray.Select(s => s.ToString()).ToArray()) + "' where id=" + statusInfo.id + "");
                _db.SaveChanges();
            }
        }
        //public bool SaveStatusInfo(int card_id, int user_id, int status)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        _db.JuridicalStatus.Add(new JuridicalStatus()
        //        {
        //            tdate = DateTime.Now,
        //            card_id = card_id,
        //            user_id = user_id,
        //            status = status

        //        });
        //        _db.SaveChanges();
        //    }
        //    return true;
        //}
        //public JuridicalLogging loggings(int id, int status_id)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        return _db.Database.SqlQuery<JuridicalLogging>("SELECT * FROM dbo.JuridicalLogging where card_id=" + id + " and status=" + status_id + "").FirstOrDefault();
        //    }
        //}
        //public bool SaveLoggings( int card_id, int user_id, int status_id)
        //{
        //    using (DataContext _db = new DataContext())
        //    {
        //        if (_db.Database.SqlQuery<JuridicalLogging>("SELECT * FROM dbo.JuridicalLogging where card_id=" + card_id + " and status=" + status_id + "").FirstOrDefault() == null)
        //        {
        //            _db.JuridicalLoggings.Add(new JuridicalLogging()
        //            {
        //                tdate = DateTime.Now,
        //                card_id = card_id,
        //                user_id = user_id,
        //                status = status_id,
        //                name = _db.Database.SqlQuery<string>("SELECT u.name FROM book.Users u where u.id=" + user_id).FirstOrDefault()

        //            });
        //            _db.SaveChanges();
        //        }
        //        return true;
        //    }
        //}
        public bool SelectSaveStatus(int id, int status_id, int user_id, DataContext _db)
        {
            _db.JuridicalStatus.Add(new JuridicalStatus()
            {
                tdate = DateTime.Now,
                card_id = id,
                user_id = user_id,
                status = status_id

            });
            _db.SaveChanges();
            return true;
        }
        public string Filter(JuridicalWhere juridicalWhere)
        {
            string str = str = @"SELECT  TOP(" + juridicalWhere.pageSize + @") d.id AS Id,d.tdate as Tdate,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.Usname as UsName,d.Ustype as UsType,d.juridical_verification as JuridicalVerifical,d.city AS City, d.addR AS Address, d.abonent_num AS Abonent_Num, d.phone1 AS Phone,d.abonent_num AS Num,d.signature, d.card_num AS CardNum,d.status AS Status,d.user_id as user_id, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,ty.name as Ustype,us.name as Usname,cr.tdate,cr.id,c.name,c.lastname,c.code,c.[type],c.city,c.address AS addR,c.phone1,c.signature, cr.doc_num, cr.abonent_num,cr.card_num,cr.juridical_verification, cr.status,cr.user_id,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
						 inner join book.Users as us on us.id=cr.user_id
						 inner join book.UserTypes as ty on ty.id=us.type
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 where c.type!=2 and cr.tdate between '" + juridicalWhere.dateFrom + "' and '" + juridicalWhere.dateTo + "' " + juridicalWhere.where + ") AS d WHERE d.row_num > " + (juridicalWhere.page == 1 ? 0 : (juridicalWhere.page - 1) * juridicalWhere.pageSize);
            if (juridicalWhere._filter == "jl.name")
            {
                str = @"SELECT DISTINCT TOP(" + juridicalWhere.pageSize + @") d.id AS Id,d.tdate as Tdate,(d.name+' '+d.lastname) AS Name,d.code AS Code,d.Usname as UsName,d.Ustype as UsType,d.juridical_verification as JuridicalVerifical,d.city AS City, d.addR AS Address, d.abonent_num AS Abonent_Num, d.phone1 AS Phone,d.abonent_num AS Num,d.signature, d.card_num AS CardNum,d.status AS Status,d.user_id as user_id, d.doc_num AS DocNum, d.pack AS ActivePacket 
                         FROM (SELECT row_number() over(ORDER BY cr.id DESC) AS row_num,ty.name as Ustype,us.name as Usname,cr.tdate,cr.id,c.name,c.lastname,c.code,c.[type],c.city,c.address AS addR,c.phone1,c.signature, cr.doc_num, cr.abonent_num,cr.card_num,cr.juridical_verification, cr.status,cr.user_id,
                         STUFF((SELECT '+' + p.name FROM doc.SubscriptionPackages AS sp INNER JOIN book.Packages AS p ON p.id=sp.package_id WHERE sp.subscription_id=s.id FOR XML PATH ('')),1,1,'') AS pack FROM book.Cards AS cr 
                         INNER JOIN book.Customers AS c ON c.id=cr.customer_id
						 inner join book.Users as us on us.id=cr.user_id
						 inner join book.UserTypes as ty on ty.id=us.type
					     left join dbo.JuridicalLogging as jl on jl.card_id=cr.id
                         LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 where c.type!=2 and cr.tdate between '" + juridicalWhere.dateFrom + "' and '" + juridicalWhere.dateTo + "' " + juridicalWhere.where + ") AS d WHERE d.row_num > " + (juridicalWhere.page == 1 ? 0 : (juridicalWhere.page - 1) * juridicalWhere.pageSize);
            }

            return str;
        }
        public string FilterCount(JuridicalWhere juridicalWhereCount)
        {
            string str = @"SELECT   COUNT(cr.id) FROM book.Cards AS cr 
                       INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                       INNER JOIN book.Users as us on us.id=cr.user_id
					   INNER JOIN book.UserTypes as ty on ty.id=us.type
                       LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.tdate between '" + juridicalWhereCount.dateFrom + "' and '" + juridicalWhereCount.dateTo + "' " + juridicalWhereCount.where + ""; ;
            if (juridicalWhereCount._filter == "jl.name")
            {
                str = @"SELECT DISTINCT  COUNT(cr.id) FROM book.Cards AS cr 
                       INNER JOIN book.Customers AS c ON c.id=cr.customer_id 
                       INNER JOIN book.Users as us on us.id=cr.user_id
					   INNER JOIN book.UserTypes as ty on ty.id=us.type
					    left join dbo.JuridicalLogging as jl on jl.card_id=cr.id
                       LEFT JOIN doc.Subscribes AS s ON s.card_id=cr.id AND s.status=1 WHERE cr.tdate between '" + juridicalWhereCount.dateFrom + "' and '" + juridicalWhereCount.dateTo + "' " + juridicalWhereCount.where + "";
            }

            return str;
        }
    }
}