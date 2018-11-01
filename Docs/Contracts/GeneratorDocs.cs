using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Docs.Contracts
{
    public class GeneratorDocs
    {
        private readonly RenderViewString renderViewString;
        private readonly string name;

        public GeneratorDocs(RenderViewString renderViewString,string Name) {
            this.renderViewString = renderViewString;
            this.name = Name;
        }

        public ActionResult Result()
        {
            string htmlString = renderViewString.Result();
            string baseUrl = "";

            string pdf_page_size = "A4";
            SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            SelectPdf.PdfPageOrientation pdfOrientation =
                (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                pdf_orientation, true);

            string pdf_align = "Justify";
            SelectPdf.PdfTextHorizontalAlign pdfTextAlign =
                (SelectPdf.PdfTextHorizontalAlign)Enum.Parse(typeof(SelectPdf.PdfTextHorizontalAlign),
                pdf_align, true);
       
            int webPageWidth = 1024;


            int webPageHeight = 0;


            // instantiate a html to pdf converter object
            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;


            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            SelectPdf.PdfFont font = doc.AddFont(SelectPdf.PdfStandardFont.Helvetica);
            //font.Size =7;
            //doc.AddFont(new System.Drawing.Font("Verdana", 20));
            // save pdf document
            var FolderName = (@"C:\Xelshekruleba\"+name.ToString());
            if (!Directory.Exists(FolderName))

            {

                Directory.CreateDirectory(FolderName);

            }

            var filePath = FolderName + '\\' + ""+ name + ".pdf";
            doc.Save(filePath);
            //System.Diagnostics.Process.Start(filePath);

            return null;
        }
    }

}