using DigitalTVBilling.Infrastructure.Juridical.JuridicalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class DocsAttachment : DocumentType
    {
        private readonly string type;
        private readonly string pack_Name;

        public DocsAttachment(string type, string pack_name)
        {
            this.type = type;
            pack_Name = pack_name;
        }
        public string ReturnDocumentType()
        {
            return $"~/Views/Juridical/Contracts/danarti.cshtml";
        }
    }
}