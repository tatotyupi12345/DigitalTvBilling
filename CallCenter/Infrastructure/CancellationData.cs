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
    public class CancellationData : IUserCancellation
    {
        private readonly IDbConnection db;
        private readonly CancellationFileter fileter;

        public CancellationData(IDbConnection db, CancellationFileter fileter)
        {
            this.db = db;
            this.fileter = fileter;
        }
        public IEnumerable<Cancellation> Execute()
        {
            return db.Query<Cancellation>(@"SELECT o.id AS ID, o.address as Address,o.approve_user as ApproveUser,o.comment AS comment,o.to_go, o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                  o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.card_num as card_num,o.is_approved as IsApproved,o.cancle_status AS cancle_status,o.name as Name,o.num as Num,o.receivers_count as ReceiversCount
                                  ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId FROM dbo.Cancellation AS o where " + fileter.Execute() + " ").ToList();
        }
    }
}