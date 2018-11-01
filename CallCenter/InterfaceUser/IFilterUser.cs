using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTVBilling.CallCenter.InterfaceUser
{
    public interface IFilterUser
    {
        int user_id { get; set; }
        bool check_user { get; set; }
    }
}
