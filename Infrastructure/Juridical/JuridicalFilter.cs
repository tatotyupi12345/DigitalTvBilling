using DigitalTVBilling.Juridical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalFilter
    {
        private readonly JuridicalFilters date_Where;
        private readonly JuridicalWhereInfo whereInfo;

        public JuridicalFilter(JuridicalFilters date_where, JuridicalWhereInfo whereInfo) {
            date_Where = date_where;
            this.whereInfo = whereInfo;
        }

        public JuridicalWhere Result()
        {
            return new JuridicalWhere
            {
                dateFrom = Utils.Utils.GetRequestDate(date_Where.dt_from, true),
                dateTo = Utils.Utils.GetRequestDate(date_Where.dt_to, false),
                where = whereInfo.Result(),
                _filter = date_Where.drp_filter,
                page = date_Where.page
            };
        }
    }
}