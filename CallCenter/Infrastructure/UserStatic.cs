using Dapper;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class UserStatic
    {
        private readonly IDbConnection _db;

        public UserStatic(IDbConnection db) {
            this._db = db;
        }

        public StaticCount Result(int user_id)
        {

                DateTime datefrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 01, 0);
                return new StaticCount
                {

                    order_count = _db.Query<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and get_date<='" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    order_yielding = _db.Query<int>(@"select COUNT(c.id) from book.Cards as c 
								left join book.Customers as a on a.id = c.customer_id
								left join book.Users as u on c.user_id = u.id
								left join dbo.SellerObject as so on u.object = so.ID
								left join book.UserTypes as ut on u.type = ut.id where  (ut.id = 4 or ut.id=44) AND u.id=" + user_id + " AND c.tdate BETWEEN '"+ datefrom + "' and '"+ DateTime.Now + "'").FirstOrDefault(),
                    order_remainder = _db.Query<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and get_date<='" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)OrderStatus.Closed + "and status!=" + (int)OrderStatus.Canceled + " and o.is_approved=0").FirstOrDefault(),
                    order_cancled = _db.Query<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)OrderStatus.Canceled + "").FirstOrDefault(),

                    damage_count = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    damage_yielding = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)DamageStatus.Closed + "").FirstOrDefault(),
                    damage_remainder = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)DamageStatus.Closed + " and is_approved=0").FirstOrDefault(),

                    cancel_count = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    cancel_yielding = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)CancleStatus.Closed + "").FirstOrDefault(),
                    cancel_remainder = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)CancleStatus.Closed + "and status!=" + (int)CancleStatus.NotClosed + " and o.is_approved=0").FirstOrDefault(),
                    cancel_cancled = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)CancleStatus.NotClosed + "").FirstOrDefault(),

                    OrderCount = _db.Query<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.status=0 ").FirstOrDefault(),
                    DamageCount = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.status =11").FirstOrDefault(),
                    CancellationCount = _db.Query<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.status =4 ").FirstOrDefault()
                };
        }
    }
}