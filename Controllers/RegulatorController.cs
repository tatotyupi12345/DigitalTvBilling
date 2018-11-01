using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    public class RegulatorController : Controller
    {
        // GET: RegulatorController
        public ActionResult Index(int? page)
        {
            if (!Utils.Utils.GetPermission("REGULATOR_SHOW"))
            {
                return new RedirectResult("/abonent");
            }

            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            ViewBag.dtFrom = dateFrom;
            ViewBag.dtTo = dateTo;
            DateTime dateFromDay = DateTime.Now;
            dateFromDay = dateFrom.AddHours((double)1);
            List<int> intss = new List<int>();
            Double Dagger = 1.18;
            Double Dagger_018 = 0.18;
            PacksChargesRent packs_charges_rent = new PacksChargesRent();
            using (DataContext _db = new DataContext())
            {
                ViewBag.ReCardAbonentPackgulatorActive = "active";
                var CardAbonentPack = _db.Cards.Include("CardLogs").Include("CardCharges").Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo)).ToList();
                decimal share15price = (decimal)_db.Packages.Where(s => s.Price == 15).FirstOrDefault().Price;
                decimal share12price = (decimal)_db.Packages.Where(s => s.Name == "სტანდარტი").FirstOrDefault().Price;
                decimal share8price = (decimal)_db.Packages.Where(p => p.Name == "აქცია 8").FirstOrDefault().Price;
                decimal share6price = (decimal)_db.Packages.Where(p => p.Name == "თანამშრომელი").FirstOrDefault().Price;
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);

                decimal Caharges15 = Math.Round((decimal)(share15price / (decimal)service_days), 2);
                decimal Caharges12 = Math.Round((decimal)(share12price / (decimal)service_days), 2);
                decimal Caharges8 = Math.Round((decimal)(share8price / (decimal)service_days), 2);
                decimal Caharges6 = Math.Round((decimal)(share6price / (decimal)service_days), 2);

                var pack15BackActive = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges15 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Physical).ToList();
                var pack12BackActive = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges12 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Physical).ToList();
                var pack8BackActive = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges8 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Physical).ToList();
                var pack6BackActive = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges6 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Physical).ToList();

                int Repetition = 0;
                var AbonentPack = CardAbonentPack.Where(c => c.CardLogs.Any(cc => cc.Date >= dateFrom && cc.Date <= dateTo && cc.Status == CardLogStatus.Open) && c.Customer.Type == CustomerType.Physical).ToList();
                var newabonent = CardAbonentPack.Where(c => c.CardLogs.Any(cc => cc.Date >= dateFrom && cc.Date <= dateTo && cc.Status == CardLogStatus.Montage) && c.Customer.Type == CustomerType.Physical).ToList();

                int count15 = 0, count12 = 0, count6 = 0, count8 = 0;

                var packs8 = pack15BackActive.Where(c => c.Subscribtions.Any(sb => sb.Tdate >= dateFrom && sb.Tdate <= dateTo && sb.SubscriptionPackages.Any(s => s.Package.Id == 304085))).ToList();
                count8 += packs8.Count;
                int _count8 = packs8.Count;
                List<Card> CardsAbonent = new List<Card>();
                foreach (var item in AbonentPack)
                {
                    var pack = item.CardLogs.Where(c => c.Date >= dateFrom && c.Date <= dateTo).OrderBy(l => l.Date).ToList();
                    if (pack != null && pack.Count() > 0)
                    {
                        if (pack[pack.Count() - 1].Status == CardLogStatus.Open)
                        {
                            if (pack15BackActive.Contains(item) || pack12BackActive.Contains(item) || pack8BackActive.Contains(item) || pack6BackActive.Contains(item))
                            {

                            }
                            else
                            {
                                CardsAbonent.Add(item);
                                DateTime chargetime = new DateTime(pack[pack.Count() - 1].Date.Year, pack[pack.Count() - 1].Date.Month, pack[pack.Count() - 1].Date.Day, 0, 1, 0);
                                chargetime = chargetime.AddDays(1);

                                CardCharge charge = item.CardCharges.Where(c => c.Tdate == chargetime).FirstOrDefault();
                                if (charge != null)
                                {
                                    decimal chargeCoeff = Math.Round(charge.Amount, 2);

                                    if (chargeCoeff == Caharges15)
                                    {
                                        if (item.Subscribtions.Any(sb => sb.Tdate >= dateFrom && sb.Tdate <= dateTo && sb.SubscriptionPackages.Any(s => s.Package.Id == 304085)))
                                        {

                                            count8++;
                                        }
                                        else
                                        {

                                            count15++;
                                        }
                                    }

                                    if (chargeCoeff == Caharges12)
                                    {

                                        count12++;

                                    }
                                    if (chargeCoeff == Caharges6)
                                    {

                                        count6++;
                                       
                                    }
                                    if (chargeCoeff == Caharges8)
                                    {

                                        count8++;
                                       
                                    }
                                }
                                else
                                {
                                    decimal packprice = (decimal)item.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Sum(s => s.Package.Price);
                                    decimal coeff = Math.Round((decimal)(packprice / (decimal)service_days), 2);

                                    if (coeff == Caharges15)
                                    {
 
                                        count15++;
                                   
                                    }

                                    if (coeff == Caharges12)
                                    {

                                        count12++;
                                     
                                    }
                                    if (coeff == Caharges6)
                                    {

                                        count6++;
                                 
                                    }
                                    if (coeff == Caharges8)
                                    {

                                        count8++;
                                   
                                    }
                                }
                            }
                        }
                    }
                }

                ViewBag.StandardNewAbonent = newabonent.Count();// pack15newAbonent.Count() + pack12newAbonent.Count()+ pack6newAbonent.Count();
                ViewBag.RegionNewAbonent = 0;

                // იურდიული
                var pack15BackActiveJuridical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges15 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Juridical).ToList();
                var pack12BackActiveJuridical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges12 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Juridical).ToList();
                var pack8BackActiveJuridical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges8 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Juridical).ToList();
                // var pack6BackActiveJuridical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Caharges6 && cc.Tdate >= dateFrom && cc.Tdate <= dateFromDay && cc.Status == CardChargeStatus.Daily) && c.Customer.Type == CustomerType.Juridical).ToList();

                var AbonentPackJuridical = CardAbonentPack.Where(c => c.CardLogs.Any(cc => cc.Date >= dateFrom && cc.Date <= dateTo && cc.Status == CardLogStatus.Open) && c.Customer.Type == CustomerType.Juridical).ToList();
                var newabonentJuridical = CardAbonentPack.Where(c => c.CardLogs.Any(cc => cc.Date >= dateFrom && cc.Date <= dateTo && cc.Status == CardLogStatus.Montage) && c.Customer.Type == CustomerType.Juridical).ToList();

                int count15Juridical = 0, count12Juridical = 0, count8Juridical = 0;

                var packs8Juridical = pack15BackActiveJuridical.Where(c => c.Subscribtions.Any(sb => sb.Tdate >= dateFrom && sb.Tdate <= dateTo && sb.SubscriptionPackages.Any(s => s.Package.Id == 304085))).ToList();
                count8Juridical += packs8Juridical.Count;
                int _count8Juridical = packs8Juridical.Count;

                List<Card> CardsAbonentJuridical = new List<Card>();

                foreach (var item in AbonentPackJuridical)
                {
                    var pack = item.CardLogs.Where(c => c.Date >= dateFrom && c.Date <= dateTo).OrderBy(l => l.Date).ToList();
                    if (pack != null && pack.Count() > 0)
                    {
                        if (pack[pack.Count() - 1].Status == CardLogStatus.Open)
                        {
                            if (pack15BackActiveJuridical.Contains(item) || pack12BackActiveJuridical.Contains(item) || pack8BackActiveJuridical.Contains(item))
                            {

                            }
                            else
                            {
                                //CardsAbonent.Add(item);
                                DateTime chargetime = new DateTime(pack[pack.Count() - 1].Date.Year, pack[pack.Count() - 1].Date.Month, pack[pack.Count() - 1].Date.Day, 0, 1, 0);
                                chargetime = chargetime.AddDays(1);

                                CardCharge charge = item.CardCharges.Where(c => c.Tdate == chargetime).FirstOrDefault();
                                if (charge != null)
                                {
                                    decimal chargeCoeff = Math.Round(charge.Amount, 2);

                                    if (chargeCoeff == Caharges15)
                                    {
                                        if (item.Subscribtions.Any(sb => sb.Tdate >= dateFrom && sb.Tdate <= dateTo && sb.SubscriptionPackages.Any(s => s.Package.Id == 304085)))
                                        {

                                            count8Juridical++;
                                        }
                                        else
                                        {

                                            count15Juridical++;
                                        }
                                    }

                                    if (chargeCoeff == Caharges12)
                                    {

                                        count12Juridical++;

                                    }
                                    if (chargeCoeff == Caharges8)
                                    {

                                        count8Juridical++;

                                    }
                                }
                                else
                                {
                                    decimal packprice = (decimal)item.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Sum(s => s.Package.Price);
                                    decimal coeff = Math.Round((decimal)(packprice / (decimal)service_days), 2);

                                    if (coeff == Caharges15)
                                    {

                                        count15Juridical++;

                                    }

                                    if (coeff == Caharges12)
                                    {

                                        count12Juridical++;

                                    }
                                    if (coeff == Caharges8)
                                    {

                                        count8Juridical++;

                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in AbonentPackJuridical)
                {
                    var pack = item.CardLogs.Where(c => c.Date >= dateFrom && c.Date <= dateTo).OrderBy(l => l.Date).ToList();
                    if (pack != null && pack.Count() > 0)
                    {
                        if (pack[pack.Count() - 1].Status == CardLogStatus.Open)
                        {
                            if (pack15BackActiveJuridical.Contains(item) || pack12BackActiveJuridical.Contains(item) || pack8BackActiveJuridical.Contains(item))
                            {

                            }
                            else
                            {
                                CardsAbonentJuridical.Add(item);
                            }
                        }
                    }
                }
                var pack15Juridical = CardsAbonentJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price == 15)).Select(c => c).ToList();
                var pack12Juridical = CardsAbonentJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();
                var pack8Juridical = CardsAbonentJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();
                //var pack6Juridical = CardsAbonentJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c).ToList();

                ViewBag.Standard = pack15BackActive.Count() - _count8 + pack12BackActive.Count() + pack6BackActive.Count() + count15 + count12 + count6;
                ViewBag.StandardNewAbonent = newabonent.Count();
                ViewBag.Region = pack8BackActive.Count() + count8;
                ViewBag.RegionJuridical = pack8BackActiveJuridical.Count() + count8Juridical;
                ViewBag.StandardJuridical = pack15BackActiveJuridical.Count()-_count8Juridical + pack12BackActiveJuridical.Count() + count15Juridical + count12Juridical;
                ViewBag.StandardNewAbonentJuridical = newabonentJuridical.Count();


                List<Card> cardsPhysical = null;
                List<Card> cardsJuridical = null;

                PacksCharges packs_charges = new PacksCharges();
                PacksChargesCoeff packs_charges_coeff = new PacksChargesCoeff();
                PacksExcept packs_execept = new PacksExcept();

                cardsPhysical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo) && c.Customer.Type == CustomerType.Physical).ToList();
                var packemployee = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c).ToList();
                var pack12 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();
                var pack8 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();
                var pack15 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price == 15)).Select(c => c).ToList();

                var packexpect = packs_execept.Packs_Except_Charges(pack8, pack15, dateTo);

                pack8 = packexpect.packs8;
                pack15 = packexpect.packs15; 
                
                var employee6 = packs_charges_coeff.PacksCardsChargesCoeff(packemployee, dateFrom, dateTo, share6price, service_days);//packs_charges.PacksCardsCharges(packemployee, dateFrom, dateTo);

                var paketphysical_8 = packs_charges.PacksCardsCharges(pack8, dateFrom, dateTo);
    
                var pack8charg= packs_charges_coeff.PacksCardsChargesCoeff(pack8, dateFrom, dateTo, share8price, service_days);

                var pack8sum15 = paketphysical_8 - pack8charg;
       
                var pack12charg = packs_charges.PacksCardsCharges(pack12, dateFrom, dateTo);
   
                var pack15charg = packs_charges.PacksCardsCharges(pack15, dateFrom, dateTo);

                var pack15coeffcharg = packs_charges_coeff.PacksCardsChargesCoeff(pack15, dateFrom, dateTo, share15price, service_days); 

                pack8charg = pack8charg + (pack15charg - pack15coeffcharg);
                pack15charg = pack15coeffcharg + pack8sum15;

                var PhysicalContraqt = pack12.Where(c => c.CardLogs.Any(cc => cc.Status == CardLogStatus.Montage && cc.Date >= dateFrom && cc.Date <= dateTo)).ToList();
                // იურდიული პირები
                cardsJuridical = CardAbonentPack.Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo) && c.Customer.Type == CustomerType.Juridical).ToList();
                var pack8_Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();
                var pack15_Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price == 15)).Select(c => c).ToList();
                var packexpectJuridical = packs_execept.Packs_Except_Charges(pack8_Juridical, pack15_Juridical, dateTo);

                pack8_Juridical = packexpectJuridical.packs8;
                pack15_Juridical = packexpectJuridical.packs15;

                var packsphysical_8Juridical = packs_charges.PacksCardsCharges(pack8_Juridical, dateFrom, dateTo);
   
                var pack8chargJuridical = packs_charges_coeff.PacksCardsChargesCoeff(pack8_Juridical, dateFrom, dateTo, share8price, service_days);

                var pack8sum15Juridical = packsphysical_8Juridical - pack8chargJuridical;

                var pack12_Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();
                
                var pack12chargJuridical = packs_charges.PacksCardsCharges(pack12_Juridical, dateFrom, dateTo);

               
                var pack15chargJuridical = packs_charges.PacksCardsCharges(pack15_Juridical, dateFrom, dateTo);//pack15statusJuridical.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();

                var pack15coeffchargJuridical = packs_charges_coeff.PacksCardsChargesCoeff(pack15_Juridical, dateFrom, dateTo, share15price, service_days);//pack15coeffJuridical.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();

                pack8chargJuridical = pack8chargJuridical + (pack15chargJuridical - pack15coeffchargJuridical);
                pack15chargJuridical = pack15coeffchargJuridical + pack8sum15Juridical;

                var JuridicalContraqt = pack12_Juridical.Where(c => c.CardLogs.Any(cc => cc.Status == CardLogStatus.Montage && cc.Date >= dateFrom && cc.Date <= dateTo)).ToList();
                //ViewBag.StandardPacks_income = Math.Round((double)((pack12charg) + (pack15charg - (pack15charg * (20m / 100m))) + ((employee6 - (employee6 * (50m / 100m))) * 3)) / Dagger, 2);
                //ViewBag.StandardPacks_incomeDagger = Math.Round((double)ViewBag.StandardPacks_income * Dagger_018, 2);
                //ViewBag.RegionPacks = Math.Round((double)(pack8charg - (pack8charg * (37.5m / 100m))) / Dagger, 2);
                //ViewBag.RegionPacksDangger = Math.Round((double)ViewBag.RegionPacks * Dagger_018, 2);
                //ViewBag.StandardPacks_income_Jurdical = Math.Round((double)((pack12chargJuridical) + (pack15chargJuridical - (pack15chargJuridical * (20m / 100m)))) / Dagger, 2);
                //ViewBag.StandardPacks_income_JurdicalDagger = Math.Round((double)ViewBag.StandardPacks_income_Jurdical * Dagger_018, 2);
                //ViewBag.RegionPacksJuridical = Math.Round((double)(pack8chargJuridical - (pack8chargJuridical * (37.5m / 100m))) / Dagger, 2);
                //ViewBag.RegionPacksJuridicalDangger = Math.Round((double)ViewBag.RegionPacksJuridical * Dagger_018, 2);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                //თანამშრომელი
                //var packemployee = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c).ToList();
                //var employee6 = packs_charges_coeff.PacksCardsChargesCoeff(packemployee, dateFrom, dateTo, share6price, service_days);
                //var packsRent6Sum = packs_charges_rent.PacksCardsChargesRent(packemployee, dateFrom, dateTo);
                //employee6 = (employee6 - packsRent6Sum);
                var emploe6 = (employee6 - (employee6 * (50m / 100m)))+((employee6 - (employee6 * (50m / 100m)))*4);
                ViewBag.StandardPacks_income = Math.Round((double)((pack12charg) + (pack15charg) + (emploe6) + (PhysicalContraqt.Count() * 87)) / Dagger, 2);
                ViewBag.StandardPacks_incomeDagger = Math.Round((double)ViewBag.StandardPacks_income * Dagger_018, 2);
                ViewBag.RegionPacks = Math.Round((double)(pack8charg) / Dagger, 2);
                ViewBag.RegionPacksDangger = Math.Round((double)ViewBag.RegionPacks * Dagger_018, 2);
                ViewBag.StandardPacks_income_Jurdical = Math.Round((double)((pack12chargJuridical) + (pack15chargJuridical) + (JuridicalContraqt.Count() * 87)) / Dagger, 2);
                ViewBag.StandardPacks_income_JurdicalDagger = Math.Round((double)ViewBag.StandardPacks_income_Jurdical * Dagger_018, 2);
                ViewBag.RegionPacksJuridical = Math.Round((double)(pack8chargJuridical) / Dagger, 2);
                ViewBag.RegionPacksJuridicalDangger = Math.Round((double)ViewBag.RegionPacksJuridical * Dagger_018, 2);
            }
            return View();
        }
    }
    public class PacksCharges
    {
        public decimal PacksCardsCharges(List<Card> CardAbonentPack,DateTime dateFrom,DateTime dateTo)
        {
             var packs_charges= CardAbonentPack.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Daily || cc.Status == CardChargeStatus.PreChange || cc.Status == CardChargeStatus.PacketChange).Select(cc => cc).ToList()).ToList();
             return packs_charges.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }

    public class PacksChargesCoeff
    {
        public decimal PacksCardsChargesCoeff(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo,decimal sahre_price, decimal service_days )
        {
            var packs_charges_coeff = CardAbonentPack.Select(s => s.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && Math.Round(cc.Amount, 2) == Math.Round((decimal)(sahre_price / (decimal)service_days), 2) && cc.Status == CardChargeStatus.Daily || cc.Status == CardChargeStatus.PreChange || cc.Status == CardChargeStatus.PacketChange)).ToList();
            return packs_charges_coeff.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }
    public class PackList
    {
        public List<Card> packs8 { get; set; }
        public List<Card> packs15 { get; set; }
    }
    public class Packdecimal
    {
        public decimal packsPromo { get; set; }
        public decimal packsRent { get; set; }
        public decimal Pack15FalsePromo { get; set; }
        public decimal Pack15FalseRentPromo { get; set; }
    }
    public class PacksExcept
    {
        public PackList Packs_Except_Charges(List<Card> pack8, List<Card> pack15, DateTime dateTo)
        {
            PackList pack = new PackList();
            var packsFalse8 = pack15.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateTo && s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Price == 15))).Select(c => c).ToList();
            var packexpect8 = packsFalse8.Where(c => c.Subscribtions.Any(s => s.Tdate <= dateTo && s.SubscriptionPackages.Any(sb => sb.Package.Name == "აქცია 8"))).Select(c => c).ToList();

            var packsFalse15 = pack8.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateTo && s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Name == "აქცია 8"))).Select(c => c).ToList();
            var packexpect15 = packsFalse15.Where(c => c.Subscribtions.Any(s => s.Tdate <= dateTo && s.SubscriptionPackages.Any(sb => sb.Package.Price == 15))).Select(c => c).ToList();

            pack8 = pack8.Except(packsFalse15).ToList();
            pack8.AddRange(packexpect8);
            pack15 = pack15.Except(packexpect8).ToList();
            pack15.AddRange(packexpect15);
            pack.packs8 = pack8;
            pack.packs15 = pack15;
            return pack;
        }

        public PackList _Packs_Except_Charge(List<Card> pack8, List<Card> pack15, DateTime dateTo)
        {
            PackList pack = new PackList();
            var packsFalse8 = pack15.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateTo && s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Price == 15))).Select(c => c).ToList();
            var packexpect8 = packsFalse8.Where(c => c.Subscribtions.Any(s => s.Tdate <= dateTo && s.SubscriptionPackages.Any(sb => sb.Package.Name == "პრომო პაკეტი"))).Select(c => c).ToList();

            var packsFalse15 = pack8.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateTo && s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Name == "პრომო პაკეტი"))).Select(c => c).ToList();
            var packexpect15 = packsFalse15.Where(c => c.Subscribtions.Any(s => s.Tdate <= dateTo && s.SubscriptionPackages.Any(sb => sb.Package.Price == 15))).Select(c => c).ToList();

            pack8 = pack8.Except(packsFalse15).ToList();
            pack8.AddRange(packexpect8);
            pack15 = pack15.Except(packexpect8).ToList();
            pack15.AddRange(packexpect15);
            pack.packs8 = pack8;
            pack.packs15 = pack15;
            return pack;
        }
        public Packdecimal _Packs_Except_Promo(List<Card> packPromo,List<Card> Pack15, DateTime dateFrom, DateTime dateTo)
        {
            Packdecimal pack = new Packdecimal();
            _PacksCharges _PacksCharge = new _PacksCharges();
            PacksChargesRent packs_charges_rent = new PacksChargesRent();
            //var packsP15 = Pack15.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateFrom && s.Tdate <= dateTo && s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Name == "სტანდარტი+ქირა" || sb.Package.Name == "აქცია 8 აქტივაცია"))).Select(c => c).ToList();
            var packsProm = Pack15.Where(c => c.Subscribtions.Any(s => /*s.Tdate >= dateFrom && s.Tdate <= dateTo &&*/ s.SubscriptionPackages.Any(sb => sb.Package.Name == "პრომო პაკეტი"))).Select(c => c).ToList();

            var packsFalsePomo = packPromo.Where(c => c.Subscribtions.Any(s => /*s.Tdate >= dateFrom && s.Tdate<=dateTo &&*/ s.Status == true && s.SubscriptionPackages.Any(sb => sb.Package.Name == "პრომო პაკეტი"))).Select(c => c).ToList();
            var packexpect15 = packPromo.Where(c => c.Subscribtions.Any(s => /*s.Tdate >= dateFrom && s.Tdate <= dateTo &&*/ s.SubscriptionPackages.Any(sb => sb.Package.Name == "სტანდარტი+ქირა" || sb.Package.Name == "აქცია 8 აქტივაცია"))).Select(c => c).ToList();

            //var sum1_15 = _PacksCharge.PacksCardsCharges(packsP15, dateFrom, dateTo);
            var sum1_promo= _PacksCharge.PacksCardsCharges(packsProm, dateFrom, dateTo);

            var sum1_promoRent = packs_charges_rent.PacksCardsChargesRent(packsProm, dateFrom, dateTo);

            var sum2_15 = _PacksCharge.PacksCardsCharges(packexpect15, dateFrom, dateTo);
            var sum2_15Ren = packs_charges_rent.PacksCardsChargesRent(packexpect15, dateFrom, dateTo);
            var sum2_promo = _PacksCharge.PacksCardsCharges(packsFalsePomo, dateFrom, dateTo);

            var sum2_15Rent= packs_charges_rent.PacksCardsChargesRent(packsProm, dateFrom, dateTo);
            var sum2_promoRent = packs_charges_rent.PacksCardsChargesRent(packsFalsePomo, dateFrom, dateTo);
            //pack.packs15 = sum1_15 - sum1_promo;
            pack.packsPromo = (sum2_promo - sum2_15)+ sum1_promo;
            pack.packsRent = (sum2_promoRent - sum2_15Rent) + sum1_promoRent;
            //pack.Pack15FalsePromo = sum2_15;
            //pack.Pack15FalseRentPromo = sum2_15Ren;
            return pack;
        }
    }
}

