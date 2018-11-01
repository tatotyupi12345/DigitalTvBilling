using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace DigitalTVBilling
{
    public class InvoicePdf
    {
        public InvoicePrint InvoiceData { get; set; }

        public string Generate()
        {
            int index = 1;
            XElement element = new XElement("root",
                new XElement("start_day", InvoiceData.StartDate.ToString("dd", CultureInfo.InvariantCulture)),
                new XElement("start_month", Utils.Utils.GetMonthText(InvoiceData.StartDate.Month)),
                new XElement("start_year", InvoiceData.StartDate.ToString("yyyy", CultureInfo.InvariantCulture) + "წ"),
                new XElement("end_day", InvoiceData.EndDate.ToString("dd", CultureInfo.InvariantCulture)),
                new XElement("end_month", Utils.Utils.GetMonthText(InvoiceData.EndDate.Month)),
                new XElement("end_year", InvoiceData.EndDate.ToString("yyyy", CultureInfo.InvariantCulture) + "წ"),
                new XElement("num", InvoiceData.Num),
                new XElement("abonent_code", InvoiceData.AbonentCode),
                new XElement("abonent_name", InvoiceData.AbonentName),
                new XElement("abonent_address", InvoiceData.AbonentAddress),
                new XElement("abonent_telephone", InvoiceData.AbonentPhone),
                new XElement("summ", InvoiceData.Items.Sum(i => i.Amount)),
                new XElement("amount_letter", NumberToString.GetNumberGeorgianString((decimal)InvoiceData.Items.Sum(i => i.Amount))),
                new XElement("items", InvoiceData.Items.Select(it => new XElement("item",
                    new XElement("loop", index++),
                    new XElement("product", it.Name),
                    new XElement("amount", it.Amount)))));

            string file_name = GetTempFilePathWithExtension("Invoice_" + InvoiceData.AbonentCode + "_" + InvoiceData.StartDate.ToString("ddMMyyyyHHmmss"));

            try
            {
                XslCompiledTransform _transform = new XslCompiledTransform(false);
                _transform.Load(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "Invoice.xslt"));

                using (StringWriter sw = new StringWriter())
                {
                    _transform.Transform(new XDocument(element).CreateNavigator(), new XsltArgumentList(), sw);

                    using (PdfDocument pdf = new PdfDocument())
                    {
                        PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat() { IsWaiting = false };
                        PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4 };

                        Thread thread = new Thread(() => { pdf.LoadFromHTML(sw.ToString(), false, setting, htmlLayoutFormat); });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();

                        pdf.SaveToFile(file_name, FileFormat.PDF);
                    }
                }
            }
            catch
            {
                file_name = string.Empty;
            }

            return file_name;
        }

        private string GetTempFilePathWithExtension(string file_name)
        {
            var path = Path.GetTempPath();
            var fileName = file_name + ".pdf";
            return Path.Combine(path, fileName);
        }


    }
}