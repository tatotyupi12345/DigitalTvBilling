using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class InvoiceSendJob : IJob
    {
        private void AddLoging(DataContext _db, LogType type, LogMode mode, int user_id, long type_id, string type_value, List<LoggingItem> items)
        {
            Logging _logging = new Logging
            {
                Tdate = DateTime.Now,
                Type = type,
                UserId = user_id,
                Mode = mode,
                TypeId = type_id,
                TypeValue = type_value,
                LoggingItems = items
            };
            _db.Loggings.Add(_logging);
        }

        public void Execute(IJobExecutionContext context)
        {
            
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            bool has_send;
            string send_cards_str = "";
            using (DataContext _db = new DataContext())
            {
                var _params = _db.Params.ToList();
                int budget_invoice_invoice_day = int.Parse(_params.First(c => c.Name == "BudgetAbonentInvoiceDay").Value);
                int[] invoice_send_intervals = _params.Where(c => c.Name == "InvoiceSendIntervals").Select(c => c.Value).First().Split(';').Select(c => int.Parse(c)).ToArray();

                string system_email = _params.First(p => p.Name == "SystemEmail").Value;
                string system_email_password = _params.First(p => p.Name == "SystemEmailPassword").Value;
                string invoice_text = _params.First(p => p.Name == "InvoiceText").Value;

                var abonents = _db.Customers.Where(c => c.Type == Models.CustomerType.Juridical)
                    .Where(c => c.Cards.Any(cc => cc.AutoInvoice && cc.CardStatus == CardStatus.Active)).Select(c => new
                {
                    AbonentName = c.Name,
                    AbonentCode = c.Code,
                    AbonentAddress = c.City + ", " + c.Address,
                    AbonentPhone = c.Phone1,
                    AbonentEmail = c.Email,
                    IsBudget = c.IsBudget,
                    Id = c.Id,
                    Cards = c.Cards.Where(cc => cc.AutoInvoice).Select(cc => new
                    {
                        FinishDate = cc.FinishDate,
                        AbonentNum = cc.AbonentNum,
                        Id = cc.Id,
                        Amount = cc.Subscribtions.FirstOrDefault(s => s.Status).Amount
                    }).ToList()

                }).ToList();

                InvoicePdf _invoice = new InvoicePdf();
                List<LoggingItem> logging_items = new List<LoggingItem>();

                foreach (var abonent in abonents)
                {
                    InvoicePrint _invoicePrint = new InvoicePrint
                    {
                        AbonentAddress = abonent.AbonentAddress,
                        AbonentCode = abonent.AbonentCode,
                        AbonentEmail = abonent.AbonentEmail,
                        AbonentName = abonent.AbonentName,
                        AbonentPhone = abonent.AbonentPhone,
                        AbonentId = abonent.Id,
                        StartDate = today,
                        EndDate = today.AddMonths(1),
                        Items = new List<InvoicePrintItem>()
                    };

                    has_send = false;

                    if (abonent.IsBudget)
                    {
                        if (today.Day == budget_invoice_invoice_day)
                        {
                            int[] ids = abonent.Cards.Select(c => c.Id).ToArray();
                            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = abonent.Cards.Sum(c => c.Amount), Name = "სააბონენტო გადასახადი - " + String.Join(",", abonent.Cards.Select(c => c.AbonentNum)) });
                            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = (double)(_db.CardServices.Where(c => ids.Contains(c.CardId)).Where(c => c.IsActive).Sum(c => (decimal?)c.Amount) ?? 0), Name = "გაწეული მომსახურება" });

                            has_send = true;
                            send_cards_str = String.Join(",", abonent.Cards.Select(c => c.AbonentNum));
                        }
                    }
                    else
                    {
                        var send_cards = abonent.Cards.Where(c => invoice_send_intervals.Contains((c.FinishDate - today).Days)).ToList();
                        if (send_cards.Count > 0)
                        {
                            int[] ids = send_cards.Select(c => c.Id).ToArray();
                            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = send_cards.Sum(c => c.Amount), Name = "სააბონენტო გადასახადი - " + String.Join(",", send_cards.Select(c => c.AbonentNum)) });
                            _invoicePrint.Items.Add(new InvoicePrintItem { Amount = (double)(_db.CardServices.Where(c => ids.Contains(c.CardId)).Where(c => c.IsActive).Sum(c => (decimal?)c.Amount) ?? 0), Name = "გაწეული მომსახურება" });

                            has_send = true;
                            send_cards_str = String.Join(",", send_cards.Select(c => c.AbonentNum));
                        }
                    }


                    if (has_send)
                    {
                        Invoice last_inv = _db.Invoices.OrderByDescending(c => c.Id).FirstOrDefault();
                        _invoicePrint.Num = last_inv == null ? Utils.Utils.GetInvoiceNum("I0", DateTime.Now.Year) : Utils.Utils.GetInvoiceNum(last_inv.Num, last_inv.Tdate.Year);

                        _invoice.InvoiceData = _invoicePrint;
                        string file_short_name = "";
                        string file_name = new InvoicePdf() { InvoiceData = _invoicePrint }.Generate();
                        if (file_name == string.Empty)
                        {
                            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                            {
                                Date = today,
                                CardNum = abonent.AbonentName,
                                ChargeCrushLogType = Models.ChargeCrushLogType.InvoiceNotSend,
                                Text = "აბონენტი: " + abonent.AbonentName + ";" + abonent.AbonentEmail
                            });
                            continue;
                        }

                        bool res = Utils.Utils.SendEmail(new List<string>() { file_name }, "GlobalTV ინვოისი", "mail." + system_email.Split('@')[1], 25, false, system_email,
                                    system_email.Split('@')[0], system_email_password, new List<string>() { _invoicePrint.AbonentEmail }, new List<string>(), invoice_text);
                        if (!res)
                        {
                            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                            {
                                Date = today,
                                CardNum = abonent.AbonentName,
                                ChargeCrushLogType = Models.ChargeCrushLogType.InvoiceNotSend,
                                Text = "აბონენტი: " + abonent.AbonentName + ";" + abonent.AbonentEmail
                            });
                            System.IO.File.Delete(file_name);
                            continue;
                        }

                        file_short_name = file_name.Substring(file_name.LastIndexOf('\\') + 1);
                        try
                        {
                            FileInfo fi = new System.IO.FileInfo(file_name);
                            Stream fs = fi.OpenRead();
                            Utils.Utils.UploadFile(fs, _params.First(p => p.Name == "FTPHost").Value + "invoce/", _params.First(p => p.Name == "FTPLogin").Value, _params.First(p => p.Name == "FTPPassword").Value, file_short_name);
                            fs.Close();
                        }
                        catch (Exception ex)
                        {
                            _db.ChargeCrushLogs.Add(new ChargeCrushLog
                            {
                                Date = today,
                                CardNum = abonent.AbonentName,
                                ChargeCrushLogType = Models.ChargeCrushLogType.InvoiceNotSend,
                                Text = "აბონენტი: " + abonent.AbonentName + ";" + abonent.AbonentEmail
                            });
                            continue;
                        }
                        finally
                        {
                            System.IO.File.Delete(file_name);
                        }

                        Invoice _inv = new Invoice
                        {
                            CustomerId = _invoicePrint.AbonentId,
                            Num = _invoicePrint.Num,
                            Tdate = DateTime.Now,
                            FileName = file_short_name,
                            AbonentNums = send_cards_str
                        };
                        _db.Invoices.Add(_inv);

                        logging_items.Add(new LoggingItem { ColumnDisplay = "ინვოისი", OldValue = abonent.AbonentName, NewValue = abonent.AbonentCode });
                    }
                }

                this.AddLoging(_db, LogType.Invoice, LogMode.Add, 1, 0, "ავტომატური ინვოისი", logging_items);
                _db.SaveChanges();
            }
        }
    }
}