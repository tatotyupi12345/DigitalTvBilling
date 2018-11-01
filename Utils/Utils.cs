using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Hosting;

namespace DigitalTVBilling.Utils
{
    public class ConvertImage
    {
        public string ImageConvert(string Name, string adderess)
        {

            var FileName = Path.GetFileName(Name);
            var path = Path.Combine(HostingEnvironment.MapPath(adderess), FileName);
            string imagepath = path;
            FileStream fs = new FileStream(imagepath, FileMode.Open);
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            var base64 = Convert.ToBase64String(byData);
            var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            return imgSrc;
        }
    }
    public class Month
    {

        public string returnMonth(string month)
        {
            string result = "";
            switch (month)
            {
                case "February":
                    result = "თებერვალი";
                    break;
                case "March":
                    result = "მარტი";
                    break;
                case "April":
                    result = "აპრილი";
                    break;
                case "May":
                    result = "მაისი";
                    break;
                case "June":
                    result = "ივნისი";
                    break;
                case "July":
                    result = "ივლისი";
                    break;
                case "August":
                    result = "აგვისტო";
                    break;
                case "September":
                    result = "სექტემბერი";
                    break;
                case "October":
                    result = "ოქტომბერი";
                    break;
                case "November":
                    result = "ნოემბერი";
                    break;
                case "December":
                    result = "დეკემბერი";
                    break;
                case "January":
                    result = "იანვარი";
                    break;

            }
            return result;

        }
    }
    public struct DateTimeSpan
    {
        private readonly int years;
        private readonly int months;
        private readonly int days;
        private readonly int hours;
        private readonly int minutes;
        private readonly int seconds;
        private readonly int milliseconds;

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public int Years { get { return years; } }
        public int Months { get { return months; } }
        public int Days { get { return days; } }
        public int Hours { get { return hours; } }
        public int Minutes { get { return minutes; } }
        public int Seconds { get { return seconds; } }
        public int Milliseconds { get { return milliseconds; } }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }

    public class Utils
    {

        //divide card charge interval
        //means to divide day interval to calculate time span in hours or less measurement of time
        public static int divide_card_charge_interval = 1; //24

        public static void ErrorLogging(Exception ex, string path)
        {
            string strPath = path;// @"D:\Rekha\Log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
        }

        public static int TryIntParse(string val)
        {
            int res = 0;
            int.TryParse(val, out res);
            return res;
        }

        public static int TryIntNegativeParse(string val)
        {
            int res = 0;
            if (!int.TryParse(val, out res))
                return -1;
            return res;
        }

        public static string GetMd5(string key)
        {
            MD5 md5 = MD5.Create();
            if (key == null) return "";
            byte[] inputBytes = Encoding.ASCII.GetBytes(key);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }

        public static string GetSmsCode()
        {
            string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            Random rand = new Random();
            string res = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                res += numbers[rand.Next(0, 9)];
            }
            return res;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }

        public static DateTime GetRequestDate(string str, bool from,string year = "")
        {
            DateTime date;
            if (year == string.Empty)
            {
                try
                {
                    if (from)
                        date = new DateTime(int.Parse(str.Substring(4, 4)), int.Parse(str.Substring(2, 2)), int.Parse(str.Substring(0, 2)), 0, 0, 0);
                    else
                        date = new DateTime(int.Parse(str.Substring(4, 4)), int.Parse(str.Substring(2, 2)), int.Parse(str.Substring(0, 2)), 23, 59, 59);
                }
                catch
                {
                    if (from)
                    {
                        date = DateTime.Now.AddMonths(-1);
                        date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                    }
                    else
                        date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
            }
            else
            {
                int y = int.Parse(year);
                if(from)
                    date = new DateTime(y, 1, 1, 0, 0, 0);
                else
                    date = new DateTime(y, 12, 31, 23, 59, 59);
            }
            return date;
        }

        public static string GenerateFileName(string abonent_num, string file_ext)
        {
            return abonent_num + "_" + DateTime.Now.ToString("ddMMyyHHmm") + file_ext;
        }

        public static void UploadFile(Stream _fileStream, string host, string user_name, string password, string file_name)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create("ftp://" + host + file_name);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(user_name, password);
            request.UseBinary = true;
            request.ContentLength = _fileStream.Length;
            request.Timeout = 1000;

            using (Stream rs = request.GetRequestStream())
            {
                byte[] buffer = new byte[4097];
                int bytes = 0;
                int total_bytes = (int)_fileStream.Length;

                while (total_bytes > 0)
                {
                    bytes = _fileStream.Read(buffer, 0, buffer.Length);
                    rs.Write(buffer, 0, bytes);
                    total_bytes = total_bytes - bytes;
                }
                rs.Close();
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        public static string RandomChoice(Random rnd, string[] source)
        {
            return source[rnd.Next(0, source.Length)];
        }

        public static DateTime GenerateFinishDate(DateTime start_date, string charge_val)
        {
            string[] charge_vals = charge_val.Split(':');
            int hour = int.Parse(charge_vals[0]);
            DateTime f_date = new DateTime(start_date.Year, start_date.Month, start_date.Day, hour, int.Parse(charge_vals[1]), 0);
            return hour < 10 ? f_date.AddDays(1) : f_date;
        }

        public static DateTime GenerateJuridicalFinishDate(DateTime start_date, string charge_val, decimal amount, decimal jurid_limit_months, double discount, int days)
        {
            decimal balance = amount * jurid_limit_months;
            int day = 0, hours = 0;

            //original code
            //while (true)
            //{
            //    int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff;
            //    dayly_amount -= (dayly_amount * (decimal)discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    day++;
            //}
            //return GenerateFinishDate(charge_val).AddDays(days + day);

            while (true)
            {
                int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
                dayly_amount -= (dayly_amount * (decimal)discount / 100);
                if (balance < dayly_amount)
                    break;
                balance -= dayly_amount;
                hours++;
            }
            return GenerateFinishDate(charge_val).AddDays(days).AddHours(hours);
        }

        public static DateTime GenerateJuridicalFinishDate(string charge_val)
        {
            //decimal balance = amount * jurid_limit_months;
            //int day = 0, hours = 0;

            //original code
            //while (true)
            //{
            //    int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff;
            //    dayly_amount -= (dayly_amount * (decimal)discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    day++;
            //}
            //return GenerateFinishDate(charge_val).AddDays(days + day);

            //while (true)
            //{
            //    int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
            //    dayly_amount -= (dayly_amount * (decimal)discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    hours++;
            //}

            string[] charge_vals = charge_val.Split(':');

            int hour = int.Parse(charge_vals[0]);
            int minutes = int.Parse(charge_vals[1]);

            //DateTime fin_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 6, hour, minutes, 0);
            DateTime fin_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10, hour, minutes, 0);
            fin_date = fin_date.AddMonths(1);
            return fin_date;//GenerateFinishDate(charge_val).AddDays(days).AddHours(hours);
        }

        public static DateTime GenerateFinishDate(string charge_val)
        {
            string[] charge_vals = charge_val.Split(':');
            int hour = int.Parse(charge_vals[0]);
            DateTime f_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, int.Parse(charge_vals[1]), 0);
            return hour < 10 ? f_date.AddDays(1) : f_date;
        }

