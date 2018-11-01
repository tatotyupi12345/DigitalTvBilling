using DigitalTVBilling.Juridical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalViewModel
    {
        private readonly JuridicalModelData JuridicalData;
        private readonly JuridicalStatusList statusList;
        private readonly JuridicalFilter juridicalFilter;

        public JuridicalViewModel(JuridicalModelData countData,JuridicalStatusList statusList, JuridicalFilters filters) {
            this.JuridicalData = countData;
            this.statusList = statusList;
            Filters = filters;
        }

        public JuridicalFilters Filters { get; }

        public JuridicalModel Result()
        {
            return new JuridicalModel
            {

                JuridicalStatus = statusList.Result(),
                count = JuridicalData.Count(),
                //juridicalLists = JuridicalData.JuridicalViewData(),
                page = Filters.page,
                drpfiltet = Filters.drp_filter,
                filterText = Filters.name,
                totalItemsCount = JuridicalData.Count()
            };
        }
    }
}