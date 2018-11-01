using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Docs.JuridicalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class DocummentAttachmentModel
    {
        private readonly AttachmentContractModel docsModel;
        private readonly IConvertImage img;

        public DocummentAttachmentModel(AttachmentContractModel docsModel, IConvertImage img)
        {
            this.docsModel = docsModel;
            this.img = img;
        }
        public List<AttachmentContractModel> ResultImage()
        {
            try
            {
                List<AttachmentContractModel> abonentDocs = new List<AttachmentContractModel>();

                docsModel.AttachmentSignature = img.Result();

                abonentDocs.Add(docsModel);
                return abonentDocs;
            }
            catch
            {
                return null;
            }
        }
    }
}