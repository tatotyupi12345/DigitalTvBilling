using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class InvoicesList
    {
        public int Id { get; set; }
        public DateTime Tdate { get; set; }
        public string AbonentName { get; set; }
        public string AbonentNum { get; set; }
        public string FileName { get; set; }
        public string Num { get; set; }

    }
    public class JuridicalInvoicesList
    {
        public string Name { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public List<CustomerSellAttachments> _attachment { get; set; }
        public decimal balance { get; set; }
        public double PackagesPrice { get; set; }
        public int Count { get; set; }
        public string Invoices_Code { get; set; }
        public int Ramdom_Generator { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
    }
    public class DisruptInvoice
    {
        public string abonent_number { get; set; }
        public decimal Debts { get; set; }
        public string Invoices_Code { get; set; }
        public string CompanyName { get; set; }
    }
}