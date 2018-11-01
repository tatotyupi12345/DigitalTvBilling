using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.DocsModel
{
    public class AttachmentContractModel
    {
        public string DirectorSignature { get; set; }
        public string AttachmentSignature { get; set; }
        public int Reciver { get; set; }
        public int Temporaryse { get; set; }
        public List<SellAttachment> attachments { get; set; }
    }
}