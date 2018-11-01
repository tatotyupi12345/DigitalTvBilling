using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalTVBilling.Models;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Utils;
using PagedList;
using PagedList.Mvc;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DigitalTVBilling.Controllers
{
    public class CardStatController : Controller
    {
        // GET: CardStat
        public ActionResult Index(int? page, FilterDetails filters)
        {
            if (!Utils.Utils.GetPermission("CARDS_STATUS_SHOW"))
            {
                return new RedirectResult("/Main");
            }

            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now; ;
            if (filters.date_from != null && filters.date_to != null)
            {
                dateFrom = Utils.Utils.GetRequestDate(filters.date_from, true);
                dateTo = Utils.Utils.GetRequestDate(filters.date_to, false);
            }
            else
            {
                dateFrom = Utils.Utils.GetRequestDate(Request["dt_from"], true);
                dateTo = Utils.Utils.GetRequestDate(Request["dt_to"], false);
            }

            List<CardStat> cardstats = new List<CardStat>();
            List<Card> cards;
            List<Logging> _loggings;
            List<User> _users;
            List<SellerObject> _sellers;
            





            using (DataContext _db = new DataContext())
            {
                //var __cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").Where(c => c.CardStatus != CardStatus.Canceled).ToList();
                //__cards = __cards.Where(c => c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Where(s => s.PackageId == 304085).ToList().Count > 0).ToList();
                //List<Customer> customers = new List<Customer>();


                //CASSocket _socket = new CASSocket() { IP = "192.168.4.143", Port = 8000 };
                //_socket.Connect();

                //foreach (var item in __cards)
                //{
                //    if (customers.Exists(c => c.Id == item.Customer.Id))
                //    {

                //    }
                //    else
                //    {
                //        customers.Add(item.Customer);
                //    }

                //    if (!_socket.SendOSDRequest(int.Parse(item.CardNum), "თუ გსურთ პაკეტის გაუმჯობესება და ყველა კოდირებული არხის ჩართვა, დაგვიკავშირდით ნომერზე: 032 205 12 12", DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), 0))
                //    {
                //        return Json(0);
                //    }


                //}

                //_socket.Disconnect();


                //foreach (var item in customers)
                //{
                //    Task.Run(async () => { await Utils.Utils.sendMessage(item.Phone1, "Tu gsurt paketis gaumjobeseba da yvela kodirebuli arxis chartva, dagvikavshirdit nomerze: 032 205 12 12"); }).Wait();
                //}




                cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).Where(c=> c.CardStatus != CardStatus.Canceled /*&& c.Tdate >= dateFrom && c.Tdate <= dateTo*/).ToList();
                
                foreach (var item in cards)
                {
                    item.CardLogs = _db.CardLogs.Where(l => l.CardId == item.Id).ToList();
                }

                //cards = cards.Where(c => (c.CardLogs.Where(l => l.Date < dateFrom).Count() > 0 && c.CardLogs.Where(l => l.Date < dateFrom).Last().Status == (CardLogStatus)filters.logStatus) || c.CardLogs.Where(l => l.Date >= dateFrom && l.Date <= dateTo).Any(l => l.Status == CardLogStatus.Open)).ToList();
                //int count = 
                //cards.Where(c => c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Any(s => s.Package.Id == 304085) && c.Tdate >= dateFrom && c.Tdate <= dateTo).ToList().ForEach(c => c.Subscribtions.Where(s => s.Status == true).FirstOrDefault().SubscriptionPackages.Where(s => s.Package.Id == 304085).FirstOrDefault().Package = _db.Packages.Where(p=>p.Id == 304084).FirstOrDefault());

                //cards = _db.Cards.Include("Customer").Include("Subscribtions.SubscriptionPackages.Package").OrderByDescending(c => c.Id).ToList();
                _loggings = _db.Loggings.Where(l => l.Mode == LogMode.Add && l.Type == LogType.Card).ToList();
                _users = _db.Users.Include("UserType").ToList();
                _sellers = _db.Seller.ToList();

                if (filters!= null)
                {
                    if(filters.abonent != null)
                    {
                        if(filters.abonent_select != null)
                        {
                            switch (filters.abonent)
                            {
                                case -1:
                                    {

                                    }
                                    break;
                                case 0:
                                    {
                                        cards = cards.Where(c => c.City.Contains(filters.abonent_select)).ToList();
                                    }
                                    break;

                                case 1:
                                    {
                                        if (cards.Any(c => c.Village != null))
                                            cards = cards.Where(c => c.Village != null && (c.Village.Contains(filters.abonent_select) || c.Village == filters.abonent_select)).ToList();
                                        else
                                            cards = new List<Card>();
                                    }
                                    break;
                                case 2:
                                    {
                                        if(filters.abonent_select != "-1")
                                        cards = cards.Where(c => c.Region != null && c.Region.Contains(filters.abonent_select)).ToList();
                                    }
                                    break;
                                case 3:
                                    {
                                        cards = cards.Where(c => c.Customer.Name.Contains(filters.abonent_select.Split(' ').First()) || c.Customer.LastName.Contains(filters.abonent_select.Split(' ').Last())).ToList();
                                    }
                                    break;
                                case 4:
                                    {
                                        cards = cards.Where(c => c.Customer.Code.Contains(filters.abonent_select)).ToList();
                                    }
                                    break;
                                case 5:
                                    {
                                        cards = cards.Where(c => c.Customer.Phone1.Contains(filters.abonent_select)).ToList();
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    if(filters.abonentType != null)
                    {
                        switch (filters.abonentType)
                        {
                            case -1:
                                {

                                }
                                break;

                            default:
                                cards = cards.Where(c => c.Customer.Type == (CustomerType)filters.abonentType).ToList();
                                break;
                        }
                    }
                    
                    if (filters.abonentStatus != null)
                    {
                        switch (filters.abonentStatus)
                        {
                            case -1:
                                {

                                }
                                break;
                            
                            default:
                                cards = cards.Where(c => c.CardStatus == (CardStatus)filters.abonentStatus).ToList();
                                break;
                        }
                    }

                    if (filters.logStatus != null)
                    {
                        switch (filters.logStatus)
                        {
                            case -1:
                                {

                                }
                                break;

                            default:
                                cards = cards.Where(c => (c.CardLogs.Where(l => l.Date < dateFrom).Count() > 0 && c.CardLogs.Where(l => l.Date < dateFrom).Last().Status == (CardLogStatus)filters.logStatus) || c.CardLogs.Where(l => l.Date >= dateFrom && l.Date <= dateTo).Any(l => l.Status == (CardLogStatus)filters.logStatus)).ToList();
                                break;
                        }
                    }

                    if (filters.package != null)
                    {
                        switch (filters.package)
                        {
                            case -1:
                                {

                                }
                                break;

                            default:
                                {
                                    //List<Card> _cards = new List<Card>(cards);
                                    //cards = cards.Where(c => c.Subscribtions.Where(s => s.Status == true).First().SubscriptionPackages.Any(s => s.Package.Id == filters.package)).ToList();


                                    List<Card> curr_package = null;
                                    if (cards.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateFrom && s.Tdate <= dateTo)).ToList().Count() > 0)
                                    {
                                        curr_package = new List<Card>(cards.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateFrom && s.Tdate <= dateTo)).ToList());
                                        if(curr_package.Count > 0)
                                        //cards = cards.Where(c => c.Subscribtions.Any(s => s.Tdate >= dateFrom && s.Tdate <= dateTo)).ToList();
                                        curr_package = curr_package.Where(c => (c.Subscribtions.Where(s => s.Tdate >= dateFrom && s.Tdate <= dateTo).First().SubscriptionPackages.Any(sb => sb.PackageId == filters.package) /*|| c.Subscribtions.Where(s => s.Tdate < dateFrom).Last().SubscriptionPackages.Any(sb => sb.PackageId == filters.package)*/)).ToList();
                                    }
                                    else
                                        curr_package = new List<Card>();

                                    cards = cards.Where(c => c.Subscribtions.Any(s => s.Tdate < dateFrom)).ToList();
                                    cards = cards.Where(c => (c.Subscribtions.Last().SubscriptionPackages.Any(sb => sb.PackageId == filters.package) /*|| c.Subscribtions.Where(s => s.Tdate < dateFrom).Last().SubscriptionPackages.Any(sb => sb.PackageId == filters.package)*/)).ToList();
                                    cards = cards.Except(curr_package).ToList();
                                    cards.AddRange(curr_package);

                                    //cards = cards.Except(cards).ToList();
                                }
                                break;
                        }
                    }

                    if(filters.saleType != null)
                    {
                        
                        switch (filters.saleType)
                        {
                            case -1:
                                {

                                }
                                break;

                            case 0:
                                {
                                    int[] userids = _users.Where(u => u.Type == 18).Select(u => u.Id).ToArray();
                                    cards = cards.Where(c => userids.Contains(c.UserId)).ToList();
                                }
                                break;

                            case 1:
                                {
                                    int[] userids = _users.Where(u => u.Type == 4).Select(u => u.Id).ToArray();
                                    cards = cards.Where(c => userids.Contains(c.UserId)).ToList();
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    if(filters.region != null)
                    {
                        switch (filters.region)
                        {
                            case "-1":
                                {
                                    
                                }
                                break;

                            default:
                                {
                                    int[] sellerids = _sellers.Where(s => s.region != null && s.region.Contains(filters.region)).Select(s => s.ID).ToArray();
                                    int[] user_ids = _users.Where(u => u.@object != null && sellerids.Contains((int)u.@object)).Select(u=>u.Id).ToArray();
                                    cards = cards.Where(c => user_ids.Contains(c.UserId)).ToList();
                                }
                                break;
                        }
                        
                    }
                }
                foreach (var card in cards)
                {
                    //if (card.Customer.Type == CustomerType.Technic)
                    //    continue;
                    //if (card.Tdate >= dateFrom && card.Tdate <= dateTo)
                    {
                        cardstats.Add(new CardStat() { card = card, });
                    }
                }
                int rownum = cardstats.Count;
                foreach (var cardstat in cardstats)
                {
                    cardstat.rowNumber =  rownum--;
                    cardstat.customer = cardstat.card.Customer;
                    cardstat.subscribe = cardstat.card.Subscribtions.Where(s=>s.Status == true).First();// _db.Subscribtions.Where(s => s.CardId == cardstat.card.Id && s.Status == true).First();
                    cardstat.subscribePackages = cardstat.subscribe.SubscriptionPackages.ToList();// _db.SubscriptionPackages.Where(s => s.SubscriptionId == cardstat.subscribe.Id).ToList();
                    cardstat.logging = _loggings.Where(l => (l.TypeId == cardstat.card.Id || l.TypeValue == cardstat.card.AbonentNum)).FirstOrDefault();
                    cardstat.user = _users.Where(u => u.Id == cardstat.logging.UserId).First();
                    cardstat.seller = _sellers.Where(s => s.ID == cardstat.user.@object).FirstOrDefault();
                    cardstat.userType = cardstat.user.UserType;// _db.UserTypes.Where(u => u.Id == cardstat.user.Type).FirstOrDefault();
                    //int[] arr = cardstat.subscribePackages.Select(s => s.PackageId).ToArray();
                    cardstat.packages = cardstat.subscribePackages.Select(s => s.Package).ToList();// _db.Packages.Where(p => arr.Contains(p.Id)).ToList();
                }
            }
            var AbonentValue = filters.abonent == -1 ? "ყველა" : filters.abonent == 0 ? "ქალაქი" : filters.abonent == 1 ? "სოფელი" : filters.abonent == 2 ? "რეგიონი" : filters.abonent == 3 ? "სახელი" : filters.abonent == 4 ? "პ/ნ" : filters.abonent == 5 ? "ტელეფონი" : "No";
            var AbonentType = filters.abonentType == -1 ? "ყველა" : filters.abonentType == 0 ? "ფიზიკური" : filters.abonentType == 1 ? "იურიდიული" : filters.abonentType == 2 ? "ტექნიკური" : "No";
            var AbonentStatus = filters.abonentStatus == -1 ? "ყველა" : filters.abonentStatus == 0 ? "აქტიური" : filters.abonentStatus == 1 ? "გათიშული" : filters.abonentStatus == 2 ? "დაპაუზებული" : filters.abonentStatus == 3 ? "მონტაჟი" : filters.abonentStatus == 4 ? "გაუქმებული" : filters.abonentStatus == 5 ? "დაბლოკილი" : filters.abonentStatus == 6 ? "უფასო დღეები" : "No";
            var AbonentPackage = filters.package == -1 ? "ყველა" : filters.package == 0 ? "თანამშრომელი" : filters.package == 1 ? "სტანდარტი" : filters.package == 2 ? "სტანდარტი+ქირა" : filters.package == 3 ? "აქცია 8 აქტივაცია" : filters.package == 4 ? "აქცია 8" : "No";
            var AbonentSale = filters.saleType == -1 ? "ყველა" : filters.saleType == 0 ? "დილერი" : filters.saleType == 1 ? "გარე გაყიდვები" : "No";
            var AbonentRegion = filters.region == null ? "ყველა" : filters.region == "-1" ? "ყველა" : filters.region; // == "0" ? "სამეგრელო - ზემო სვანეთი" : filters.region == "1" ? "აფხაზეთის ა.რ." : filters.region == "2" ? "აჭარის ა.რ." : filters.region == "3" ? "გურია" : filters.region == "4" ? "თბილისი" : filters.region == "5" ? "იმერეთი" : filters.region == "6" ? "კახეთი" : filters.region == "7" ? "მცხეთა-მთიანეთი" : filters.region == "8" ? "რაჭა-ლეჩხუმი და ქვემო სვანეთი" : filters.region == "9" ? "სამცხე-ჯავახეთი" : filters.region == "10" ? "ქვემო ქართლი" : filters.region == "11" ? "შიდა ქართლი" : "No";
            ViewBag.AbonentFilter = new string[] { dateFrom.ToString(), dateTo.ToString(), AbonentValue, AbonentType, AbonentStatus, AbonentPackage, AbonentSale, AbonentRegion };
            return View(cardstats.ToPagedList(page ?? 1, 20));
        }

        [HttpPost]
        public PartialViewResult GetDetailFilterModal()
        {
            using (DataContext _db = new DataContext())
            {
                ViewBag.Towers = _db.Towers.Select(c => new IdName { Id = c.Id, Name = c.Name }).OrderBy(r => r.Name).ToList();
                ViewBag.Receivers = _db.Receivers.Select(c => new IdName { Id = c.Id, Name = c.Name }).ToList();
                ViewBag.Packages = _db.Packages.Where(p => p.RentType != RentType.technic && p.RentType != RentType.block).ToList();

                List<string> regions = new List<string>();
                XmlDocument doc1 = new XmlDocument();
                doc1.Load(Server.MapPath("~/App_Data/city_xml.xml"));
                XmlNodeList elemList = doc1.GetElementsByTagName("region");
                for (int i = 0; i < elemList.Count; i++)
                {
                    //Console.WriteLine(elemList[i].InnerXml);
                    if (regions.Contains(elemList[i].InnerXml))
                        continue;
                    regions.Add(elemList[i].InnerXml);
                }
                ViewBag.regions = regions;

                return PartialView("~/Views/CardStat/_CardStatDetailFilter.cshtml");
            }
        }
    }

   
}