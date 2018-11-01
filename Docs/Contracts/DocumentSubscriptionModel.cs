using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Docs.JuridicalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class DocumentSubscriptionModel 
    {
        private readonly AbonentDocsModel docsModel;
        private readonly IConvertImage img;

        public DocumentSubscriptionModel(AbonentDocsModel docsModel, IConvertImage img)
        {
            this.docsModel = docsModel;
            this.img = img;
        }
        public List<AbonentDocsModel> ResultImage()
        {
            try
            {
                List<AbonentDocsModel> abonentDocs = new List<AbonentDocsModel>();

                docsModel.signature = img.Result();

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