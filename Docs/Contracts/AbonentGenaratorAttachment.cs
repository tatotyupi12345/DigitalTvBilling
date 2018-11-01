using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Docs.JuridicalInterface;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class AbonentGenaratorAttachment : DocumentAttachment
    {
        private readonly Abonent abonent;

        public AbonentGenaratorAttachment(Abonent abonent)
        {
            this.abonent = abonent;
        }

        public AttachmentContractModel Result()
        {
            return new AttachmentContractModel
            {
                DirectorSignature="",
                AttachmentSignature=abonent.Customer.signature_attachment,
                attachments=abonent.attachments,
                 Temporaryse=abonent.Customer.temporary_use
            };
        }
    }
}