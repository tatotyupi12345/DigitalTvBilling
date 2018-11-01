using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    //public class JuridicalList
    //{
    //    public int Id { get; set; }
    //    public DateTime Tdate { get; set; }
    //    public string Name { get; set; }
    //    public string Code { get; set; }
    //    public string Abonent_Num { get; set; }
    //    public string UsName { get; set; }
    //    public string UsType { get; set; }
    //    public int JuridicalVerifical { get; set; }
    //    public string City { get; set; }
    //    public string Phone { get; set; }
    //    public string Num { get; set; }
    //    public string CardNum { get; set; }
    //    public string DocNum { get; set; }
    //    public string ActivePacket { get; set; }
    //    public CardStatus Status { get; set; }
    //    public int user_id { get; set; }
    //}
    public enum JuridicalVerifyStatus
    {
        [Description("სტატუსის გარეშე")]
        [Display(Name = "სტატუსის გარეშე")]
        None = -1,
        [Description("ჩაბარებული")]
        [Display(Name = "ჩაბარებული")]
        Delivered,
        [Description("გავლილი")]
        [Display(Name = "გავლილი")]
        Passed,
        [Description("პრობლემური გამოსწორებადი")]
        [Display(Name = "პრობლემური გამოსწორებადი")]
        FixableProblematic,
        [Description("პრობლემური გავლილი")]
        [Display(Name = "პრობლემური გავლილი")]
        PassedProblematic,
        [Description("პრობლემური გამოუსწორებელი")]
        [Display(Name = "პრობლემური გამოუსწორებელი")]
        NotFixableProblem,
        [Description("გაუქმებული")]
        [Display(Name = "გაუქმებული")]
        Stopped,
        [Description("აბონენტის მონაცემები არასწორია/არასრულია")]
        [Display(Name = "აბონენტის მონაცემები არასწორია/არასრულია")]
        InvalidIncomplete,
        [Description("ხელმოწერის გარეშე")]
        [Display(Name = "ხელმოწერის გარეშე")]
        WithoutSigning,
        [Description("პაკეტის არევა")]
        [Display(Name = "პაკეტის არევა")]
        PackageMessedUp,
        [Description("დანართის გარეშე")]
        [Display(Name = "დანართის გარეშე")]
        WithoutAnAttachment,
        [Description("ატვირთული")]
        [Display(Name = "ატვირთული")]
        Uploaded
    }

}