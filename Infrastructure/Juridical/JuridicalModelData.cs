using Dapper;
using DigitalTVBilling.Juridical;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.Infrastructure.Juridical
{
    public class JuridicalModelData
    {
        private readonly IDbConnection db;
        private readonly FilterData filterData;
        private readonly JuridicalFilter juridicalWhereQuery;

        public JuridicalModelData(IDbConnection db, FilterData filterData, JuridicalFilter juridicalWhere)
        {
            this.db = db;
            this.filterData = filterData;
            this.juridicalWhereQuery = juridicalWhere;
        }
        public int Count()
        {
            return  db.Query<int>(filterData.FilterCount()).FirstOrDefault();
        }
        public async Task<IPagedList<JuridicalResult>> JuridicalViewData()
        {
            return  db.Query<JuridicalResult>(filterData.FilterInfo()).ToPagedList(juridicalWhereQuery.Result().page, juridicalWhereQuery.Result().pageSize);
        }
    }
}