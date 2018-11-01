using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class UserDetails
    {
        public User user { get; set; }
        public UserType userType { get; set; }
        public SellerObject sellerObj { get; set; }
        public SellAttachment attach { get; set; }
    }
}