        public static void ChangeFinishDateByPackage(DataContext _db, CardDetailData _card, string charge_time, decimal jurid_limit_months, decimal amount = 0)
        {
            decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
            int day = 0, hours = 0;

            if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
            {
                balance += amount * jurid_limit_months;
            }

            if (amount == 0)
                return;

            //original code
            //while (true)
            //{
            //    int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff;
            //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    day++;
            //}

            //_card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(day);

            while (true)
            {
                int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
                dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                if (balance < dayly_amount)
                    break;
                balance -= dayly_amount;
                hours++;
            }

            _card.Card.FinishDate = _card.Card.CloseDate.AddHours(hours);// GenerateFinishDate(charge_time).AddHours(hours);

            _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
        }

        public static void SetFinishDate(DataContext _db, CardDetailData _card, decimal jurid_limit_months, int pause_days = 0)
        {
            decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
            decimal amount = (decimal)_card.SubscribAmount;
            int day = 0, time_interval = 0;
            int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);

            if (amount == 0)
                return;

            string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
            if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
            {
                balance += amount * jurid_limit_months;
            }

            //while (true)
            //{
            //    int coeff = DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff;
            //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    day++;
            //}

            //_card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(day + pause_days);


            //while (true)
            //{
            //    int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
            //    decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
            //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
            //    if (balance < dayly_amount)
            //        break;
            //    balance -= dayly_amount;
            //    time_interval++;
            //}

            int coeff = service_days;
            amount -= (amount * (decimal)_card.Card.Discount / 100);
            decimal dayly_amount = amount / coeff;
            time_interval = (int)Math.Round((balance / dayly_amount), 0);

            _card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(pause_days).AddHours(time_interval);

            _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public static DateTime? SetFinishDate(DataContext _db, decimal jurid_limit_months, int card_id)
        {
            CardDetailData _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
            {
                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                Card = c,
                CustomerType = c.Customer.Type,
                IsBudget = c.Customer.IsBudget,
                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                CardLogs = c.CardLogs.ToList()
            }).FirstOrDefault();

            if (_card != null)
            {
                
                decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                decimal amount = (decimal)_card.SubscribAmount;
                int day = 0, time_interval = 0;

                if (amount == 0)
                    return null;

                if (balance >= amount)
                {
                    //if (_card.CustomerType != CustomerType.Juridical)
                    //    _card.Card.CardStatus = CardStatus.Active;
                }
                    
                //else
                //{
                //    _card.Card.CardStatus = CardStatus.Closed;
                //}

                string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                {
                    balance += amount * jurid_limit_months;
                }

                //int coeff = service_days;
                //amount -= (amount * (decimal)_card.Card.Discount / 100);
                //decimal dayly_amount = amount / coeff;
                //day = (int)Math.Round((balance / dayly_amount), 0);

                //original code
                while (true)
                {
                    int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                    decimal dayly_amount = amount / coeff;
                    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                    if (balance < dayly_amount)
                        break;
                    balance -= dayly_amount;
                    day++;
                }

                //while (true)
                //{
                //    int coeff = service_days;//DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                //    decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
                //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                //    if (balance < dayly_amount)
                //        break;
                //    balance -= dayly_amount;
                //    time_interval++;
                //}

                //_card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(time_interval);// AddHours(time_interval); //GenerateFinishDate(charge_time).AddHours(hours);
                _card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(day);

                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                return _card.Card.FinishDate;
            }

