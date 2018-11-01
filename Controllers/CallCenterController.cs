using DigitalTVBilling.CallCenter;
using DigitalTVBilling.CallCenter.Infrastructure;
using DigitalTVBilling.Utils;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Controllers
{
    public class CallCenterController : Controller
    {
        public ActionResult Index()
        {
            CallPresentation callPresentationIndex = new CallPresentation();
            return View(
                    new UserViewModel(
                            new Users(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                ),
                            new UserStatic(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                ),
                            new OrderToGo(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                ),
                            new DamageToGo(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                ),
                            new CancellationToGo(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                ),
                            new CityRegionList(
                                  new UserRegionGoOrder(
                                   new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                        ),
                                  new UserRegionGoDamage(
                                       new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                      ),
                                  new UserRegionGoCancellation(
                                       new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                                      )
                                )
                 ).Result()
              );
        }

        public PartialViewResult HistoryShow(FilterUser filterUser)
        {
            return PartialView(
                    "~/Views/CallCenter/_HistoryOrder.cshtml",
                        new HistoryOrderViewModel(
                                new OrderData(
                                    new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                                        ),
                                    new OrderApllication(
                                        filterUser
                                        ).Execute()
                               ),
                               new Users(
                                       new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                                        )
                                   )
                ).Execute()
               );
        }
        public PartialViewResult HistoryShowDamage(FilterUser filterUser)
        {
            return
                PartialView(
                         "~/Views/CallCenter/_HistoryDamage.cshtml",
                         new HistoryDamageViewModel(
                            new DamageData(
                                  new SqlConnection(
                                        ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                   new FilterDamageResult(
                                            filterUser,
                                            new DateFrom()
                                       )
                            ),
                            new Users(
                                       new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                                        )
                                 )
                    ).Execute()
            );
        }
        public PartialViewResult HistoryShowCancellation(FilterUser filterUser)
        {
            return PartialView(
                    "~/Views/CallCenter/_HistoryCancel.cshtml",
                        new HistoryCancellationViewModel(
                                new CancellationData(
                                        new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                                        ),
                                        new CancellationFileter(
                                                filterUser
                                            )
                                    ),
                                 new Users(
                                       new SqlConnection(
                                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString
                                        )
                                 )
                            ).Execute()
                );
        }
        public PartialViewResult OrderCategorized(FilterUser userFilter)
        {
            return PartialView(
             "~/Views/CallCenter/_HistoryOrder.cshtml",
                 new HistoryOrderPartialViewModel(
                     new Orders(
                          new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                    ),
                     new Users(
                             new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                     )
               ).Execute()
         );
        }
        public PartialViewResult DamageCategorized(FilterUser userFilter)
        {
            return PartialView(
             "~/Views/CallCenter/_HistoryDamage.cshtml",
                 new HistoryDamagePartialViewModel(
                     new Damages(
                          new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                    ),
                     new Users(
                             new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                     )
               ).Execute()
         );
        }

        public PartialViewResult CancellationCategorized(FilterUser userFilter)
        {
            return PartialView(
             "~/Views/CallCenter/_HistoryCancel.cshtml",
                 new HistoryCancellationPartialViewModel(
                     new Cancellations(
                          new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                    ),
                     new Users(
                             new SqlConnection(ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                     )
               ).Execute()
         );
        }
    }
}