using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class PackagesChargesList
    {
        public int Card_Id { get; set; }
        public DateTime Tdate { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Packages { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public int user_id { get; set; }
        public string City { get; set; }
        public PackagesChargesType ChargesType { get; set; }
        public int verify_status { get; set; }
    }
    public enum PackagesChargesType
    {
        [Description("პაკეტის ცვლილება 8-დან 15-ზე")]
        CardPackageCharges,
        [Description("ბარათი დაპაუზდა")]
        CardPaused,
        [Description("პაკეტის ცვლილება 8-დან 12-ზე")]
        PackAgesCharges12,
        [Description("პაკეტის ცვლილება 8-დან 6-ზე")]
        PackAgesCharges6,
        [Description("პაკეტის ცვლილება პრომო-დან 15-ზე")]
        CardPackageChargesPromo15,
        [Description("პაკეტის ცვლილება პრომო-დან 8-ზე")]
        CardPackageChargesPromo8
    }
}