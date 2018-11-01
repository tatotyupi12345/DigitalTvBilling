using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.DocsModel
{
    public class AbonentDocsModel
    {
        public string doc_num { get; set; }
        public DateTime tdate { get; set; }
        public string abonent { get; set; }
        public string user_name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string abonent_num { get; set; }
        public string abonent_signature { get; set; }
        public string signature { get; set; }
        public string signature_attachment { get; set; }
    }
}