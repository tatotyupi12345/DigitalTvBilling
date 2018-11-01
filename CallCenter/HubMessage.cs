using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DigitalTVBilling.CallCenter
{
    [HubName("hubMessage")]
    public class HubMessage : Hub
    {
        public void Hello()
        {
            //Clients.All.hello();
        }
    }
}