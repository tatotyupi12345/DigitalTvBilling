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
    public class AccountantPackagesController : Controller
    {
        // GET: AccountantPackages
        public ActionResult Index(int? page)
        {
            if (!Utils.Utils.GetPermission("SHOW_PACKAGES_BY_CHARGE"))
            {
                return new RedirectResult("/Main");
            }

            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
            dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            ViewBag.dtFrom = dateFrom;
            ViewBag.dtTo = dateTo;
            List<Card> cards = null;
            List<Card> cardsPhysical = null;
            List<Card> cardsJuridical = null;
            Checkout packcharges8 = new Checkout();
            Checkout packchargesPromo15 = new Checkout();
            Checkout packcharges12 = new Checkout();
            Checkout packcharges15 = new Checkout();
            decimal Payamount = 0, CanceledCardAmount = 0, servicepakechanges = 0;
            _PacksCharges packs_charges = new _PacksCharges();
            _PacksChargesCoeff packs_charges_coeff = new _PacksChargesCoeff();
            PacksExcept packs_execept = new PacksExcept();
            PacksChargesRent packs_charges_rent = new PacksChargesRent();
            PacksChargesCoeffRent packs_charges_coeff_rent = new PacksChargesCoeffRent();
            PacksFines pack_fines = new PacksFines();
            PacksFinesEmployee pack_fines_employe = new PacksFinesEmployee();
            PacksChargesPreChange packs_charges_pre_change = new PacksChargesPreChange();
            using (DataContext _db = new DataContext())
            {
                decimal share15price = (decimal)_db.Packages.Where(p => p.Name == "აქცია 8 აქტივაცია").FirstOrDefault().Price;
                decimal share8price = (decimal)_db.Packages.Where(p => p.Name == "აქცია 8").FirstOrDefault().Price;
                decimal share6price = (decimal)_db.Packages.Where(p => p.Name == "თანამშრომელი").FirstOrDefault().Price;
                int service_days = Convert.ToInt32(_db.Params.First(p => p.Name == "ServiceDays").Value);

                cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Include("CardCharges").Include("ReturnedCards").Include("Payments").Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo)).ToList();

                var cards6 = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c).ToList();
                ViewBag.cards6 = cards6;
                ViewBag.cards12 = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();

               
               var cardsPromo15= cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "პრომო პაკეტი")).Select(c => c).ToList();
               var cards15 = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name== "სტანდარტი+ქირა" || s.Package.Name== "აქცია 8 აქტივაცია")).Select(c => c).ToList();
                var cards8 = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();

                var packexpectCards = packs_execept.Packs_Except_Charges(cards8, cards15, dateTo);
                cards8 = packexpectCards.packs8;
                cards15 = packexpectCards.packs15;


                var pack15_Sum = cards15.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Math.Round((decimal)(share15price / (decimal)service_days), 2) && cc.Tdate >= dateFrom && cc.Tdate <= dateTo)).ToList();
                var pack8_Sum = cards8.Where(c => c.CardCharges.Any(cc => Math.Round(cc.Amount, 2) == Math.Round((decimal)(share8price / (decimal)service_days), 2) && cc.Tdate >= dateFrom && cc.Tdate <= dateTo)).ToList();
                var pack15_charg = pack8_Sum.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Daily).Select(cc => cc).ToList()).ToList();

                cards15.AddRange(cards8.Except(pack8_Sum).ToList());
                cards8.AddRange(cards15.Except(pack15_Sum).ToList());

                //
                var promopackexpect = packs_execept._Packs_Except_Promo(cardsPromo15, cards15, dateFrom, dateTo);
                //cards15 = promopackexpect.packs15;
                ViewBag.cardsPromo15 = cardsPromo15;
                //

                ViewBag.cards8 = cards8;
                ViewBag.cards15 = cards15;

                ViewBag.pack8charges = pack15_charg.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();


                ViewBag.PackagesByChargeActive = "active";

                // ფიზიკური
                cardsPhysical = cards.Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo) && c.Customer.Type == CustomerType.Physical).ToList();
                var pack8 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();
                var pack12 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();
                var pack15 = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი+ქირა" || s.Package.Name == "აქცია 8 აქტივაცია")).Select(c => c).ToList();
               // var pack15_s= cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price==15)).Select(c => c).ToList();
                var packexpect = packs_execept.Packs_Except_Charges(pack8, pack15, dateTo);
                //var cardsPromo15P = cardsPhysical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "პრომო პაკეტი")).Select(c => c).ToList();
                pack8 = packexpect.packs8;
                pack15 = packexpect.packs15;

                //var promopackexpe = packs_execept._Packs_Except_Charge(cardsPromo15P, pack15, dateTo);
                //cards15 = promopackexpe.packs15;
                //cardsPromo15P = promopackexpect.packs8;

                var pack8charg = packs_charges_coeff.PacksCardsChargesCoeff(pack8, dateFrom, dateTo, share8price, service_days) + (packs_charges.PacksCardsCharges(pack15, dateFrom, dateTo) - packs_charges_coeff.PacksCardsChargesCoeff(pack15, dateFrom, dateTo, share15price, service_days));
                var pack15charg = packs_charges_coeff.PacksCardsChargesCoeff(pack15, dateFrom, dateTo, share15price, service_days) + (packs_charges.PacksCardsCharges(pack8, dateFrom, dateTo) - packs_charges_coeff.PacksCardsChargesCoeff(pack8, dateFrom, dateTo, share8price, service_days));
                var pack12charg = packs_charges.PacksCardsCharges(pack12, dateFrom, dateTo);
                var PromoBonus15 = promopackexpect.packsPromo; /*packs_charges.PacksCardsCharges(cardsPromo15P, dateFrom, dateTo);*/
                // წინასწარი დარიცხვები
                //var pack8_prechange = packs_charges_pre_change.PacksCardsChargesPreChange(pack8, dateFrom, dateTo);
                //var pack12_prechange = packs_charges_pre_change.PacksCardsChargesPreChange(pack12, dateFrom, dateTo);
                //var pack15_prechange = packs_charges_pre_change.PacksCardsChargesPreChange(pack15, dateFrom, dateTo);

                // იჯარა
                var packsRent8Sum = packs_charges_coeff_rent.PacksCardsChargesCoeffRent(pack8, dateFrom, dateTo, share8price, service_days);
                var pack15chargRentSum = packs_charges_rent.PacksCardsChargesRent(pack15, dateFrom, dateTo) + (packs_charges_rent.PacksCardsChargesRent(pack8, dateFrom, dateTo) - packs_charges_coeff_rent.PacksCardsChargesCoeffRent(pack8, dateFrom, dateTo, share8price, service_days));
                var pack12chargRentSum = packs_charges_rent.PacksCardsChargesRent(pack12, dateFrom, dateTo);
                var PromoLease15 = promopackexpect.packsRent; /*packs_charges_rent.PacksCardsChargesRent(cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "პრომო პაკეტი")).Select(c => c).ToList(), dateFrom, dateTo);*/

                ViewBag.CheckoutPhysical = pack_fines.PackChargesFines(cardsPhysical, pack8charg, pack12charg, pack15charg, packsRent8Sum, pack12chargRentSum, pack15chargRentSum, dateFrom, dateTo);
                
                var canceledPhysical = cardsPhysical.Where(cr => cr.ReturnedCards.Any(c => c.Tdate >= dateFrom && c.Tdate <= dateTo)).Select(s => s.Id).ToList();
                CanceledCardAmount += Math.Round(cards.Select(c => c.Payments.Where(p => canceledPhysical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum() - cards.Select(c => c.CardCharges.Where(p => canceledPhysical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum(), 3);

                string[][] arrray = cardsPhysical.Select(s => s.ReturnedCards.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(ss => ss.commission).ToArray()).ToArray();
                Commision _returne_cash = new Commision();
                CashCashless _cash = new CashCashless();
                _cash = _returne_cash.Returned_commision(arrray);
                ViewBag.CashPhysical = _cash.Cash;
                ViewBag.CashlessPhysical = _cash.Cashless;
                // იურდიული
                cardsJuridical = cards.Where(c => c.CardCharges.Any(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo) && c.Customer.Type == CustomerType.Juridical).ToList();
                var pack8Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8")).Select(c => c).ToList();
                var pack12Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "სტანდარტი")).Select(c => c).ToList();
                var pack15Juridical = cardsJuridical.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Price == 15)).Select(c => c).ToList();

                var packexpectJuridical = packs_execept.Packs_Except_Charges(pack8Juridical, pack15Juridical, dateTo);
                pack8Juridical = packexpectJuridical.packs8;
                pack15Juridical = packexpectJuridical.packs15;
                var pack8chargJuridical = packs_charges_coeff.PacksCardsChargesCoeff(pack8Juridical, dateFrom, dateTo, share8price, service_days) + (packs_charges.PacksCardsCharges(pack15Juridical, dateFrom, dateTo) - packs_charges_coeff.PacksCardsChargesCoeff(pack15Juridical, dateFrom, dateTo, share15price, service_days));
                var pack12chargJuridical = packs_charges.PacksCardsCharges(pack12Juridical, dateFrom, dateTo);
                var pack15chargJuridical = packs_charges_coeff.PacksCardsChargesCoeff(pack15Juridical, dateFrom, dateTo, share15price, service_days) + (packs_charges.PacksCardsCharges(pack8Juridical, dateFrom, dateTo) - packs_charges_coeff.PacksCardsChargesCoeff(pack8Juridical, dateFrom, dateTo, share8price, service_days));
                // წინასწარი დარიცხვები
                //var pack8_prechangeJuridical = packs_charges_pre_change.PacksCardsChargesPreChange(pack8Juridical, dateFrom, dateTo);
                //var pack12_prechangeJuridical = packs_charges_pre_change.PacksCardsChargesPreChange(pack12Juridical, dateFrom, dateTo);
                //var pack15_prechangeJuridical = packs_charges_pre_change.PacksCardsChargesPreChange(pack15Juridical, dateFrom, dateTo);

                // იჯარა
                var packsRent8SumJuridical = packs_charges_coeff_rent.PacksCardsChargesCoeffRent(pack8Juridical, dateFrom, dateTo, share8price, service_days);
                var packsRent12SumJuridical = packs_charges_rent.PacksCardsChargesRent(pack12Juridical, dateFrom, dateTo);
                var pack15chargRentSumJuridical = packs_charges_rent.PacksCardsChargesRent(pack15Juridical, dateFrom, dateTo) + (packs_charges_rent.PacksCardsChargesRent(pack8Juridical, dateFrom, dateTo) - packs_charges_coeff_rent.PacksCardsChargesCoeffRent(pack8Juridical, dateFrom, dateTo, share8price, service_days));

                ViewBag.CheckoutJuridical = pack_fines.PackChargesFines(cardsJuridical, pack8chargJuridical, pack12chargJuridical, pack15chargJuridical, packsRent8SumJuridical, packsRent12SumJuridical, pack15chargRentSumJuridical, dateFrom, dateTo);

                packcharges8.Bonus = (pack8charg + pack8chargJuridical) - ((pack8charg + pack8chargJuridical) * (37.5m / 100m));
                packcharges8.Lease = (packsRent8Sum + packsRent8SumJuridical) + (pack8charg - (pack8charg * (62.5m / 100m))) + (pack8chargJuridical - (pack8chargJuridical * (62.5m / 100m)));
                packcharges12.Bonus = pack12charg + pack12chargJuridical;
                packcharges12.Lease = pack12chargRentSum + packsRent12SumJuridical;
                packcharges15.Bonus = (pack15charg + pack15chargJuridical) - ((pack15charg + pack15chargJuridical) * (20m / 100m));
                packcharges15.Lease = (pack15chargRentSum + pack15chargRentSumJuridical) + (pack15charg - (pack15charg * (80m / 100m))) + (pack15chargJuridical - (pack15chargJuridical * (80m / 100m)));

                //var PromoBonus15 = packs_charges.PacksCardsCharges(cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "პრომო პაკეტი")).Select(c => c).ToList(), dateFrom, dateTo);
                //var PromoLease15 = packs_charges_rent.PacksCardsChargesRent(cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "პრომო პაკეტი")).Select(c => c).ToList(), dateFrom, dateTo);
                packchargesPromo15.Bonus = PromoBonus15 - (PromoBonus15 * (20m / 100m));
                packchargesPromo15.Lease = PromoLease15 + (PromoBonus15 - (PromoBonus15 * (80m / 100m)));

                ViewBag.Packcharges8 = packcharges8;
                ViewBag.Packcharges12 = packcharges12;
                ViewBag.Packcharges15 = packcharges15;
                ViewBag.PackchargesPromo15 = packchargesPromo15;

                var canceledJuridical = cardsJuridical.Where(cr => cr.ReturnedCards.Any(c => c.Tdate >= dateFrom && c.Tdate <= dateTo)).Select(s => s.Id).ToList();
                CanceledCardAmount += Math.Round(cardsJuridical.Select(c => c.Payments.Where(p => canceledJuridical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum() - cardsJuridical.Select(c => c.CardCharges.Where(p => canceledJuridical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum(), 3);

                Commision _returne_cashJuridical = new Commision();
                CashCashless _cashJuridical = new CashCashless();

                string[][] arrJuridical = cardsJuridical.Select(s => s.ReturnedCards.Where(c => c.Tdate >= dateFrom && c.Tdate <= dateTo).Select(ss => ss.commission).ToArray()).ToArray();

                _cashJuridical = _returne_cashJuridical.Returned_commision(arrJuridical);
                ViewBag.CashJuridical = _cashJuridical.Cash;
                ViewBag.CashlessJuridical = _cashJuridical.Cashless;
                //  იურდიული

                //თანამშრომელი
                var packemployee = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c).ToList();
                var employee6 = packs_charges_coeff.PacksCardsChargesCoeff(packemployee, dateFrom, dateTo, share6price, service_days);

                var packsRent6Sum = packs_charges_rent.PacksCardsChargesRent(packemployee, dateFrom, dateTo);
                //წინასწარი დარიცხვები
                var pack6_prechange = packs_charges_pre_change.PacksCardsChargesPreChange(packemployee, dateFrom, dateTo);
                //
                ViewBag.Employee = pack_fines_employe.PackChargesFinesEmployee(packemployee, employee6, packsRent6Sum, dateFrom, dateTo);
                //ViewBag.Employee = Employee;

                string[][] arrraytechnical = packemployee.Select(s => s.ReturnedCards.Select(ss => ss.commission).ToArray()).ToArray();
                Commision _returne_cashTechnical = new Commision();
                CashCashless _cashTechnical = new CashCashless();
                _cashTechnical = _returne_cashTechnical.Returned_commision(arrraytechnical);
                ViewBag.CashTechnical = _cashTechnical.Cash;
                ViewBag.CashlessTechnical = _cashTechnical.Cashless;

                var canceledTechnical = cards.Where(c => c.CardStatus == CardStatus.Canceled && c.Tdate >= dateFrom && c.Tdate <= dateTo && c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "თანამშრომელი")).Select(c => c.Id).ToList();
                CanceledCardAmount += Math.Round(cards.Select(c => c.Payments.Where(p => canceledJuridical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum() - cards.Select(c => c.CardCharges.Where(p => canceledJuridical.Contains(p.CardId)).Select(cc => cc.Amount).Sum()).DefaultIfEmpty().Sum(), 3);
                ViewBag.Payamount = CanceledCardAmount;


            }
            return View(cards.ToPagedList(page ?? 1, 20));
        }
    }
    public class Checkout
    {
        public decimal Bonus { get; set; }
        public decimal Lease { get; set; }
        public decimal ReturnComm { get; set; }
        public decimal Pause { get; set; }
        public decimal Blocked { get; set; }
        public decimal AccessoryCharge { get; set; }
    }
    public class CashCashless
    {
        public double Cash { get; set; }
        public double Cashless { get; set; }
    }
    public class Commision
    {
        public CashCashless Returned_commision(string[][] commisiontype)
        {
            CashCashless _returnedcash = new CashCashless();
            JArray amount, commision_type;
            double Cash = 0;
            double Cashless = 0;
            for (int i = 0; i < commisiontype.Length; i++)
            {
                if (commisiontype[i].Length != 0)
                {
                    string arr = commisiontype[i].GetValue(0).ToString();
                    var _returned = commisiontype.GetValue(i);
                    string parse_returned = arr;
                    JObject parsed = JObject.Parse(parse_returned);
                    commision_type = (JArray)parsed["commisionType"];
                    amount = (JArray)parsed["amount"];
                    for (int j = 0; j < amount.Count(); j++)
                    {
                        if (Convert.ToInt32(commision_type[j]) == 2)
                        {
                            Cash += Convert.ToDouble(amount[j]);
                        }
                        if (Convert.ToInt32(commision_type[j]) == 18)
                        {
                            Cashless += Convert.ToDouble(amount[j]);
                        }

                    }
                }
            }
            _returnedcash.Cash = Cash;
            _returnedcash.Cashless = Cashless;
            return _returnedcash;
        }
    }
    public class PacksChargesPreChange
    {
        public decimal PacksCardsChargesPreChange(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo)
        {
            var packs_charges_PreChange = CardAbonentPack.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.PreChange).Select(cc => cc).ToList()).ToList();
            return packs_charges_PreChange.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }
    public class PacksChargesRent
    {
        public decimal PacksCardsChargesRent(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo)
        {
            var packs_charges_rent = CardAbonentPack.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.PenDaily).Select(cc => cc).ToList()).ToList();
            var id = packs_charges_rent.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.CardId).ToList()).ToList();
            return packs_charges_rent.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }

    public class PacksChargesCoeffRent
    {
        public decimal PacksCardsChargesCoeffRent(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo, decimal sahre_price, decimal service_days)
        {
            var packs_charges_coeff_rent = CardAbonentPack.Select(s => s.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && Math.Round(cc.Amount, 2) == Math.Round((decimal)(sahre_price / (decimal)service_days), 2) && cc.Status == CardChargeStatus.PenDaily)).ToList();
            return packs_charges_coeff_rent.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }
    public class PacksFines
    {

        public Checkout PackChargesFines(List<Card> Cards, decimal pack8charg, decimal pack12charg, decimal pack15charg, decimal packsRent8Sum, decimal pack12chargRentSum, decimal pack15chargRentSum,/*decimal PromoBonus15,decimal PromoLease15,*/ DateTime dateFrom, DateTime dateTo)
        {
            Checkout packcharges = new Checkout();
            packcharges.Bonus = (pack8charg - (pack8charg * (37.5m / 100m))) + (pack12charg) /*+ (PromoBonus15 - (PromoBonus15 * (20m / 100m)))*/ + (pack15charg - (pack15charg * (20m / 100m)));
            packcharges.Lease = (pack8charg - (pack8charg * (62.5m / 100m))) + (pack15charg - (pack15charg * (80m / 100m))) /*+ (PromoBonus15 - (PromoBonus15 * (80m / 100m)))*/ + packsRent8Sum + pack15chargRentSum + pack12chargRentSum;

            var Fines = Cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Name == "აქცია 8" || s.Package.Name == "სტანდარტი" || s.Package.Price == 15)).Select(c => c).ToList();

            var ReturnFines = Fines.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.ReturnComm).Select(cc => cc).ToList());
            var PauseFines = Fines.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Pause).Select(cc => cc).ToList());
            var AccessoryChargeFines = Fines.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.AccessoryCharge).Select(cc => cc).ToList());
            var PenFines = Fines.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Pen).Select(cc => cc).ToList());

            packcharges.ReturnComm = ReturnFines.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            packcharges.Pause = PauseFines.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            packcharges.AccessoryCharge = AccessoryChargeFines.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            packcharges.Blocked = PenFines.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();

            return packcharges;
        }
    }
    public class PacksFinesEmployee
    {

        public Checkout PackChargesFinesEmployee(List<Card> packemployee, decimal employee6, decimal packsRent6Sum, DateTime dateFrom, DateTime dateTo)
        {
            Checkout Employee = new Checkout();
            Employee.Bonus = (employee6 - (employee6 * (50m / 100m)));
            Employee.Lease = (employee6 - (employee6 * (50m / 100m))) + packsRent6Sum;

            var ReturnCommFinesemployee = packemployee.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.ReturnComm).Select(cc => cc).ToList());
            var PauseFinesemployee = packemployee.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Pause).Select(cc => cc).ToList());
            var AccessoryChargeFinesemployee = packemployee.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.AccessoryCharge).Select(cc => cc).ToList());
            var BlockedFinesemployee = packemployee.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Pen).Select(cc => cc).ToList());

            Employee.ReturnComm = ReturnCommFinesemployee.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            Employee.AccessoryCharge = AccessoryChargeFinesemployee.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            Employee.Pause = PauseFinesemployee.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
            Employee.Blocked = BlockedFinesemployee.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();

            return Employee;
        }
    }
    public class _PacksCharges
    {
        public decimal PacksCardsCharges(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo)
        {
            var packs_charges = CardAbonentPack.Select(c => c.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && cc.Status == CardChargeStatus.Daily || cc.Status == CardChargeStatus.PreChange || cc.Status == CardChargeStatus.PacketChange).Select(cc => cc).ToList()).ToList();
            return packs_charges.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }

    public class _PacksChargesCoeff
    {
        public decimal PacksCardsChargesCoeff(List<Card> CardAbonentPack, DateTime dateFrom, DateTime dateTo, decimal sahre_price, decimal service_days)
        {
            var packs_charges_coeff = CardAbonentPack.Select(s => s.CardCharges.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo && Math.Round(cc.Amount, 2) == Math.Round((decimal)(sahre_price / (decimal)service_days), 2) && cc.Status == CardChargeStatus.Daily || cc.Status == CardChargeStatus.PreChange || cc.Status == CardChargeStatus.PacketChange)).ToList();
            return packs_charges_coeff.Select(c => c.Where(cc => cc.Tdate >= dateFrom && cc.Tdate <= dateTo).Select(s => s.Amount).Sum()).Sum();
        }
    }
}