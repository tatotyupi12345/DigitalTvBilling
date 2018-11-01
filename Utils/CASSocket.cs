using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class CASSocket
    {
        public string ErrorEx { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public Socket NetSocket { get; private set; }
        public int SessionId { get; set; }
        public int error_code { get; set; }

        public int ReceivedDataLength { get; set; }

        public bool Connect()
        {
            this.ErrorEx = string.Empty;
            IPAddress _address = IPAddress.Parse(this.IP);
            try
            {
                NetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 1000 };
                IPEndPoint RemoteEndPoint = new IPEndPoint(_address, this.Port);
                NetSocket.Connect(RemoteEndPoint);
                return true;
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool Disconnect()
        {
            if (this.NetSocket == null || !this.NetSocket.Connected)
                return true;
            try
            {
                this.NetSocket.Disconnect(false);
            }
            catch (Exception ex)
            {
                this.ErrorEx = ex.Message;
                return false;
            }
            return true;

        }

        public bool SendCardStatus(int card_num, bool is_activate, DateTime expire_date)
        {
            byte[] dataSend = new byte[16];

            //Session Id    
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            dataSend[0] = _sessionIdBytes[1];
            dataSend[1] = _sessionIdBytes[0];

            //CAS version
            dataSend[2] = 2;

            //Command type 
            dataSend[3] = 11; //Activate/deactivate the cards

            //Data length
            dataSend[4] = 0;
            dataSend[5] = 10;

            //Card number
            byte[] cardNumBytes = BitConverter.GetBytes(card_num);
            dataSend[6] = cardNumBytes[3];
            dataSend[7] = cardNumBytes[2];
            dataSend[8] = cardNumBytes[1];
            dataSend[9] = cardNumBytes[0];

            //Send or not
            dataSend[10] = Convert.ToByte(true);

            //Card status
            dataSend[11] = Convert.ToByte(is_activate);

            //Expired time
            Int32 unixTimestamp = (Int32)(expire_date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            byte[] expiredByBytes = BitConverter.GetBytes(unixTimestamp);
            dataSend[12] = expiredByBytes[3];
            dataSend[13] = expiredByBytes[2];
            dataSend[14] = expiredByBytes[1];
            dataSend[15] = expiredByBytes[0];

            try
            {
                
                ReceivedDataLength = 11;
                NetSocket.Send(dataSend, 0, dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        private bool SendFake100EntitlementRequest(int card_num, short[] cas_ids, DateTime begin_time, bool is_activate)
        {
            
            int _packageLength = 11 + (13 + 0) * cas_ids.Length;
            byte[] _dataSend = new byte[_packageLength];

            #region header
            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type - Entitle
            _dataSend[3] = 1;

            //Data length
            Int16 _dataLength = Convert.ToInt16(_packageLength - 6);
            byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
            _dataSend[4] = _dataLengthBytes[1];
            _dataSend[5] = _dataLengthBytes[0];
            #endregion

            //--------------------------------------------------------------------
            #region body data
            //Card ID
            byte[] cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = cardNumBytes[3];
            _dataSend[7] = cardNumBytes[2];
            _dataSend[8] = cardNumBytes[1];
            _dataSend[9] = cardNumBytes[0];

            //Product amount
            _dataSend[10] = Convert.ToByte(cas_ids.Length);

            byte _send = Convert.ToByte(true);
            byte _descriptionLength = Convert.ToByte(0);
            byte[] _descriptionBytes = Encoding.ASCII.GetBytes("");
            byte[] productIDBytes;
            Int32 _totalSeconds;
            int _index = 11;

            DateTime BeginTime = is_activate ? begin_time : begin_time.AddYears(20);
            DateTime EndTime = begin_time.AddYears(20);

            short[] _product_ids = cas_ids;
            foreach (Int16 _productID in _product_ids)
            {
                //Send or not
                _dataSend[_index++] = _send;

                //TapingCtrl
                _dataSend[_index++] = 1;

                //Product id
                productIDBytes = BitConverter.GetBytes(_productID);
                _dataSend[_index++] = productIDBytes[1];
                _dataSend[_index++] = productIDBytes[0];

                //Product begin time
                _totalSeconds = (Int32)(BeginTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productBeginDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productBeginDateBytes[3];
                _dataSend[_index++] = productBeginDateBytes[2];
                _dataSend[_index++] = productBeginDateBytes[1];
                _dataSend[_index++] = productBeginDateBytes[0];

                //Product end time
                _totalSeconds = (Int32)(EndTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productEndDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productEndDateBytes[3];
                _dataSend[_index++] = productEndDateBytes[2];
                _dataSend[_index++] = productEndDateBytes[1];
                _dataSend[_index++] = productEndDateBytes[0];

                //The length of description
                _dataSend[_index++] = _descriptionLength;

                //description
                foreach (byte _descriptionByte in _descriptionBytes)
                {
                    _dataSend[_index++] = _descriptionByte;
                }
            }
            #endregion

            try
            {
                ReceivedDataLength = 10 + (3 + 0) * cas_ids.Length;
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool SendEntitlementRequestTemp(int card_num, short[] cas_ids, DateTime begin_time, bool is_activate)
        {
            int _packageLength = 11 + (13 + 0) * cas_ids.Length;
            byte[] _dataSend = new byte[_packageLength];

            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type - Entitle
            _dataSend[3] = 1;

            //Data length
            Int16 _dataLength = Convert.ToInt16(_packageLength - 6);
            byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
            _dataSend[4] = _dataLengthBytes[1];
            _dataSend[5] = _dataLengthBytes[0];

            //Card number
            byte[] cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = cardNumBytes[3];
            _dataSend[7] = cardNumBytes[2];
            _dataSend[8] = cardNumBytes[1];
            _dataSend[9] = cardNumBytes[0];

            //Product amount
            _dataSend[10] = Convert.ToByte(cas_ids.Length);

            byte _send = Convert.ToByte(true);
            byte _descriptionLength = Convert.ToByte(0);
            byte[] _descriptionBytes = Encoding.ASCII.GetBytes("");
            byte[] productIDBytes;
            Int32 _totalSeconds;
            int _index = 11;


            //original code
            DateTime BeginTime = begin_time;
            DateTime EndTime = begin_time;

            //test code
            //DateTime BeginTime = new DateTime(2016, 07, 4);
            //DateTime EndTime = new DateTime(2016, 07, 30);

            short[] _product_ids = cas_ids;
            foreach (Int16 _productID in _product_ids)
            {
                //Send or not
                _dataSend[_index++] = _send;

                //TapingCtrl
                _dataSend[_index++] = 1;

                //Product id
                productIDBytes = BitConverter.GetBytes(_productID);
                _dataSend[_index++] = productIDBytes[1];
                _dataSend[_index++] = productIDBytes[0];

                //Product begin time
                _totalSeconds = (Int32)(BeginTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productBeginDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productBeginDateBytes[3];
                _dataSend[_index++] = productBeginDateBytes[2];
                _dataSend[_index++] = productBeginDateBytes[1];
                _dataSend[_index++] = productBeginDateBytes[0];

                //Product end time
                _totalSeconds = (Int32)(EndTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productEndDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productEndDateBytes[3];
                _dataSend[_index++] = productEndDateBytes[2];
                _dataSend[_index++] = productEndDateBytes[1];
                _dataSend[_index++] = productEndDateBytes[0];

                //The length of description
                _dataSend[_index++] = _descriptionLength;

                //description
                foreach (byte _descriptionByte in _descriptionBytes)
                {
                    _dataSend[_index++] = _descriptionByte;
                }
            }

            try
            {
                ReceivedDataLength = 10 + (3 + 0) * cas_ids.Length;
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool SendEntitlementRequest(int card_num, short[] cas_ids, DateTime begin_time, bool is_activate)
        {
            if (is_activate)
            {
                this.SendFake100EntitlementRequest(card_num, new short[] { 100 }, begin_time, true);
                Thread.Sleep(3000);
                this.SendFake100EntitlementRequest(card_num, new short[] { 100 }, begin_time, false);
                Thread.Sleep(2000);
                this.SendCardStatus(card_num, true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
            }

            int _packageLength = 11 + (13 + 0) * cas_ids.Length;
            byte[] _dataSend = new byte[_packageLength];

            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type - Entitle
            _dataSend[3] = 1;

            //Data length
            Int16 _dataLength = Convert.ToInt16(_packageLength - 6);
            byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
            _dataSend[4] = _dataLengthBytes[1];
            _dataSend[5] = _dataLengthBytes[0];

            //Card number
            byte[] cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = cardNumBytes[3];
            _dataSend[7] = cardNumBytes[2];
            _dataSend[8] = cardNumBytes[1];
            _dataSend[9] = cardNumBytes[0];

            //Product amount
            _dataSend[10] = Convert.ToByte(cas_ids.Length);

            byte _send = Convert.ToByte(true);
            byte _descriptionLength = Convert.ToByte(0);
            byte[] _descriptionBytes = Encoding.ASCII.GetBytes("");
            byte[] productIDBytes;
            Int32 _totalSeconds;
            int _index = 11;

            DateTime BeginTime = is_activate ? begin_time : begin_time.AddYears(20);
            DateTime EndTime = begin_time.AddYears(20);

            short[] _product_ids = cas_ids;
            foreach (Int16 _productID in _product_ids)
            {
                //Send or not
                _dataSend[_index++] = _send;

                //TapingCtrl
                _dataSend[_index++] = 1;

                //Product id
                productIDBytes = BitConverter.GetBytes(_productID);
                _dataSend[_index++] = productIDBytes[1];
                _dataSend[_index++] = productIDBytes[0];

                //Product begin time
                _totalSeconds = (Int32)(BeginTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productBeginDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productBeginDateBytes[3];
                _dataSend[_index++] = productBeginDateBytes[2];
                _dataSend[_index++] = productBeginDateBytes[1];
                _dataSend[_index++] = productBeginDateBytes[0];

                //Product end time
                _totalSeconds = (Int32)(EndTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productEndDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productEndDateBytes[3];
                _dataSend[_index++] = productEndDateBytes[2];
                _dataSend[_index++] = productEndDateBytes[1];
                _dataSend[_index++] = productEndDateBytes[0];

                //The length of description
                _dataSend[_index++] = _descriptionLength;

                //description
                foreach (byte _descriptionByte in _descriptionBytes)
                {
                    _dataSend[_index++] = _descriptionByte;
                }
            }

            try
            {
                ReceivedDataLength = 10 + (3 + 0) * cas_ids.Length;
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool SendEntitlementRequest(int card_num, short[] cas_ids, DateTime begin_time, DateTime end_time, bool is_activate)
        {
            //if (is_activate)
            //{
            //    this.SendFake100EntitlementRequest(card_num, new short[] { 100 }, begin_time, true);
            //    Thread.Sleep(3000);
            //    this.SendFake100EntitlementRequest(card_num, new short[] { 100 }, begin_time, false);
            //    Thread.Sleep(2000);
            //    this.SendCardStatus(card_num, true, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
            //}

            int _packageLength = 11 + (13 + 0) * cas_ids.Length;
            byte[] _dataSend = new byte[_packageLength];

            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type - Entitle
            _dataSend[3] = 1;

            //Data length
            Int16 _dataLength = Convert.ToInt16(_packageLength - 6);
            byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
            _dataSend[4] = _dataLengthBytes[1];
            _dataSend[5] = _dataLengthBytes[0];

            //Card number
            byte[] cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = cardNumBytes[3];
            _dataSend[7] = cardNumBytes[2];
            _dataSend[8] = cardNumBytes[1];
            _dataSend[9] = cardNumBytes[0];

            //Product amount
            _dataSend[10] = Convert.ToByte(cas_ids.Length);

            byte _send = Convert.ToByte(true);
            byte _descriptionLength = Convert.ToByte(0);
            byte[] _descriptionBytes = Encoding.ASCII.GetBytes("");
            byte[] productIDBytes;
            Int32 _totalSeconds;
            int _index = 11;

            DateTime BeginTime = begin_time;// new DateTime(2016, 07, 1).AddHours(3 + 12).AddMinutes(53) ;//begin_time;// new DateTime(2016, 05, 30);// begin_time;//is_activate ? begin_time : begin_time.AddDays(25);
            DateTime EndTime = end_time;// new DateTime(2016, 07, 15);//end_time;// new DateTime(2016, 05, 30);// begin_time.AddDays(25); 

            short[] _product_ids = cas_ids;
            foreach (Int16 _productID in _product_ids)
            {
                //Send or not
                _dataSend[_index++] = _send;

                //TapingCtrl
                _dataSend[_index++] = 1;

                //Product id
                productIDBytes = BitConverter.GetBytes(_productID);
                _dataSend[_index++] = productIDBytes[1];
                _dataSend[_index++] = productIDBytes[0];

                //Product begin time
                _totalSeconds = (Int32)(BeginTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productBeginDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productBeginDateBytes[3];
                _dataSend[_index++] = productBeginDateBytes[2];
                _dataSend[_index++] = productBeginDateBytes[1];
                _dataSend[_index++] = productBeginDateBytes[0];

                //Product end time
                _totalSeconds = (Int32)(EndTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                byte[] productEndDateBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[_index++] = productEndDateBytes[3];
                _dataSend[_index++] = productEndDateBytes[2];
                _dataSend[_index++] = productEndDateBytes[1];
                _dataSend[_index++] = productEndDateBytes[0];

                //The length of description
                _dataSend[_index++] = _descriptionLength;

                //description
                foreach (byte _descriptionByte in _descriptionBytes)
                {
                    _dataSend[_index++] = _descriptionByte;
                }
            }

            try
            {
                ReceivedDataLength = 10 + (3 + 0) * cas_ids.Length;
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                string rcved = this.ReceiveResponse();
                return !rcved.Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;

                return false;
            }
        }

        //public bool SendOSDRequest(int card_num, string message, DateTime date, int duration)
        //{
        //    byte ContentLength = Convert.ToByte(message.Length);
        //    byte[] _dataSend = new byte[19 + ContentLength];

        //    //Session Id
        //    byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
        //    _dataSend[0] = _sessionIdBytes[1];
        //    _dataSend[1] = _sessionIdBytes[0];

        //    //CAS version
        //    _dataSend[2] = 2;

        //    //Command type 
        //    _dataSend[3] = 2; //OSD

        //    //Data length
        //    Int16 _dataLength = Convert.ToInt16(ContentLength + 13);
        //    byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
        //    _dataSend[4] = _dataLengthBytes[1];
        //    _dataSend[5] = _dataLengthBytes[0];

        //    //Card number
        //    byte[] _cardNumBytes = BitConverter.GetBytes(card_num);
        //    _dataSend[6] = _cardNumBytes[3];
        //    _dataSend[7] = _cardNumBytes[2];
        //    _dataSend[8] = _cardNumBytes[1];
        //    _dataSend[9] = _cardNumBytes[0];

        //    //Show Time Len
        //    byte[] _showTimeLenBytes = BitConverter.GetBytes(duration);
        //    _dataSend[10] = _showTimeLenBytes[1];
        //    _dataSend[11] = _showTimeLenBytes[0];

        //    //Show_Times
        //    _dataSend[12] = 1;

        //    //Priority
        //    _dataSend[13] = (int)MessagePrioritet.Low;

        //    //Expired time
        //    Int32 _totalSeconds = (Int32)(date.AddHours(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //    byte[] _expiredByBytes = BitConverter.GetBytes(_totalSeconds);
        //    _dataSend[14] = _expiredByBytes[3];
        //    _dataSend[15] = _expiredByBytes[2];
        //    _dataSend[16] = _expiredByBytes[1];
        //    _dataSend[17] = _expiredByBytes[0];

        //    //Data len
        //    _dataSend[18] = ContentLength;

        //    //Content
        //    byte[] _contentBytes = Encoding.ASCII.GetBytes(message);
        //    int _index = 19;
        //    for (int i = 0; i < ContentLength; i++)
        //    {
        //        _dataSend[_index++] = _contentBytes[i];
        //    }

        //    // send sms to cas
        //    try
        //    {
        //        NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
        //        return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorEx = ex.Message;
        //        return false;
        //    }
        //}


        public bool SendOSDRequest(int card_num, string message, DateTime date, int duration)
        {
            try
            {
                //byte[] bytes = Encoding.Default.GetBytes(message);
                //message = Encoding.UTF8.GetString(bytes);
                byte[] _contentBytes = Encoding.UTF8.GetBytes(message);

                byte ContentLength = Convert.ToByte(_contentBytes.Length);// Convert.ToByte(msg.Length);
                byte[] _dataSend = new byte[19 + ContentLength];

                //Session Id
                byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
                _dataSend[0] = _sessionIdBytes[1];
                _dataSend[1] = _sessionIdBytes[0];

                //CAS version
                _dataSend[2] = 2;

                //Command type 
                _dataSend[3] = 2; //OSD

                //Data length
                Int16 _dataLength = Convert.ToInt16(ContentLength + 13);
                byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
                _dataSend[4] = _dataLengthBytes[1];
                _dataSend[5] = _dataLengthBytes[0];

                //Card number
                byte[] _cardNumBytes = BitConverter.GetBytes(card_num);
                _dataSend[6] = _cardNumBytes[3];
                _dataSend[7] = _cardNumBytes[2];
                _dataSend[8] = _cardNumBytes[1];
                _dataSend[9] = _cardNumBytes[0];

                //Show Time Len
                byte[] _showTimeLenBytes = BitConverter.GetBytes(duration);
                _dataSend[10] = _showTimeLenBytes[1];
                _dataSend[11] = _showTimeLenBytes[0];

                //Show_Times
                _dataSend[12] = 1;

                //Priority
                _dataSend[13] = (int)MessagePrioritet.Low;

                //Expired time
                Int32 _totalSeconds = (Int32)(date.AddHours(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int result = (Int32)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds;
                //Int32 _totalSeconds = (Int32)(date.AddHours(1).Second);
                byte[] _expiredByBytes = BitConverter.GetBytes(_totalSeconds);
                _dataSend[14] = _expiredByBytes[3];
                _dataSend[15] = _expiredByBytes[2];
                _dataSend[16] = _expiredByBytes[1];
                _dataSend[17] = _expiredByBytes[0];

                //Data len
                _dataSend[18] = ContentLength;

                //Content
                //byte[] _contentBytes = Encoding.UTF8.GetBytes(message);

                int _index = 19;
                for (int i = 0; i < _contentBytes.Length; i++)
                {
                    _dataSend[_index++] = _contentBytes[i];
                }

            //Coverage_Rate
            //byte coverageRateBytes = 60;
            //_dataSend[ContentLength] = coverageRateBytes;

            // send sms to cas
            
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool SendPinResetRequest(int card_num, DateTime expired_time)
        {

            byte[] _dataSend = new byte[6 + 8];

            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type 
            _dataSend[3] = 3; //Reset Pin code

            //Data length
            Int16 _dataLength = 8;//Convert.ToInt16(ContentLength + 13);
            byte[] _dataLengthBytes = BitConverter.GetBytes(_dataLength);
            _dataSend[4] = _dataLengthBytes[1];
            _dataSend[5] = _dataLengthBytes[0];

            //Card number
            byte[] _cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = _cardNumBytes[3];
            _dataSend[7] = _cardNumBytes[2];
            _dataSend[8] = _cardNumBytes[1];
            _dataSend[9] = _cardNumBytes[0];

            //Expired time
            Int32 _totalSeconds = (Int32)(expired_time.AddHours(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int result = (Int32)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds;
            //Int32 _totalSeconds = (Int32)(date.AddHours(1).Second);
            byte[] _expiredByBytes = BitConverter.GetBytes(_totalSeconds);
            _dataSend[10] = _expiredByBytes[3];
            _dataSend[11] = _expiredByBytes[2];
            _dataSend[12] = _expiredByBytes[1];
            _dataSend[13] = _expiredByBytes[0];

            try
            {
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        public bool SendEmailRequest(int card_num, string message, DateTime date)
        {
            byte _emailSenderLength = Convert.ToByte("გლობალ ტვ".Length);
            Int16 _emailContentLength = Convert.ToInt16("globaltv".Length);
            int _packageLength = 17 + _emailSenderLength + _emailContentLength;
            Int16 dataLength = Convert.ToInt16(_packageLength - 6);

            byte[] _dataSend = new byte[_packageLength];

            //Session Id
            byte[] _sessionIdBytes = BitConverter.GetBytes(this.SessionId);
            _dataSend[0] = _sessionIdBytes[1];
            _dataSend[1] = _sessionIdBytes[0];

            //CAS version
            _dataSend[2] = 2;

            //Command type 
            _dataSend[3] = 13; //Send email

            //Data length
            byte[] dataLengthBytes = BitConverter.GetBytes(dataLength);
            _dataSend[4] = dataLengthBytes[1];
            _dataSend[5] = dataLengthBytes[0];

            //Card id
            byte[] _cardNumBytes = BitConverter.GetBytes(card_num);
            _dataSend[6] = _cardNumBytes[3];
            _dataSend[7] = _cardNumBytes[2];
            _dataSend[8] = _cardNumBytes[1];
            _dataSend[9] = _cardNumBytes[0];

            //Exp date
            Int32 _totalSeconds = (Int32)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            byte[] bytes = BitConverter.GetBytes(_totalSeconds);

            _dataSend[10] = bytes[3];
            _dataSend[11] = bytes[2];
            _dataSend[12] = bytes[1];
            _dataSend[13] = bytes[0];

            //Sender email length
            _dataSend[14] = _emailSenderLength;

            //Sender email name (depending  on length)
            byte[] _contentBytes = Encoding.ASCII.GetBytes("გლობალ ტვ");
            int _index = 15;
            for (int i = 0; i < _emailSenderLength; i++)
            {
                _dataSend[_index++] = _contentBytes[i];
            }

            //Email content length
            byte[] _emailContentLengthBytes = BitConverter.GetBytes(_emailContentLength);
            _dataSend[_index++] = _emailContentLengthBytes[1];
            _dataSend[_index++] = _emailContentLengthBytes[0];

            byte[] _emailContent = Encoding.ASCII.GetBytes(message);
            for (int i = 0; i < _emailContentLength; i++)
            {
                _dataSend[_index++] = _emailContent[i];
            }
            try
            {
                NetSocket.Send(_dataSend, 0, _dataSend.Length, SocketFlags.None);
                return !this.ReceiveResponse().Trim().Split(' ').Skip(6).Any(c => c != "00");
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return false;
            }
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2} ", b);
            return hex.ToString();
        }
        private string ReceiveResponse()
        {
            ErrorEx = string.Empty;
            byte[] buffer = new byte[ReceivedDataLength];
            byte[] error_bytes = new byte[4];
            try
            {
                int iRx = NetSocket.Receive(buffer);
                error_bytes[0] = buffer[ReceivedDataLength - 1];
                error_bytes[1] = buffer[ReceivedDataLength - 2];
                error_bytes[2] = buffer[ReceivedDataLength - 3];
                error_bytes[3] = buffer[ReceivedDataLength - 4];
                this.error_code = BitConverter.ToInt32(error_bytes, 0);
                return ByteArrayToString(buffer);
            }
            catch (Exception ex)
            {
                ErrorEx = ex.Message;
                return ErrorEx;
            }
        }


    }

    public enum MessagePrioritet
    {
        Low,
        Medium,
        High,
        Highest
    }
}