using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class DamageList
    {
        public int Id { get; set; }

        public DateTime Tdate { get; set; }

        public DateTime GetDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public string AbonentName { get; set; }

        public string AbonentCode { get; set; }

        public string AbonentNum { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Village { get; set; }

        public string District { get; set; }

        public string Address { get; set; }

        public string DamageDesc { get; set; }

        public CardDamageStatus Status { get; set; }

        public List<DamageReason> Reasons { get; set; }
        public string Phone { get; set; }
        public string Num { get; set; }
        public string User { get; set; }
        public string GroupUser { get; set; }
        public string ChangeUser { get; set; }

        public string ApproveUser { get; set; }
        
        public bool IsApproved { get; set; }

    }
}