            return null;
        }

        public static void SetJuridFinishDate(DataContext _db, decimal jurid_limit_months, int card_id)
        {
            CardDetailData _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
            {
                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                Card = c,
                CustomerType = c.Customer.Type,
                IsBudget = c.Customer.IsBudget,
                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                CardLogs = c.CardLogs.ToList()
            }).FirstOrDefault();

            if (_card != null)
            {

                decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                decimal amount = (decimal)_card.SubscribAmount;
                int day = 0, time_interval = 0;

                if (amount == 0)
                    return;

                string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                //if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                //{
                //    balance += amount * jurid_limit_months;
                //}

                //int coeff = service_days;
                //amount -= (amount * (decimal)_card.Card.Discount / 100);
                //decimal dayly_amount = amount / coeff;
                //day = (int)Math.Round((balance / dayly_amount), 0);

                //original code
                //while (true)
                //{
                //    int coeff = service_days;// DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                //    decimal dayly_amount = amount / coeff;
                //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                //    if (balance < dayly_amount)
                //        break;
                //    balance -= dayly_amount;
                //    day++;
                //}

                while (true)
                {
                    int coeff = service_days;//DateTime.DaysInMonth(DateTime.Now.AddDays(day).Year, DateTime.Now.AddDays(day).Month);
                    decimal dayly_amount = amount / coeff / Utils.divide_card_charge_interval;
                    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                    if (balance < dayly_amount)
                        break;
                    balance -= dayly_amount;
                    time_interval++;
                }

                //_card.Card.FinishDate = GenerateFinishDate(charge_time).AddDays(time_interval);// AddHours(time_interval); //GenerateFinishDate(charge_time).AddHours(hours);
                string[] charge_vals = charge_time.Split(':');
                int hour = int.Parse(charge_vals[0]);
                int minutes = int.Parse(charge_vals[1]);

                //DateTime dt__ = new DateTime();
                // var datejuridacl=GenerateFinishDate(charge_time).AddDays(time_interval);
                //DateTime fin_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6, hour, minutes, 0);
                //fin_date = fin_date.AddMonths(1);

                //DateTime f_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6, hour, minutes, 0);


                DateTime dfrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                DateTime dTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
                _card = _db.Cards.Where(c => c.Id == card_id).Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Select(c => new CardDetailData
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
                decimal _balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);   
                decimal CardCahrge_Balance = 0;
                var error = _db.CardCharges.Where(c => c.CardId == card_id && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).ToList();
                if (error.Count() != 0)
                {
                    CardCahrge_Balance = _db.CardCharges.Where(c => c.CardId == card_id && c.Tdate >= dfrom && c.Tdate <= dTo).Select(s => s.Amount).Sum();
                   
                }
                decimal _blance = _balance + CardCahrge_Balance;
                DateTime f_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minutes, 0);
                f_date = f_date.AddDays(time_interval);
                bool day_balance = false;

                if (Math.Round(_blance, 2) >= 0 || (_blance < 0 && _blance > (decimal)-0.2) && day_balance == false)
                {
                    //f_date = f_date.AddMonths(1);
                    f_date = f_date.AddMonths(1);
                    f_date = new DateTime(f_date.Year, f_date.Month, 10, f_date.Hour, f_date.Minute, 0);
                }
                else
                {

                    if (f_date.Day >= 10)
                    {
                        f_date = f_date.AddMonths(1);
                        f_date = new DateTime(f_date.Year, f_date.Month, 10, f_date.Hour, f_date.Minute, 0);
                        day_balance = true;
                    }

                    if (f_date.Day < 10)
                    {
                        f_date = new DateTime(f_date.Year, f_date.Month, 10, f_date.Hour, f_date.Minute, 0);
                        //f_date = f_date.AddDays(6 - f_date.Day);
                    }
                }


                _card.Card.FinishDate = f_date;//fin_date;// GenerateFinishDate(charge_time).AddDays(day);
                _card.Card.CardStatus = CardStatus.Active;
                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;


                _db.SaveChanges();

            }
        }

        ///adds time interval to finish date
        public static DateTime? getFinishDate(DataContext _db, decimal jurid_limit_months, int card_id, int time_interval_to_add)
        {
            DateTime ? returndt = null;
            CardDetailData _card = _db.Cards.Where(c => c.Id == card_id).Select(c => new CardDetailData
            {
                PaymentAmount = c.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                ChargeAmount = c.CardCharges.Select(s => (decimal?)s.Amount).Sum() ?? 0,
                Card = c,
                CustomerType = c.Customer.Type,
                IsBudget = c.Customer.IsBudget,
                SubscribAmount = c.Subscribtions.Where(s => s.Status).FirstOrDefault().Amount,
                MinPrice = c.Subscribtions.FirstOrDefault(s => s.Status).SubscriptionPackages.Sum(p => p.Package.MinPrice),
                CardLogs = c.CardLogs.ToList()
            }).FirstOrDefault();

            if (_card != null)
            {
                decimal balance = GetBalance(_card.PaymentAmount, _card.ChargeAmount);
                decimal amount = (decimal)_card.SubscribAmount;
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);
                int day = 0;

                if (amount == 0)
                    return null;

                string charge_time = _db.Params.First(p => p.Name == "CardCharge").Value;
                if (_card.CustomerType == CustomerType.Juridical && _card.IsBudget)
                {
                    balance += amount * jurid_limit_months;
                }

                //balance = 10;

                int coeff = service_days;
                amount -= (amount * (decimal)_card.Card.Discount / 100);
                decimal dayly_amount = amount / coeff;
                day = (int)Math.Round((balance / dayly_amount), 0);

                //while (true)
                //{
                //    int coeff = service_days;
                //    decimal dayly_amount = amount / coeff;
                //    dayly_amount = Math.Round(dayly_amount, 4);
                //    dayly_amount -= (dayly_amount * (decimal)_card.Card.Discount / 100);
                //    if (balance < dayly_amount)
                //        break;
                //    balance -= dayly_amount;
                //    day++;
                //}

                _card.Card.FinishDate =  GenerateFinishDate(charge_time).AddDays(day + time_interval_to_add);

                returndt = _card.Card.FinishDate;
                _db.Entry(_card.Card).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return returndt;
        }

        public static decimal GetBalance(decimal payments, decimal charges)
        {
            return Math.Round(payments - charges, 2);
        }

        public static decimal GetServiceBalance(decimal payments, decimal s_charges, decimal n_s_charges)
        {
            return payments - Math.Abs(s_charges - n_s_charges);
        }

        public static int  GetServicesDays(Card _card, int service_days)
        {
            if (_card.CardServices.Any(s => s.IsActive))
            {
                int y = (_card.FinishDate - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _card.FinishDate.Hour, _card.FinishDate.Minute, 0)).Days;
                int x = (_card.CardServices.First(s=>s.IsActive).Date.AddDays(service_days).Date - DateTime.Now.Date).Days;
                if (y < x)
                    return y;
                else
                    return x;
            }

            return -1;
        }

        public static string GenerateAbonentNum(string letter, int max_num)
        {
            if (max_num < 99999)
            {
                return letter + (max_num + 1).ToString().PadLeft(5, '0');
            }
            else
            {
                string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                return letters.GetValue((Array.IndexOf(letters, letter) + 1)) + "00001";
            }
        }

        public static string GetInvoiceNum(string max_num, int last_year)
        {
            if (last_year == DateTime.Now.Year)
            {
                return "I" + (TryIntParse(max_num.Substring(1)) + 1);
            }
            else
                return "I" + 1;
        }

        public static string IsAbonentNumExists(DataContext _db,string num)
        {
            if(_db.Cards.Any(c=>c.AbonentNum == num && c.CardStatus != CardStatus.Canceled))
            {
                string max_num = _db.Cards.Select(c => c.AbonentNum).OrderByDescending(c => c).FirstOrDefault();
                return GenerateAbonentNum(max_num.Substring(0, 1), int.Parse(max_num.Substring(1)));
            }
            return string.Empty;
        }

        public static Dictionary<string, bool> GetPrivilegies(DataContext _db, int type_id)
        {
            Dictionary<string, bool> data = new Dictionary<string, bool>();
            var perms = _db.UserPermissions.Where(p => p.Type == type_id).ToList();
            foreach(UserPermission perm in perms)
            {
                if (data.ContainsKey(perm.Tag))
                    continue;
                data.Add(perm.Tag, perm.Sign);
            }
            if (type_id == 1)
            {
                if (!data.ContainsKey("GROUP_SHOW"))
                    data.Add("GROUP_SHOW", true);
                if (!data.ContainsKey("GROUP_EDIT"))
                    data.Add("GROUP_EDIT", true);
            }
            return data;
        }

        public static string GetMonthText(int month)
        {
            switch(month)
            {
                case 1:
                    return "იანვარი";
                case 2:
                    return "თებერვალი";
                case 3:
                    return "მარტი";
                case 4:
                    return "აპრილი";
                case 5:
                    return "მაისი";
                case 6:
                    return "ივნისი";
                case 7:
                    return "ივლისი";
                case 8:
                    return "აგვისტო";
                case 9:
                    return "სექტემბერი";
                case 10:
                    return "ოქტომბერი";
                case 11:
                    return "ნოემბერი";
                case 12:
                    return "დეკემბერი";
                default:
                    return "";
            }
        }

        public static string GetDisplayName(Type type, string property_name)
        {
            return type.GetProperty(property_name).GetCustomAttribute<DisplayNameAttribute>().DisplayName;
        }

        public static List<LoggingData> GetAddedData(Type type, object obj)
        {
            List<LoggingData> result = new List<LoggingData>();
            try
            {
                //List<LoggingData> result = new List<LoggingData>();
                DisplayNameAttribute displayNameAttribute;

                PropertyInfo[] pInfos = type.GetProperties();
                foreach (PropertyInfo p_info in pInfos)
                {
                    if (p_info.GetCustomAttribute<NoLog>() == null)
                    {
                        displayNameAttribute = p_info.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayNameAttribute != null && displayNameAttribute.DisplayName!="სურათი:")
                        {
                            result.Add(new LoggingData
                            {
                                field = displayNameAttribute.DisplayName,
                                old_val = string.Empty,
                                new_val = p_info.GetValue(obj, null).ToString(),
                            });
                        }
                    }
                }
                return result;
            }
           
            catch ( Exception ex) { 


                     return result;
            }
            
        }
        
        public static bool GetPermission(string key)
        {
            var perms = (Dictionary<string, bool>)HttpContext.Current.Session["UserPermissions"];
            return perms != null && perms.ContainsKey(key) ? perms[key] : false;
        }
        public static bool GetPermissionInstalator(string key)
        {
            using (DataContext _db = new DataContext())
            {
                var perms = _db.UserPermissions.Where(s=>s.Tag==key && s.Type==44).FirstOrDefault();
                return perms != null && perms.Sign ? true : false;
            }
        }
        public static bool GetPermission(int user_id, string key)
        {
            using (DataContext _db = new DataContext())
            {
                var user = _db.Users.Where(u => u.Id == user_id).FirstOrDefault();
                var perms = GetPrivilegies(_db, user.Type); //(Dictionary<string, bool>)HttpContext.Current.Session["UserPermissions"];
                return perms != null && perms.ContainsKey(key) ? perms[key] : false;
            }
        }

        public static bool GetPermissionForProto(string key, Dictionary<string, bool> perms)
        {
            if (perms != null)
                //var perms = (Dictionary<string, bool>)HttpContext.Current.Session["UserPermissions"];
                return perms != null && perms.ContainsKey(key) ? perms[key] : false;
            else return false;
        }

        public static bool RestartAppPool()
        {
            try
            {
                Process process = Process.GetCurrentProcess();
                process.Kill();
                return true;
            }
            catch
            {
            }

            try
            {
                HttpRuntime.UnloadAppDomain();
                return true;
            }
            catch
            {
            }

            string webConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";
            try
            {
                File.SetLastWriteTimeUtc(webConfigPath, DateTime.UtcNow);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool SendEmail(List<string> file, string Subject, string SmtpHost, int SmtpPort, bool EnableSsl, string SenderAddress, string login, string password, List<string> ReceiverAddress, List<string> BCC, string MessageText)
        {
            System.Net.Mail.Attachment fileAttachment;
            using (MailMessage mailMessage = new MailMessage() { From = new MailAddress(SenderAddress), Priority = MailPriority.High, Body = MessageText, Subject = Subject })
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient()
                    {
                        Host = SmtpHost,
                        Port = SmtpPort,
                        Credentials = new NetworkCredential(login, password),
                        EnableSsl = EnableSsl
                    };
                    file.ForEach(filePath =>
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileAttachment = new System.Net.Mail.Attachment(filePath, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = fileAttachment.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);
                            mailMessage.Attachments.Add(fileAttachment);
                        }
                    });
                    ReceiverAddress.ForEach(toEmail => { mailMessage.To.Add(toEmail); });
                    BCC.ForEach(bcc => { mailMessage.Bcc.Add(bcc); });
                    smtpClient.Send(mailMessage);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
        }

        internal static bool IsRequestValid(SecureRequest secure, bool CheckWithDate = true)
        {
            if (secure == null)
                return false;
            DateTime _date;
            if (!DateTime.TryParseExact(secure.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date))
                return false;
            string prepeare = string.Concat(secure.Date, (_date.Year + _date.Month + _date.Day + _date.Hour + _date.Minute + _date.Second));

            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(prepeare);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            return ((sb.ToString() == secure.Key) && isValitData(_date, CheckWithDate));
        }

        private static bool isValitData(DateTime datetime, bool CheckWithDate)
        {
            if (!CheckWithDate)
                return true;
            DateTime server_date = DateTime.Now;
            TimeSpan ts = server_date.Subtract(datetime);
            if (Math.Abs(ts.Minutes) > 30)
                return false;
            return true;
        }

        public static List<string> GetPackageNames(List<int> packages)
        {
            using(DataContext _db = new DataContext())
            {
                return _db.Packages.Where(p => packages.Contains(p.Id)).Select(c => c.Name).ToList();
            }
        }

        public static string ReplaceMessageTags(string message_text, Card card, DataContext _db)
        {
            if (message_text.Contains("[name]"))
                message_text = message_text.Replace("[name]", card.Customer.Name);
            if (message_text.Contains("[lastname]"))
                message_text = message_text.Replace("[lastname]", card.Customer.LastName);
            if (message_text.Contains("[code]"))
                message_text = message_text.Replace("[code]", card.Customer.Code);
            if (message_text.Contains("[address]"))
                message_text = message_text.Replace("[address]", card.Customer.Address);
            if (message_text.Contains("[city]"))
                message_text = message_text.Replace("[city]", card.Customer.City);
            if (message_text.Contains("[village]"))
                message_text = message_text.Replace("[village]", card.Customer.Village);
            if (message_text.Contains("[region]"))
                message_text = message_text.Replace("[region]", card.Customer.Region);
            if (message_text.Contains("[phone1]"))
                message_text = message_text.Replace("[phone1]", card.Customer.Phone1);
            if (message_text.Contains("[doc_num]"))
                message_text = message_text.Replace("[doc_num]", card.DocNum);
            if (message_text.Contains("[abonent_num]"))
                message_text = message_text.Replace("[abonent_num]", card.AbonentNum);
            if (message_text.Contains("[card_num]"))
                message_text = message_text.Replace("[card_num]", card.CardNum);
            if (message_text.Contains("[card_address]"))
                message_text = message_text.Replace("[card_address]", card.Address);
            if (message_text.Contains("[receiver]"))
                message_text = message_text.Replace("[receiver]", card.Receiver.Name);
            if (message_text.Contains("[tower]"))
                message_text = message_text.Replace("[tower]", card.Tower.Name);
            if (message_text.Contains("[discount]"))
                message_text = message_text.Replace("[discount]", card.Discount.ToString());
            if (message_text.Contains("[balance]"))
                message_text = message_text.Replace("[balance]", ((_db.Payments.Where(p => p.CardId == card.Id).Select(c => (decimal?)c.Amount).Sum() ?? 0) - (_db.CardCharges.Where(p => p.CardId == card.Id).Select(c => (decimal?)c.Amount).Sum() ?? 0)).ToString());
            if (message_text.Contains("[finish_date]"))
                message_text = message_text.Replace("[finish_date]", card.FinishDate.ToString("yyyy-MM-dd HH:mm"));
            if (message_text.Contains("[packet]"))
                message_text = message_text.Replace("[packet]", String.Join("+", _db.Subscribtions.Include("SubscriptionPackages.Package").Where(s => s.CardId == card.Id).Where(s => s.Status).FirstOrDefault().SubscriptionPackages.Select(c => c.Package.Name).ToList()));
            if (message_text.Contains("[packet_price]"))
                message_text = message_text.Replace("[packet_price]", String.Join(",", _db.Subscribtions.Where(s => s.CardId == card.Id).Where(s => s.Status).FirstOrDefault().Amount));

            return message_text;
        }

        //public static bool OnSendSMS(List<Card> cards, string text, string username, string password, DataContext _db, long message_id)
        //{
        //    Func<string, string> convertToLat = (string txt) =>
        //    {
        //        Dictionary<char, string> alphabet = new Dictionary<char, string>();
        //        alphabet.Add('ა', "a");
        //        alphabet.Add('ბ', "b");
        //        alphabet.Add('გ', "g");
        //        alphabet.Add('დ', "d");
        //        alphabet.Add('ე', "e");
        //        alphabet.Add('ვ', "v");
        //        alphabet.Add('ზ', "z");
        //        alphabet.Add('თ', "t");
        //        alphabet.Add('ი', "i");
        //        alphabet.Add('კ', "k");
        //        alphabet.Add('ლ', "l");
        //        alphabet.Add('მ', "m");
        //        alphabet.Add('ნ', "n");
        //        alphabet.Add('ო', "o");
        //        alphabet.Add('პ', "p");
        //        alphabet.Add('ჟ', "jh");
        //        alphabet.Add('რ', "r");
        //        alphabet.Add('ს', "s");
        //        alphabet.Add('ტ', "t");
        //        alphabet.Add('უ', "u");
        //        alphabet.Add('ფ', "f");
        //        alphabet.Add('ქ', "q");
        //        alphabet.Add('ღ', "g");
        //        alphabet.Add('ყ', "k");
        //        alphabet.Add('შ', "sh");
        //        alphabet.Add('ჩ', "ch");
        //        alphabet.Add('ც', "c");
        //        alphabet.Add('ძ', "dz");
        //        alphabet.Add('წ', "ts");
        //        alphabet.Add('ჭ', "tch");
        //        alphabet.Add('ხ', "kh");
        //        alphabet.Add('ჯ', "j");
        //        alphabet.Add('ჰ', "h");

        //        string textLat = string.Empty;
        //        char[] characters = txt.ToCharArray();
        //        foreach (char val in characters)
        //        {
        //            textLat += alphabet.ContainsKey(val) ? alphabet[val] : val.ToString();
        //        }
        //        return textLat;
        //    };

        //    HttpClient httpClient = new HttpClient()
        //    {
        //        BaseAddress = new Uri("http://smsco.ge/"),
        //    };
        //    HttpResponseMessage result;
        //    string message = "";

        //    foreach (Card phone in cards)
        //    {
        //        message = Uri.EscapeDataString(convertToLat(ReplaceMessageTags(text, phone, _db)));
        //        result = httpClient.GetAsync("components/com_smsreseller/smsapi2.php?username=" + username + "&password=" + password + "&recipient=995" + phone.Customer.Phone1 + "&message=" + message).Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            using (StreamReader resultStream = new StreamReader(result.Content.ReadAsStreamAsync().Result))
        //            {
        //                if (!resultStream.ReadToEnd().StartsWith("OK"))
        //                {
        //                    _db.MessageNotSendLogs.Add(new MessageNotSendLog
        //                    {
        //                        CardId = phone.Id,
        //                        MessageId = message_id,
        //                        MessageType = MessageType.SMS,
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    _db.SaveChanges();
        //    return true;
        //}

        public static bool OnSendSMS(List<Card> cards, string text, string username, string password, DataContext _db, long message_id)
        {
            Func<string, string> convertToLat = (string txt) =>
            {
                Dictionary<char, string> alphabet = new Dictionary<char, string>();
                alphabet.Add('ა', "a");
                alphabet.Add('ბ', "b");
                alphabet.Add('გ', "g");
                alphabet.Add('დ', "d");
                alphabet.Add('ე', "e");
                alphabet.Add('ვ', "v");
                alphabet.Add('ზ', "z");
                alphabet.Add('თ', "t");
                alphabet.Add('ი', "i");
                alphabet.Add('კ', "k");
                alphabet.Add('ლ', "l");
                alphabet.Add('მ', "m");
                alphabet.Add('ნ', "n");
                alphabet.Add('ო', "o");
                alphabet.Add('პ', "p");
                alphabet.Add('ჟ', "jh");
                alphabet.Add('რ', "r");
                alphabet.Add('ს', "s");
                alphabet.Add('ტ', "t");
                alphabet.Add('უ', "u");
                alphabet.Add('ფ', "f");
                alphabet.Add('ქ', "q");
                alphabet.Add('ღ', "g");
                alphabet.Add('ყ', "k");
                alphabet.Add('შ', "sh");
                alphabet.Add('ჩ', "ch");
                alphabet.Add('ც', "c");
                alphabet.Add('ძ', "dz");
                alphabet.Add('წ', "ts");
                alphabet.Add('ჭ', "tch");
                alphabet.Add('ხ', "kh");
                alphabet.Add('ჯ', "j");
                alphabet.Add('ჰ', "h");

                string textLat = string.Empty;
                char[] characters = txt.ToCharArray();
                foreach (char val in characters)
                {
                    textLat += alphabet.ContainsKey(val) ? alphabet[val] : val.ToString();
                }
                return textLat;
            };

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://81.95.160.47/"),
            };
            HttpResponseMessage result;
            string message = "";

            foreach (Card phone in cards)
            {
                
                message = Uri.EscapeDataString(convertToLat(ReplaceMessageTags(text, phone, _db)));
                result = httpClient.GetAsync("mt/oneway?username=" + username + "&password=" + password + "&client_id=570&service_id=1&to=995" + phone.Customer.Phone1 + "&text=" + message).Result;
                string url = "http://81.95.160.47/" + "mt/oneway?username=" + username + "&password=" + password + "&client_id=570&service_id=1&to=995" + phone.Customer.Phone1 + "&text=" + message;
                if (result.IsSuccessStatusCode)
                {
                    using (StreamReader resultStream = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                    {
                        if (!resultStream.ReadToEnd().StartsWith("OK"))
                        {
                            _db.MessageNotSendLogs.Add(new MessageNotSendLog
                            {
                                CardId = phone.Id,
                                MessageId = message_id,
                                MessageType = MessageType.SMS,
                            });
                        }
                    }
                }
            }
            _db.SaveChanges();
            return true;
        }

        public static bool OnSendAutorizeSMS(string phone, string text, string username, string password)
        {
            Task.Run(async () => { await sendMessage(phone, text); }).Wait();
            return true;

            //HttpClient httpClient = new HttpClient()
            //{
            //    BaseAddress = new Uri("http://81.95.160.47/"),
            //};
            ////HttpResponseMessage result = httpClient.GetAsync("components/com_smsreseller/smsapi2.php?username=" + username + "&password=" + password + "&recipient=995" + phone + "&message=avtorizaciis kodi:" + text).Result;
            //HttpResponseMessage result = httpClient.GetAsync("mt/oneway?username=" + username + "&password=" + password + "&client_id=570&service_id=1&to=995" + phone + "&text=avtorizaciis kodi:" + text).Result;
            //if (result.IsSuccessStatusCode)
            //{
            //    using (StreamReader resultStream = new StreamReader(result.Content.ReadAsStreamAsync().Result))
            //    {
            //        return resultStream.ReadToEnd().StartsWith("OK");
            //    }
            //}
            //return false;
        }

        public static bool OnSendSMS(List<string> phones, string text, string username, string password, DataContext _db)
        {
            Func<string, string> convertToLat = (string txt) =>
            {
                Dictionary<char, string> alphabet = new Dictionary<char, string>();
                alphabet.Add('ა', "a");
                alphabet.Add('ბ', "b");
                alphabet.Add('გ', "g");
                alphabet.Add('დ', "d");
                alphabet.Add('ე', "e");
                alphabet.Add('ვ', "v");
                alphabet.Add('ზ', "z");
                alphabet.Add('თ', "t");
                alphabet.Add('ი', "i");
                alphabet.Add('კ', "k");
                alphabet.Add('ლ', "l");
                alphabet.Add('მ', "m");
                alphabet.Add('ნ', "n");
                alphabet.Add('ო', "o");
                alphabet.Add('პ', "p");
                alphabet.Add('ჟ', "jh");
                alphabet.Add('რ', "r");
                alphabet.Add('ს', "s");
                alphabet.Add('ტ', "t");
                alphabet.Add('უ', "u");
                alphabet.Add('ფ', "f");
                alphabet.Add('ქ', "q");
                alphabet.Add('ღ', "g");
                alphabet.Add('ყ', "k");
                alphabet.Add('შ', "sh");
                alphabet.Add('ჩ', "ch");
                alphabet.Add('ც', "c");
                alphabet.Add('ძ', "dz");
                alphabet.Add('წ', "ts");
                alphabet.Add('ჭ', "tch");
                alphabet.Add('ხ', "kh");
                alphabet.Add('ჯ', "j");
                alphabet.Add('ჰ', "h");

                string textLat = string.Empty;
                char[] characters = txt.ToCharArray();
                foreach (char val in characters)
                {
                    textLat += alphabet.ContainsKey(val) ? alphabet[val] : val.ToString();
                }
                return textLat;
            };

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://81.95.160.47/"),
            };
            HttpResponseMessage result;
            string message = "";

            foreach (string phone in phones)
            {
                message = Uri.EscapeDataString(convertToLat(text));
                //result = httpClient.GetAsync("components/com_smsreseller/smsapi2.php?username=" + username + "&password=" + password + "&recipient=995" + phone + "&message=" + message).Result;
                result = httpClient.GetAsync("mt/oneway?username=" + username + "&password=" + password + "&client_id=570&service_id=1&to=995" + phone + "&text=" + message).Result;
                if (result.IsSuccessStatusCode)
                {
                    using (StreamReader resultStream = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                    {
                        if (!resultStream.ReadToEnd().StartsWith("OK"))
                        {
                        }
                    }
                }
            }
            return true;
        }

        public static async System.Threading.Tasks.Task<string> sendMessage(string phone, string message)
        {
            //http://81.95.160.47/mt/oneway?username=stereoplus&password=ST570&client_id=570&service_id=1&to=995598894533&text=Test
            if (phone.All(Char.IsDigit))
            {
                if (phone.Length == 9)
                {
                    phone = "995" + phone;
                }
                else if (phone.Length > 12)
                {
                    phone = phone.Substring((phone.Length) - 12);
                }

                using (HttpClient client = new HttpClient())
                {
                    string smsurl = "";// 
                    using (DataContext _db = new DataContext())
                    {
                        smsurl = String.Format(_db.Params.Where(p => p.Name == "SmsUrl").FirstOrDefault().Value, phone, message);
                    }
                    var uri = new Uri(smsurl);

                    var response = await client.GetAsync(uri);

                    string textResult = await response.Content.ReadAsStringAsync();
                    return textResult;
                }
            }
            return "only numbers are allowed!";
        }
        public static async System.Threading.Tasks.Task<string> send_Message(string phone, string message)
        {
            //http://81.95.160.47/mt/oneway?username=stereoplus&password=ST570&client_id=570&service_id=1&to=995598894533&text=Test
            if (phone.All(Char.IsDigit))
            {
                if (phone.Length == 9)
                {
                    phone = "995" + phone;
                }
                else if (phone.Length > 12)
                {
                    phone = phone.Substring((phone.Length) - 12);
                }

                using (HttpClient client = new HttpClient())
                {
                    string smsurl = "";// 
                    using (DataContext _db = new DataContext())
                    {
                        smsurl = "http://www.smsoffice.ge/api/send.aspx?key=2ace960774d84c91b5072748c59fde9b&destination="+phone+"&sender=DIGITAL%20TV&content="+message+"";
                            //String.Format(_db.Params.Where(p => p.Name == "SmsUrl").FirstOrDefault().Value, phone, message);
                    }
                    var uri = new Uri(smsurl);

                    var response = await client.GetAsync(uri);

                    string textResult = await response.Content.ReadAsStringAsync();
                    return textResult;
                }
            }
            return "only numbers are allowed!";
        }
        public static T GetAttributeOfType<T>(Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }

        static T Deserialize<T>(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch
            {
                throw;
            }
        }

        public static bool isValitContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }

        public static bool isValidLenght(int contentLength)
        {
            return (contentLength / 1024) / 1024 < 1; //1MB
        }

        public static void UploadFileOnFTP(string filename, string ftpServerIP, string ftpUserName, string ftpPassword)
        {
            //string filename = Server.MapPath("file1.txt");
            //string ftpServerIP = "ftp.demo.com/";
            //string ftpUserName = "dummy";
            //string ftpPassword = "dummy";

            FileInfo objFile = new FileInfo(filename);
            FtpWebRequest objFTPRequest;

            // Create FtpWebRequest object 
            objFTPRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + objFile.Name));

            // Set Credintials
            objFTPRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            // By default KeepAlive is true, where the control connection is 
            // not closed after a command is executed.
            objFTPRequest.KeepAlive = false;

            // Set the data transfer type.
            objFTPRequest.UseBinary = true;

            // Set content length
            objFTPRequest.ContentLength = objFile.Length;

            // Set request method
            objFTPRequest.Method = WebRequestMethods.Ftp.UploadFile;

            // Set buffer size
            int intBufferLength = 16 * 1024;
            byte[] objBuffer = new byte[intBufferLength];

            // Opens a file to read
            FileStream objFileStream = objFile.OpenRead();

            try
            {
                // Get Stream of the file
                Stream objStream = objFTPRequest.GetRequestStream();

                int len = 0;

                while ((len = objFileStream.Read(objBuffer, 0, intBufferLength)) != 0)
                {
                    // Write file Content 
                    objStream.Write(objBuffer, 0, len);

                }

                objStream.Close();
                objFileStream.Close();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public static bool DeleteFileOnFtpServer(Uri serverUri, string ftpUsername, string ftpPassword)
        {
            try
            {
                // The serverUri parameter should use the ftp:// scheme.
                // It contains the name of the server file that is to be deleted.
                // Example: ftp://contoso.com/someFile.txt.
                // 

                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
    

    
}