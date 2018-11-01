using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Reflection;
using DigitalTVBilling.Juridical;
using DigitalTVBilling.Infrastructure.Juridical;
using System.Configuration;
using DigitalTVBilling.Docs.Contracts;
using DigitalTVBilling.Infrastructure.Juridical.JuridicalDocs;

namespace DigitalTVBilling.Controllers
{
    public class JuridicalController : Controller
    {
        private int pageSize = 20;
        // GET: Accountant
        public async System.Threading.Tasks.Task<ActionResult> Index(JuridicalFilters filter)
        {
            if (!Utils.Utils.GetPermission("JURIDICAL_SHOW"))
            {
                return new RedirectResult("/Main");
            }
            using (DataContext _db = new DataContext())
            {
                JuridicalPresentation juridicalPresentation = new JuridicalPresentation();
                JuridicalModel juridicalModel = new JuridicalModel();
                juridicalModel = await juridicalPresentation.EndJuridical(filter);
                ViewBag.Models = juridicalModel;
                return View(juridicalModel.juridicalLists);
            }
            //return View(
            //        new JuridicalViewModel(
            //                new JuridicalModelData(
            //                        new SqlConnection(
            //                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
            //                        new FilterData(
            //                                new Infrastructure.Juridical.JuridicalFilter(
            //                                        filter,
            //                                        new JuridicalWhereInfo(
            //                                                filter
            //                                            )
            //                                    )
            //                            ),
            //                        new Infrastructure.Juridical.JuridicalFilter(
            //                                filter,
            //                                new JuridicalWhereInfo(
            //                                        filter
            //                                    )
            //                            )


            //                    ),
            //                 new JuridicalStatusList(
            //                        new SqlConnection(
            //                            ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
            //                     ),
            //                     filter
            //            ).Result()

            //    );
        }

        [HttpPost]
        public async Task<PartialViewResult> GetStatusInfo(int card_id)
        {
            return PartialView(
                "~/Views/Juridical/_StatusInfo.cshtml",
                        new JuridicalStatusView(
                              card_id,
                              new SqlConnection(
                                  ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                            ).Result()
                        );
        }
        [HttpPost]
        public async Task<PartialViewResult> _Logging(int card_id)
        {
            return PartialView(
                "~/Views/Juridical/_logging.cshtml",
                    new JuridicalLogginView(
                            card_id,
                            new SqlConnection(
                                ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                            ).Result()
                        );
        }

        [HttpPost]
        public JsonResult SaveStatus(StatusInfo statusInfo)
        {
            if (!Utils.Utils.GetPermission("JURIDICAL_STATUS_CHANGE"))
            {
                return Json(0);
            }

            return Json(
                    new SaveStatusView(
                           statusInfo,
                            ((User)Session["CurrentUser"]).Id,
                            new Infrastructure.Juridical.SaveStatus.CardInfo(
                                new SqlConnection(
                                ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                statusInfo.id
                                ),
                            new Infrastructure.Juridical.SaveStatus.UpdateDeleteStatus(
                                new DataContext(),
                                statusInfo
                                ),
                           new DataContext()
                        ).Result()
                     );

        }

        [HttpPost]
        public JsonResult DowloadDocs(string filePath)
        {
            var FolderName = (@"C:\Xelshekruleba\" + filePath.ToString());
            System.Diagnostics.Process.Start(FolderName + '\\' + "" + filePath + ".pdf");
            var FolderNameDanarti = (@"C:\Xelshekruleba\" + filePath.ToString()+" danarti");
            System.Diagnostics.Process.Start((@"C:\Xelshekruleba\" + filePath.ToString()+" danarti") + '\\' + "" + filePath + " danarti" + ".pdf");
            return null;


        }
        [HttpPost]
        public JsonResult Dowload(Abonent abonent)
        {
            new GeneratorDocs(
                    new RenderViewString(
                            "Contracts",
                            new DocsName(
                                abonent.Customer.Type.ToString(),
                                ""
                             ).ReturnDocumentType(),
                            new DocumentSubscriptionModel(
                                new AbonentGenaratorDate(
                                    new SqlConnection(
                                         ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                            abonent
                                ).Result(),
                            new ConvertImageBase(
                                  abonent.Customer.signature
                                    )
                            ).ResultImage()
                        ),
                    (abonent.Customer.Name + " " + abonent.Customer.LastName)

                ).Result();
            return null;

        }
        public void JuridicalDocs(Abonent abonent, string pack_name)
        {
            new GeneratorDocs(
                    new RenderViewString(
                            "Contracts",
                            new DocsName(
                                abonent.Customer.Type.ToString(),
                                pack_name
                             ).ReturnDocumentType(),
                            new DocumentSubscriptionModel(
                                new AbonentGenaratorDate(
                                    new SqlConnection(
                                         ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
                                            abonent
                                ).Result(),
                            new ConvertImageBase(
                                  abonent.Customer.signature
                                    )
                            ).ResultImage()
                        ),
                    (abonent.Customer.Name + " " + abonent.Customer.LastName)

                ).Result();
        }
        public void JuridicalDocsAttachment(Abonent abonent, string pack_name)
        {
            new GeneratorDocs(
                    new RenderViewString(
                            "Contracts",
                            new DocsAttachment(
                                abonent.Customer.Type.ToString(),
                                pack_name
                             ).ReturnDocumentType(),
                            new DocummentAttachmentModel(
                                new AbonentGenaratorAttachment(
                                            abonent
                                ).Result(),
                            new ConvertImageBase(
                                  abonent.Customer.signature
                               )
                            ).ResultImage()
                        ),
                    (abonent.Customer.Name + " " + abonent.Customer.LastName+ " danarti")

                ).Result();
        }
        //public void JuridicalDocsAppendix(string code, string lastname_name, string type, string pack_name)
        //{
        //    new GeneratorDocs(
        //            new RenderViewString(
        //                    "Contracts",
        //                   "~/Views/Juridical/Contracts/danarti.cshtml",
        //                    new ConvertImageBase64(
        //                        new AbonentDateAppendix(
        //                            new SqlConnection(
        //                                 ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString),
        //                                      ""
        //                        ).Result()
        //                    ).ResultImage()
        //                ),
        //            lastname_name

        //        ).Result();
        //}
        [HttpPost]
        public async Task<PartialViewResult> DocsJuridicalInfos(int card_id)
        {
            return PartialView(
                "~/Views/Juridical/_JuridicalDocsInfo.cshtml",
                            new JuridicalDocsView(
                              card_id,
                              new SqlConnection(
                                  ConfigurationManager.ConnectionStrings["DataConnect"].ConnectionString)
                            ).Result()
                        );

        }


    }
}