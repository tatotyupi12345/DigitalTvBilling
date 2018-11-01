using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter
{
    public class CallLogic
    {
        public CallLogic()
        {

        }

        public CallModel ReturnResult()
        {

            CallData callData = new CallData();
        
            return new CallModel
            {
                users = callData.ReturnCallData().users.Select(s => new CallUser
                {
                    Id = s.Id,
                    Name = s.Name,
                    Phone = s.Phone,
                    Password = s.Password,
                    CodeWord = s.CodeWord,
                    HardAutorize = s.HardAutorize,
                    Logging = s.Logging,
                    Login = s.Login,
                    SellerObj = s.SellerObj,
                    Type = s.Type,
                    TypeName = s.TypeName,
                    UserType = s.UserType,
                    image = s.image,
                    Email = s.Email,
                    StaticCounts = callData.UserStatic(s.Id),
                    start_end=s.start_end,
                    //OrderToGo=callData.OrderToGo(s.Id),
                    //DamageGoTo=callData.DamageToGo(s.Id),
                    //CancellationToGo=callData.CancellationToGo(s.Id)

                }).ToList()
            };
            //return user_static;
        }
        public CallModel ReturnOrderResult(FilterUser filterUser)
        {
            CallData callData = new CallData();
            var where = "";
            if (filterUser.check_user == true)
            {
                where = " (" + dateFromTo() + " and executor_id=" + filterUser.user_id + ") or (change_date='2222-12-12 00:00:00.000') ";
            }
            else
            {
                where = "" + dateFromTo() + " and executor_id=" + filterUser.user_id;
            }
            return callData.ReturnedOrder(where);
        }

        public CallModel ReturnDamageResult(FilterUser filterUser)
        {
            CallData callData = new CallData();
            var where = "";
            if (filterUser.check_user == true)
            {
                where = " (" + dateFromTo() + " and executor_id=" + filterUser.user_id + ") or (change_date='2222-12-12 00:00:00.000') ";
            }
            else
            {
                where = "" + dateFromTo() + " and executor_id=" + filterUser.user_id;
            }
            return callData.ReturnedDamage(where);
        }

        public CallModel ReturnCancellationResult(FilterUser filterUser)
        {
            CallData callData = new CallData();
            var where = "";
            if (filterUser.check_user == true)
            {
                where = " (" + dateFromTo() + " and executor_id=" + filterUser.user_id + ") or ( status=4 ) ";
            }
            else
            {
                where = "" + dateFromTo() + " and executor_id=" + filterUser.user_id;
            }
            return callData.ReturnedCancellation(where);
        }
        public string dateFromTo()
        {
            DateTime dfrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime dTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            
            return "change_date between '"+dfrom+"' and '"+dTo+ "'";
        }
    }
}