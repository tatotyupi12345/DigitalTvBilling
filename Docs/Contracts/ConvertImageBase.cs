using DigitalTVBilling.Docs.DocsModel;
using DigitalTVBilling.Docs.JuridicalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Docs.Contracts
{
    public class ConvertImageBase :IConvertImage
    {
        private readonly string docsModel;

        public ConvertImageBase(string docsModel)
        {
            this.docsModel = docsModel;
        }
        public string Result()
        {
            try
            {

                string convert = docsModel.Replace("data:image/png;base64,", String.Empty);

                byte[] image64 = Convert.FromBase64String(convert);

                var base64s = Convert.ToBase64String(image64);

                var imgSrcs = String.Format("data:image/jpg;base64,{0}", base64s);      
                
                return imgSrcs;
            }
            catch
            {
                return "";
            }
        }
    }
}