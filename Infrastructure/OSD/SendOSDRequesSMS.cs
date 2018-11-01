using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Infrastructure.OSD
{
    interface IOSDRequest
    {
        void SendOSD(string CardNum, string messageText_Geo, List<Param> ParamList);
    }
    public class SendOSDRequesSMS :IOSDRequest
    {
        //private readonly string cardNum;
        //private readonly string messageText_Geo;
        //private readonly List<Param> paramList;

        public SendOSDRequesSMS()
        {
            //cardNum = CardNum;
            //this.messageText_Geo = messageText_Geo;
            //paramList = ParamList;
        }

        public void SendOSD(string CardNum, string messageText_Geo, List<Param> ParamList)
        {
            int osd_duration = int.Parse(ParamList.First(c => c.Name == "OSDDuration").Value);
            string[] address = ParamList.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
            _socket.Connect();

            if (_socket.SendOSDRequest(int.Parse(CardNum), messageText_Geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), 0))
            {

            }

            _socket.Disconnect();
        }
    }
}