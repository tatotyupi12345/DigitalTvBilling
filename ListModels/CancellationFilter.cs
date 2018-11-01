using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class CancellationFilter
    {
        public int id { get; set; }
        public DateTime tdate { get; set; }
        public int num { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string card_address { get; set; }
        public string approve_user { get; set; }
        public bool is_approved { get; set; }
        public string data { get; set; }
        public int receivers_count { get; set; }
        public DateTime get_date { get; set; }
        public DateTime change_date { get; set; }
        public CancleStatus status { get; set; }
        public int cancle_status { get; set; }
        public string changer_user { get; set; }
        public int user_id { get; set; }
        public User UserId { get; set; }
        public int user_group_id { get; set; }
        public int card_num { get; set; }
        public int executor_id { get; set; }
        public string exec_name { get; set; }
        public User UserGroupId { get; set; }
        public Customer Customer { get; set; }
    }
}