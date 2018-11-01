using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTVBilling.CallCenter.InterfaceUser
{
    public interface IUserOrder
    {
        IEnumerable<Order> Execute();
    }
}
