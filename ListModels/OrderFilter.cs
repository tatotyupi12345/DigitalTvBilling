using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class OrderFilter
    {
        public string data { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public bool montage_status { get; set; }
        public int num { get; set; }
        public DateTime tdate { get; set; }
        public DateTime get_date { get; set; }
        public DateTime change_date { get; set; }
        public int receivers_count { get; set; }
        public OrderStatus status { get; set; }
        public string changer_user { get; set; }
        public string approve_user { get; set; }
        public string create_user { get; set; }
        public string group_name { get; set; }
        public bool is_approved { get; set; }
        public string reasons { get; set; }
        public int executor_id { get; set; }
        public string exec_name { get; set; }
        public int to_go { get; set; }
        public string comment { get; set; }
        public string InstalatorName { get; set; }
    }
}