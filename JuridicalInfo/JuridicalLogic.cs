using DigitalTVBilling.JuridicalInfo;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Juridical
{
    public class JuridicalLogic
    {
        //public JuridicalLogic() { }

        public async System.Threading.Tasks.Task<JuridicalModel> ReturnResult(JuridicalFilters juridicalFil)
        {
            JuridicalData juridicalData = new JuridicalData();
            return await juridicalData.ReturnResultJuridical(juridicalFil, ReturnDate(juridicalFil));
        }
        public ReturnJson SaveStatusLogic(StatusInfo statusInfo, int user_id)
        {

            JuridicalData juridicalStatus = new JuridicalData();
            var card = juridicalStatus.CardInfoData(statusInfo.id);
            if (card != null)
            {
                juridicalStatus.DeleteStatus(statusInfo);
            }
            if (statusInfo.statusArray.Length == 1)
            {
                juridicalStatus.UpdateCardVerifuStatus(statusInfo);
            }
            using (DataContext _Db = new DataContext())
            {
                statusInfo.statusArray.Select(st =>  
                    new SaveStatusInfos(statusInfo.id, user_id, st, _Db).
                        Result()).Select(sl => 
                            new SaveLoggings(statusInfo.id, user_id, 1, _Db).
                                Result()
                ).ToList();
            }
            return juridicalStatus.SaveLogging(statusInfo, user_id, card);
        }

        public List<JuridicalStatus> StatusInfoLogic(int card_id)
        {
            JuridicalData data = new JuridicalData();
            return data.ReturnStatusInfo(card_id);
        }
        public List<JuridicalLogging> JuridicalLoggingsLogic(int card_id)
        {
            JuridicalData data = new JuridicalData();
            return data.juridicalLoggings(card_id);
        }
        public string ReturnWhere(JuridicalFilters filter)
        {
            string where = "";
            if (filter.name != null)
            {
                where = "and " + filter.drp_filter + " LIKE N'%" + filter.name + "%'";
                if (filter.drp_filter == "cr.status" || filter.drp_filter == "cr.tower_id" || filter.drp_filter == "c.type")
                    where = filter.drp_filter + "=" + filter.name;
                else if (filter.drp_filter == "c.lastname c.name")
                {
                    filter.drp_filter = "c.lastname+c.name";
                    where = "and " + filter.drp_filter + " LIKE N'%" + filter.name + "%'";
                }
                where = where.Replace("+", "+' '+");

            }
            if (filter.status != null && filter.status != "")
            {
                if (filter.j_checked == false || filter.j_checked == null)
                {
                    if (filter.status == "-1")
                    {
                        where = where + "and cr.juridical_verify_status LIKE '%" + filter.status + "%' and  cr.juridical_verification LIKE '%" + filter.status + "%'";
                    }
                    else
                    {
                        where = where + " and cr.juridical_verify_status='" + filter.status + "'";
                    }
                }
                else
                {
                    where = where + "and cr.juridical_verify_status LIKE '%" + filter.status + "%'";
                }
            }
            return where;
        }
        public JuridicalWhere ReturnDate(JuridicalFilters date_where)
        {
            return new JuridicalWhere
            {
                dateFrom = Utils.Utils.GetRequestDate(date_where.dt_from, true),
                dateTo = Utils.Utils.GetRequestDate(date_where.dt_to, false),
                where = ReturnWhere(date_where),
                _filter = date_where.drp_filter,
                page = date_where.page
            };
        }
    }
}