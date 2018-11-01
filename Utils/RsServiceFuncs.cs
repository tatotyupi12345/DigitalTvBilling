using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace DigitalTVBilling.Utils
{
    public class RsServiceFuncs
    {
        string _path = HttpContext.Current.Server.MapPath("~/App_Data");
        private string _rs_url = "http://services.rs.ge/WayBillService/WayBillService.asmx";
        private string _rs_user = string.Empty;
        private string _rs_password = string.Empty;

        public RsServiceFuncs(bool b)
        {
            if (b)
            {
                XDocument doc = XDocument.Load(Path.Combine(_path, "rs_data.xml"));
                this._rs_user = doc.Root.Element("user").Value;
                this._rs_password = doc.Root.Element("pass").Value;
            }
        }

        public bool UpdateRsUser()
        {
            
            XDocument res = this.CallRsService("get_service_users", "<user_name>tbilisi</user_name><user_password>123456</user_password>");
            if (res != null)
            {
                XElement el = res.Descendants("ServiceUser").FirstOrDefault();
                if (el != null)
                {
                    string user = el.Element("USER_NAME").Value;
                    string name = el.Element("NAME").Value;
                    string ip = this.CallRsService("what_is_my_ip", "").Root.Value;
                    string is_update = this.CallRsService("update_service_user", "<user_name>tbilisi</user_name><user_password>123456</user_password><ip>" + ip + "</ip><name>" + name + "</name><su>" + user + "</su><sp>globaltv</sp>").Root.Value;
                    if (is_update == "true")
                    {
                        string file_path = Path.Combine(_path, "rs_data.xml");
                        XDocument doc = XDocument.Load(file_path);
                        doc.Root.Element("user").SetValue(user);
                        doc.Root.Element("pass").SetValue("globaltv");
                        doc.Save(file_path);

                        return true;
                    }
                }
            }

            return false;
        }

        public string GetAbonentName(string code)
        {
            XDocument doc = XDocument.Load(Path.Combine(_path, "rs_data.xml"));
            this._rs_user = doc.Root.Element("user").Value;
            this._rs_password = doc.Root.Element("pass").Value;

            XDocument res = new RsServiceFuncs(true).CallRsService("get_name_from_tin", "<su>" + this._rs_user + "</su><sp>" + this._rs_password + "</sp><tin>" + code + "</tin>");
            return res.Root.Value;
        }

        private XDocument CreateSoapEnvelope(string action, string data)
        {
            string str = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                   "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                   "<soap:Body><" + action + " xmlns=\"http://tempuri.org/\">" + data + "</" + action + "></soap:Body></soap:Envelope>";

           return XDocument.Parse(str);
        }

        public XDocument CallRsService(string soap_action,string soap_data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_rs_url);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Accept = "text/xml";
            request.Headers.Add("SOAPAction","http://tempuri.org/" + soap_action);
            using (Stream stream = request.GetRequestStream())
            {
                CreateSoapEnvelope(soap_action, soap_data).Save(stream);
            }

            IAsyncResult asyncResult = request.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = request.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }
            return XDocument.Parse(soapResult);
        }


    }
}