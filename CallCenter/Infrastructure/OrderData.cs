using Dapper;
using DigitalTVBilling.CallCenter.InterfaceUser;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter.Infrastructure
{
    public class OrderData : IUserOrder
    {
        private readonly IDbConnection _db;
        private readonly string where;

        public OrderData(IDbConnection _Db,string where)
        {
            this._db = _Db;
            this.where = where;
        }
        public IEnumerable<Order> Execute()
        {
            return _db.Query<Order>(@"SELECT o.id AS ID, o.address as Address,o.approve_user as ApproveUser,o.comment AS comment,o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                     o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.is_approved as IsApproved,o.montage_status as MontageStatus,o.name as Name,o.num as Num,o.poll as Poll,o.receivers_count as ReceiversCount
                                    ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId ,o.to_go FROM doc.Orders AS o where " + where + " ").ToList();
        }
    }
}