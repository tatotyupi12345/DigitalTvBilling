using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigitalTVBilling.SendSMSFreeInstalation
{
    public class FreeInstalationLogic
    {
        public void ReturnLogicSMS()
        {
            FreeInstalationData freeInstalationData = new FreeInstalationData();

            var message = freeInstalationData.ReturnMessageText();
            var message_geo = freeInstalationData.returnMessage_Geo();
            
           foreach (var cards in freeInstalationData.SendSMSData())
            {
                string messageText = String.Format(message, cards.finish_date.ToString("dd/MM/yyyy"));
                Run(cards.phone1, messageText);
                SendOSD(cards.id.ToString(), message_geo, freeInstalationData.returnParam());
                freeInstalationData.SaveMessageLogging(cards.id);
            }


        }
        public void Run(string phone,string messageText)
        {
            //Task.Run(async () => { await Utils.Utils.sendMessage("598733767", messageText); }).Wait();

            Task.Run(async () => { await Utils.Utils.sendMessage(phone, messageText); }).Wait();
          
        }

        public void SendOSD(string CardNum,string messageText_Geo,List<Param> Params)
        {
           
            int osd_duration = int.Parse(Params.First(c => c.Name == "OSDDuration").Value);
            string[] address = Params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
            _socket.Connect();

            if (_socket.SendOSDRequest(int.Parse(CardNum), messageText_Geo, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), osd_duration))
            {

            }

            _socket.Disconnect();
        }
    }

}