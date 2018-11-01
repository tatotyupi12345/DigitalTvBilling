using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter
{
    public class CallData
    {
        public CallData() { }

        public CallModel ReturnCallData()
        {
            using (DataContext _db = new DataContext())
            {
                return new CallModel
                {
                    users = _db.Database.SqlQuery<CallUser>(@"SELECT * FROM book.Users  where type=4 or type=44 ").ToList(),
                };
            }
        }
        public CallModel ReturnedOrder(string where)
        {
            using (DataContext _db = new DataContext())
            {          
                return new CallModel
                {
                    order = _db.Database.SqlQuery<Order>(@"SELECT o.id AS ID, o.address as Address,o.approve_user as ApproveUser,o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                  o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.is_approved as IsApproved,o.montage_status as MontageStatus,o.name as Name,o.num as Num,o.poll as Poll,o.receivers_count as ReceiversCount
                                  ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId ,o.to_go FROM doc.Orders AS o where " + where + " ").ToList(),
                    users = _db.Database.SqlQuery<CallUser>(@"SELECT * FROM book.Users where type=4 or type=44").ToList()
                };
            }
        }

        public CallModel ReturnedDamage(string where)
        {
            using (DataContext _db = new DataContext())
            {
                return new CallModel
                {
                    damage = _db.Database.SqlQuery<Damage>(@"SELECT o.id AS ID, o.address as Address,o.approve_user as ApproveUser,o.to_go,o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                  o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.is_approved as IsApproved,o.montage_status as MontageStatus,o.name as Name,o.num as Num,o.montage_user_id as montage_user_id,o.comment as comment,o.receivers_count as ReceiversCount
                                  ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId FROM dbo.Damage AS o where " + where + " ").ToList(),
                    users = _db.Database.SqlQuery<CallUser>(@"SELECT * FROM book.Users where type=4 or type=44").ToList()
                };
            }
        }
        public CallModel ReturnedCancellation(string where)
        {
            using (DataContext _db = new DataContext())
            {
                return new CallModel
                {
                    cancellation = _db.Database.SqlQuery<Cancellation>(@"SELECT o.id AS ID, o.address as Address,o.approve_user as ApproveUser,o.to_go, o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                  o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.card_num as card_num,o.is_approved as IsApproved,o.cancle_status AS cancle_status,o.name as Name,o.num as Num,o.receivers_count as ReceiversCount
                                  ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId FROM dbo.Cancellation AS o where " + where + " ").ToList(),
                    users = _db.Database.SqlQuery<CallUser>(@"SELECT * FROM book.Users where type=4 or type=44").ToList()
                };
            }
        }
        public StaticCount UserStatic(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                DateTime datefrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 01, 0);
                return new StaticCount
                {

                    order_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and get_date<='" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    order_yielding = _db.Database.SqlQuery<int>(@"select COUNT(c.id) from book.Cards as c 
								left join book.Customers as a on a.id = c.customer_id
								left join book.Users as u on c.user_id = u.id
								left join dbo.SellerObject as so on u.object = so.ID
								left join book.UserTypes as ut on u.type = ut.id where  (ut.id = 4 or ut.id=44) AND u.id=" + user_id + " AND c.tdate BETWEEN @date_from AND @date_to", new SqlParameter("date_from", datefrom), new SqlParameter("date_to", DateTime.Now)).FirstOrDefault(),
                    order_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and get_date<='" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)OrderStatus.Closed + "and status!=" + (int)OrderStatus.Canceled + " and o.is_approved=0").FirstOrDefault(),
                    order_cancled = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)OrderStatus.Canceled + "").FirstOrDefault(),

                    damage_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    damage_yielding = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)DamageStatus.Closed + "").FirstOrDefault(),
                    damage_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)DamageStatus.Closed + " and is_approved=0").FirstOrDefault(),

                    cancel_count = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "'").FirstOrDefault(),
                    cancel_yielding = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)CancleStatus.Closed + "").FirstOrDefault(),
                    cancel_remainder = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status!=" + (int)CancleStatus.Closed + "and status!=" + (int)CancleStatus.NotClosed + " and o.is_approved=0").FirstOrDefault(),
                    cancel_cancled = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.executor_id = " + user_id + " and change_date between '" + datefrom.ToString("MM / dd / yyyy HH:mm:ss") + "' and '" + DateTime.Now.ToString("MM / dd / yyyy HH:mm:ss") + "' and status=" + (int)CancleStatus.NotClosed + "").FirstOrDefault(),

                    OrderCount = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM doc.Orders as o where o.status=0 ").FirstOrDefault(),
                    DamageCount = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Damage as o where o.status =11").FirstOrDefault(),
                    CancellationCount = _db.Database.SqlQuery<int>("SELECt COUNT(o.id) FROM dbo.Cancellation as o where o.status =4 ").FirstOrDefault()
                };
            }

        }
        public int OrderToGo(int user_id)
        {
            using (DataContext _db=new DataContext())
            {
                return _db.Database.SqlQuery<int>($"SELECT o.to_go FROM doc.Orders as o where executor_id={user_id} and to_go=1").FirstOrDefault();
            }
                
        }
        public int DamageToGo(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<int>($"SELECT d.to_go FROM  dbo.Damage as d where executor_id={user_id} and to_go=1").FirstOrDefault();
            }

        }
        public int CancellationToGo(int user_id)
        {
            using (DataContext _db = new DataContext())
            {
                return _db.Database.SqlQuery<int>($"SELECT c.to_go FROM  dbo.Cancellation as c where executor_id={user_id} and to_go=1").FirstOrDefault();
            }

        }
    }
}