using DigitalTVBilling.Juridical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalWhereInfo
    {
        private readonly JuridicalFilters filter;

        public JuridicalWhereInfo(JuridicalFilters juridicalFilters) {
            this.filter = juridicalFilters;
        }

        public string Result()
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
    }
}