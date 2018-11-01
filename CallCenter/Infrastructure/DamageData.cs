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
    public class DamageData :IUserDamage
    {
        private readonly IDbConnection db;
        private readonly ISearchResult search;

        public DamageData(IDbConnection db, ISearchResult search)
        {
            this.db = db;
            this.search = search;
        }
        public IEnumerable<Damage> Execute()
        {

            return db.Query<Damage>(@"SELECT o.id AS ID, o.address as Address,o.comment AS comment,o.approve_user as ApproveUser,o.to_go,o.card_address as CardAddress,o.change_date as ChangeDate,o.changer_user as ChangerUser,o.code, Code,
                                  o.data as Data,o.executor_id as ExecutorID,o.get_date as GetDate,o.is_approved as IsApproved,o.montage_status as MontageStatus,o.name as Name,o.num as Num,o.montage_user_id as montage_user_id,o.comment as comment,o.receivers_count as ReceiversCount
                                  ,o.tdate as Tdate,o.status as Status,o.user_group_id as UserGroupId,o.user_id as UserId FROM dbo.Damage AS o where " + search.Result() + " ").ToList();
        }
    }
}