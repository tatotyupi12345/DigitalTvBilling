using DigitalTVBilling.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Juridical
{
    public class JuridicalModel
    {
        public IPagedList<JuridicalResult> juridicalLists { get; set; }
        public int count { get; set; }
        public string drpfiltet { get; set; }
        public string filterText { get; set; }
        public int totalItemsCount { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public List<JuridicalStatus> JuridicalStatus { get; set; }

    }
    public class ReturnJson
    {
        public List<int> Status { get; set; }
        public int ID { get; set; }
    }
    public class StatusInfo
    {
        public int id { get; set; }
        public int [] statusArray { get; set; }
        public string _status { get; set; }
    }
    public class CardNumID
    {
        public int id { get; set; }
        public string card_num { get; set; }
    }
    public class JuridicalResult
    {
        public int Id { get; set; }
        public DateTime Tdate { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Abonent_Num { get; set; }
        public string UsName { get; set; }
        public string UsType { get; set; }
        public int JuridicalVerifical { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Num { get; set; }
        public string CardNum { get; set; }
        public string DocNum { get; set; }
        public string ActivePacket { get; set; }
        public CardStatus Status { get; set; }
        public int user_id { get; set; }
        public string signature { get; set; }
    }
    public class JuridicalFilters
    {
        public JuridicalFilters()
        {
            this.page = 1;
        }
        public string name { get; set; }
        public string dt_from { get; set; }
        public string dt_to { get; set; }
        public string drp_filter { get; set; }
        public bool? j_checked { get; set; }
        public string status { get; set; }
        public int page { get; set; }
    }
    public class JuridicalWhere
        {
        public JuridicalWhere()
        {
            this.pageSize = 20;
        }
            public int page { get; set; }
            public int pageSize { get; set; }
            public string _filter { get; set; }
            public string where { get; set; }
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }
        }
   
}