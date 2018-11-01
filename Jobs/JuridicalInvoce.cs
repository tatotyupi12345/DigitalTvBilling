using DigitalTVBilling.Controllers;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalTVBilling.Jobs
{
    public class JuridicalInvoce : IJob
    {
        CardDetailData _card;
        public void Execute(IJobExecutionContext context)
        {
            List<int> OneTicketMessed = new List<int>();
            RandomMessed(OneTicketMessed);

            try
            {
                using (DataContext _db = new DataContext())
                {
                    var Invoce_Code = _db.InvoiceLoggings.Select(s => s.invoce_code).ToList();
                    if (Invoce_Code != null) // განმეორებადი კოდის გამორიცხვა
                    {
                        OneTicketMessed = OneTicketMessed.Except(Invoce_Code).ToList();
                    }
                    string _Image = ImageConvert();
                    DateTime dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    //ViewBag.FilePath = _db.Params.Where(p => p.Name == "FTPHost").Select(p => p.Value).First() + "invoce/";
                    var _CustumerI = _db.Customers.Include("Cards").Include("CustomerSellAttachments").Where(c => c.Type == CustomerType.Juridical).ToList();
                    InvoicesList _invoce = new InvoicesList();
                    List<DisruptInvoice> Disrupt = new List<DisruptInvoice>();
                    //Task<CardInfo> _info;

                    foreach (var item in _CustumerI)
                    {

                        decimal balanceSum = 0; bool send_invoce = false;
                        foreach (var card_item in item.Cards)
                        {
                            _card = _db.Cards.Where(c => c.Id == card_item.Id).Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Select(c => new CardDetailData
                            {
                                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                                Card = c,
                                CustomerType = c.Customer.Type,
                                IsBudget = c.Customer.IsBudget,
                                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                                CasIds = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Select(sp => (short)sp.Package.CasId)
                            }).FirstOrDefault();
                            decimal balance = Utils.Utils.GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                            if (/*_info.Balances.Select(s => s.CurrentBalance).LastOrDefault()*/ balance < 0)
                            {
                                DisruptInvoice _disrup_invoice = new DisruptInvoice();
                                _disrup_invoice.Debts = balance;
                                _disrup_invoice.abonent_number = card_item.AbonentNum;
                                _disrup_invoice.Invoices_Code = _card.Card.Customer.Code;
                                _disrup_invoice.CompanyName = item.Name;
                                Disrupt.Add(_disrup_invoice);

                                balanceSum += balance;
                            }
                           
                        }
                        if (balanceSum < 0)
                        {
                            //var ID = item.Cards.Select(s => s.Id).FirstOrDefault();
                            //var pack = _db.Subscribtions.Include("SubscriptionPackages").Where(c => c.CardId == ID).Select(ss => ss).ToList();
                            ////var card_packages = pack.Where(c => c.CardId == ID).SelectMany(s => s.SubscriptionPackages).ToList();
                            JuridicalInvoicesList juridical_invoice = new JuridicalInvoicesList()
                            {
                                Name = item.Name,
                                dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0),
                                dateTo = DateTime.Now,//,
                                _attachment = item.CustomerSellAttachments.Select(s => s).ToList(),
                                balance = Math.Abs(balanceSum),
                                Count = item.Cards.Count(),
                                Invoices_Code = _card.Card.Customer.Code,
                                PackagesPrice = _card.SubscribAmount,
                                Ramdom_Generator = OneTicketMessed[0],
                                Image = _Image,
                                Phone = item.Phone1

                            };
                            var SellAttachments = _db.SellAttachments.ToList();
                            var result = AutoInvoice(juridical_invoice);
                            var _result = DisruptAutoInvoice(Disrupt);
                            Disrupt.Clear();
                            bool send_sms = false;
                            bool send_email = SendMail(item.Email, juridical_invoice.Name, _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Value).FirstOrDefault(), _db.Params.Where(c => c.Name == "SystemEmailPassword").Select(s => s.Value).FirstOrDefault(), juridical_invoice.Invoices_Code);
                            bool _send_email_Accounting = SendMail("ssvansky@mail.ru", juridical_invoice.Name, _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Value).FirstOrDefault(), _db.Params.Where(c => c.Name == "SystemEmailPassword").Select(s => s.Value).FirstOrDefault(), juridical_invoice.Invoices_Code);
                            bool _send_email_aprove = SendMail(_db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Value).FirstOrDefault(), juridical_invoice.Name, _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Value).FirstOrDefault(), _db.Params.Where(c => c.Name == "SystemEmailPassword").Select(s => s.Value).FirstOrDefault(), juridical_invoice.Invoices_Code);
                            bool _send_email_disrupt = Disrupt_SendMail("burdulinato2@gmail.com", juridical_invoice.Name, _db.Params.Where(c => c.Name == "SystemEmail").Select(s => s.Value).FirstOrDefault(), _db.Params.Where(c => c.Name == "SystemEmailPassword").Select(s => s.Value).FirstOrDefault(), juridical_invoice.Invoices_Code);
                            if (item.Email != null)
                            {
                                send_sms = SendSms("568304304");
                                send_sms = SendSms(item.Phone1);
                            }
                            var _invoice = new InvoceLogging
                            {
                                tdate = DateTime.Now,
                                custumer_id = item.Id,
                                name = item.Name,
                                invoce_code = OneTicketMessed[0],
                                send_email = send_email,
                                send_sms = send_sms
                            };
                            _db.InvoiceLoggings.Add(_invoice);
                            _db.SaveChanges();
                            OneTicketMessed.Remove(OneTicketMessed[0]);

                          Thread.Sleep(60000);
                        }
                    }


                }
            }
            catch (Exception ex)
            {

                Utils.Utils.ErrorLogging(ex, @"C:\DigitalTV\InvoiceLog\log.txt");
            }
            
        }

        Random Rad = new Random(); // shemtxveviti ricxvis generatori
        public void RandomMessed(List<int> List_Random)
        {
            List<int> sawyisi = new List<int>();
            sawyisi = Enumerable.Range(1000000, 1999999).OrderBy(x => Rad.Next()).ToList();
            List_Random.AddRange(sawyisi);

        }
        public bool SendSms(string phone)
        {
            try
            {
                Task.Run(async () => { await Utils.Utils.sendMessage(phone, "Digital TV-s angarishze dagericxat davalianeba. invoisis sanaxavad sheamowmet eleqtronuli posta. Gtxovt tanxa charicxot angarishis axal nomerze: GE90TB7843736020100007 "); }).Wait();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendMail(string user_email, string FileName, string SystemEmail, string SystemEmailPassword,string invoice_code)
        {
            try
            {
                if (user_email!=null)
                {
                    string email = SystemEmail;// "tarasamedzmariashvili@gmail.com";
                    string password = SystemEmailPassword;
                    var FolderName = (@"C:\JuridicalInvoice\Invoice\" + DateTime.Now.ToString("dd -MM-yyyy"));
                    var loginInfo = new NetworkCredential(email, password);
                    var msg = new MailMessage();
                    var smtpClient = new SmtpClient("digitaltv.ge", 25);
                    smtpClient.Timeout = 200000;
                    msg.From = new MailAddress(email);
                    msg.To.Add(new MailAddress(user_email)); //user_email
                    msg.Subject =FileName+ " From DigitalTv Invoice N:"+ invoice_code;
                    msg.Body = "მოგესალმებით, <br /> გიგზავნით ინვოისს.  <br /> პატივისცემით, <br /> Kakha Koberidze  <br /> Sales Representative  <br /> mob: +995 599283010 <br /> www.digitaltv.ge";

                    msg.Attachments.Add(new System.Net.Mail.Attachment(FolderName + '\\' + SplitAddresName("DigitalTv Invoice " + invoice_code)));
                    msg.IsBodyHtml = true;
                    smtpClient.EnableSsl = true;
                   
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = loginInfo;
                    smtpClient.Send(msg);
                }
            }
            catch (Exception ex)
            {
                Utils.Utils.ErrorLogging(ex, @"C:\DigitalTV\InvoiceLog\log.txt");
                return false;
                
            }
            return true;
        }
        public bool Disrupt_SendMail(string user_email, string FileName, string SystemEmail, string SystemEmailPassword, string invoice_code)
        {
            try
            {
                if (user_email != null)
                {
                    string email = SystemEmail;// "tarasamedzmariashvili@gmail.com";
                    string password = SystemEmailPassword;// "tyupi123";
                    var FolderName = (@"C:\JuridicalInvoice\DisruptInvoice\" + DateTime.Now.ToString("dd-MM-yyyy"));
                    var loginInfo = new NetworkCredential(email, password);
                    var msg = new MailMessage();
                    var smtpClient = new SmtpClient("digitaltv.ge", 25);
                    smtpClient.Timeout = 200000;
                    msg.From = new MailAddress(email);
                    msg.To.Add(new MailAddress(/*"tato.medzmariashvili@gmail.com"*/user_email)); //user_email
                    msg.Subject = FileName + " From DigitalTv Invoice N:" + invoice_code;
                    msg.Body = "მოგესალმებით, <br /> გიგზავნით ინვოისს.  <br /> პატივისცემით, <br />  www.digitaltv.ge";
                    msg.Attachments.Add(new System.Net.Mail.Attachment(FolderName + '\\' + SplitAddresName("DigitalTv Invoice " + invoice_code)));
                    msg.IsBodyHtml = true;
                    smtpClient.EnableSsl = true;

                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = loginInfo;
                    smtpClient.Send(msg);
                }
            }
            catch (Exception ex)
            {
                Utils.Utils.ErrorLogging(ex, @"C:\DigitalTV\InvoiceLog\log.txt");
                return false;

            }
            return true;
        }
        [ValidateInput(false)]
        public ActionResult AutoInvoice(JuridicalInvoicesList SendInvoice)
        {
            string DocName = "JuridicalInvoce";

            var path = System.IO.Path.GetTempPath();
            var fileName = DocName + ".pdf";
            var var = System.IO.Path.Combine(path, fileName);


            bool isFromDiller = false;

            string htmlString = RenderViewAsString("Utils", "~/Views/Utils/JuridicalInvoce.cshtml", SendInvoice, isFromDiller);

            string baseUrl = "";// collection["TxtBaseUrl"];

            string pdf_page_size = "A4";// collection["DdlPageSize"];
            SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";// collection["DdlPageOrientation"];
            SelectPdf.PdfPageOrientation pdfOrientation =
                (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                pdf_orientation, true);

            string pdf_align = "Justify";// collection["DdlPageOrientation"];
            SelectPdf.PdfTextHorizontalAlign pdfTextAlign =
                (SelectPdf.PdfTextHorizontalAlign)Enum.Parse(typeof(SelectPdf.PdfTextHorizontalAlign),
                pdf_align, true);

            int webPageWidth = 1024;

            int webPageHeight = 0;

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            var FolderName = (@"C:\JuridicalInvoice\Invoice\" + DateTime.Now.ToString("dd -MM-yyyy"));
            if (!Directory.Exists(FolderName))

            {

                Directory.CreateDirectory(FolderName);

            }
            var name = SplitAddresName("DigitalTv Invoice " + SendInvoice.Invoices_Code);
            doc.Save(FolderName + '\\' + name);
            return null;
        }
        [ValidateInput(false)]
        public ActionResult DisruptAutoInvoice(List<DisruptInvoice> SendInvoice)
        {
            string DocName = "DisruptInvoiceBalance";

            var path = System.IO.Path.GetTempPath();
            var fileName = DocName + ".pdf";
            var var = System.IO.Path.Combine(path, fileName);


            bool isFromDiller = false;

            string htmlString = RenderViewAsString("Utils", "~/Views/Utils/DisruptInvoiceBalance.cshtml", SendInvoice, isFromDiller);

            string baseUrl = "";// collection["TxtBaseUrl"];

            string pdf_page_size = "A4";// collection["DdlPageSize"];
            SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";// collection["DdlPageOrientation"];
            SelectPdf.PdfPageOrientation pdfOrientation =
                (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                pdf_orientation, true);

            string pdf_align = "Justify";// collection["DdlPageOrientation"];
            SelectPdf.PdfTextHorizontalAlign pdfTextAlign =
                (SelectPdf.PdfTextHorizontalAlign)Enum.Parse(typeof(SelectPdf.PdfTextHorizontalAlign),
                pdf_align, true);

            int webPageWidth = 1024;

            int webPageHeight = 0;

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();


            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            var FolderName = (@"C:\JuridicalInvoice\DisruptInvoice\" + DateTime.Now.ToString("dd-MM-yyyy"));
            if (!Directory.Exists(FolderName))

            {

                Directory.CreateDirectory(FolderName);

            }
            var name = SplitAddresName("DigitalTv Invoice " + SendInvoice.Select(s=>s.Invoices_Code).FirstOrDefault());
            doc.Save(FolderName + '\\' + name);
            return null;
        }
        public string RenderViewAsString(string controllerName, string viewName, object model, bool isFromDiller = false)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(fakeControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    fakeControllerContext,
                    viewResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    stringWriter
                    );
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }
        public static string ImageConvert()
        {

            var FileName = Path.GetFileName("Logowhite.png");
            var path = Path.Combine(HostingEnvironment.MapPath("~/Static/Images"), FileName);
            string imagepath = path;
            FileStream fs = new FileStream(imagepath, FileMode.Open);
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            var base64 = Convert.ToBase64String(byData);
            var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            return imgSrc;
        }
        public string SplitAddresName(string Name)
        {
            string FileNameArray = Name +" "+ DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";
            //string[] SplitFileName = FileNameArray.Split('\"');
            //string FileName = "";
            //foreach (var item in SplitFileName)
            //{
            //    FileName = FileName + item;
            //}
            return FileNameArray;
        }
    }
}