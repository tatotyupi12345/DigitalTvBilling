using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Xml;

namespace DigitalTVBilling.Utils
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    public class Export
    {
        public byte[] getExcelData(string xslt, XElement element)
        {
            XslCompiledTransform _transform = new XslCompiledTransform(false);
            _transform.Load(HttpContext.Current.Server.MapPath("~/App_Data") + "/" + xslt);
            using (StringWriter sw = new Utf8StringWriter())
            {
                _transform.Transform(new XDocument(element).CreateNavigator(), new XsltArgumentList(), sw);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbook = doc.AddWorkbookPart();
                        WorksheetPart sheet = workbook.AddNewPart<WorksheetPart>();
                        string sheetId = workbook.GetIdOfPart(sheet);

                        // Create a blank XLSX file
                        string XML = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships""><sheets><sheet name=""{1}"" sheetId=""1"" r:id=""{0}"" /></sheets></workbook>";
                        XML = string.Format(XML, sheetId, "Sheet1");
                        this.AddPartXml(workbook, XML);
                        // Insert our sheetData element to the sheet1.xml
                        XML = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" >{0}</worksheet>";
                        XML = string.Format(XML, XDocument.Parse(sw.ToString()).ToString());
                        this.AddPartXml(sheet, XML);

                        doc.Close();

                        return ms.ToArray();
                    }
                }
            }
        }

        protected void AddPartXml(OpenXmlPart part, string xml)
        {
            using (Stream stream = part.GetStream())
            {
                byte[] buffer = (new UTF8Encoding()).GetBytes(xml);
                stream.Write(buffer, 0, buffer.Length);
            }
        }


    }
}