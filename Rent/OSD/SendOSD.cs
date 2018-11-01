using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace DigitalTVBilling.Rent.OSD
{
    public class SendOSD
    {
        public SendOSD()
        {

        }
        public void SendOSDCard(DataContext _db, CardDetailData _card)
        {
            var _params = _db.Params.ToList();
            string[] address = _params.Where(c => c.Name == "CASAddress").Select(c => c.Value).First().Split(':');
            CASSocket _socket = new CASSocket() { IP = address[0], Port = int.Parse(address[1]) };
            _socket.Connect();

            if (!_socket.SendEntitlementRequest(Convert.ToInt32(_card.Card.CardNum), new short[1] { 9 },DateTime.Now.AddHours(-4), _card.Card.RentFinishDate.AddHours(-4), true))
            {
                //throw new Exception(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss" + "ვერ მოხერხდა ბარათის სტატუსის შეცვლა: " + Utils.Utils.GetEnumDescription(_card.Card.CardStatus) + " , " + _card.Card.AbonentNum));
            }
            _socket.Disconnect();
        }
        public void SendLock(int CardNum)
        {
            for (int i = 21; i <= 65; i++)
            {

                if (i != 21 && i != 22 && i != 26 && i != 27 && i != 28 && i != 30 && i != 32 && i != 33 && i != 36 && i != 41 && i != 43 && i != 47)
                {
                    CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                    _socket.Connect();

                    if (!_socket.SendOSDRequest(int.Parse("63698151"), LockOSD(i), DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), 0))
                    {

                    }
                    _socket.Disconnect();
                    Thread.Sleep(27000);
                }
               
            }


        }
        public void SendUnLock(int CardNum)
        {


            for (int i = 21; i <= 65; i++)
            {
                if (i != 21 && i != 22 && i != 26 && i != 27 && i != 28 && i != 30 && i != 32 && i != 33 && i != 36 && i != 41 && i != 43 &&  i != 47)
                {
                    CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                    _socket.Connect();
                    if (!_socket.SendOSDRequest(int.Parse("63698151"), UnLockOSD(i), DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), 1))
                    {

                    }
                    _socket.Disconnect();
                    Thread.Sleep(27000);
                }
               

            }
           
        }
        public void SendUnlock()
        {

        }
        public string LockOSD(int i)
        {
            return $"LOCK#66{i}#";
        }
        public string UnLockOSD(int i)
        {
            return $"LOCK#76{i}#";
        }
    }
}