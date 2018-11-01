using DigitalTVBilling.CallCenter.InterfaceUser;
using DigitalTVBilling.ListModels;
using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.CallCenter
{
    public class CallModel
    {
        public List<CallUser> users { get; set; }
        public IEnumerable<Order> order { get; set; }
        public IEnumerable<Damage> damage { get; set; }
        public IEnumerable<Cancellation> cancellation { get; set; }
    }
    public class CallUsersModel
    {
        public List<CallUser> users { get; set; }
    }
    public class FilterUser :IFilterUser
    {
        public int user_id { get; set; }
        public bool check_user { get; set; }
    }
    public class CallUser
    {

        public int Id { get; set; }

        public string Login { get; set; }


        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }

        public int Type { get; set; }

        public bool HardAutorize { get; set; }

        public UserType UserType { get; set; }

        public int start_end { get; set; }

        public string Logging { get; set; }


        public string TypeName { get; set; }

        public int? @object { get; set; }

        public SellerObject SellerObj { get; set; }

        public string CodeWord { get; set; }

        public string image { get; set; }

        public StaticCount StaticCounts { get; set; }

        public ToGoId OrderToGo { get; set; }
        
        public ToGoId DamageGoTo { get; set; }

        public ToGoId CancellationToGo { get; set; }

        public List<RegionView> CityRegion { get; set; }
    }
    public class ToGoId
    {
        public int to_go { get; set; } = 0;
        public int id { get; set; } = 0;
    }
    public class CallDamage
    {

        public int Id { get; set; }


        public DateTime Tdate { get; set; }


        public int Num { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string Address { get; set; }


        public string CardAddress { get; set; }


        public string ApproveUser { get; set; }


        public bool IsApproved { get; set; }

        public string Data { get; set; }


        public int ReceiversCount { get; set; }

        public DateTime GetDate { get; set; }

        public DateTime ChangeDate { get; set; }


        public DamageStatus Status { get; set; }


        public bool MontageStatus { get; set; }


        public int montage_user_id { get; set; }


        public string ChangerUser { get; set; }


        public int UserId { get; set; }


        public User UserUser { get; set; }

        public int UserGroupId { get; set; }


        public User UserGroup { get; set; }

        public Customer Customer { get; set; }


        public int ExecutorID { get; set; }

        public string comment { get; set; }
    }
    public class CallCancellation
    {

       
        public int Id { get; set; }

        
        public DateTime Tdate { get; set; }

        public int Num { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string CardAddress { get; set; }

        public string ApproveUser { get; set; }

        public bool IsApproved { get; set; }

        public string Data { get; set; }

        public int ReceiversCount { get; set; }

        public DateTime GetDate { get; set; }

        public DateTime ChangeDate { get; set; }


        public CancleStatus Status { get; set; }


        public int cancle_status { get; set; }

        public string ChangerUser { get; set; }


        public int UserId { get; set; }


        public User UserUser { get; set; }


        public int UserGroupId { get; set; }

        public int card_num { get; set; }


        public int ExecutorID { get; set; }


        public User UserGroup { get; set; }


        public Customer Customer { get; set; }



    }
    public class StaticCount
    {
        public int order_count { get; set; }
        public int order_yielding { get; set; }
        public int order_remainder { get; set; }
        public int order_cancled { get; set; }

        public int damage_count { get; set; }
        public int damage_yielding { get; set; }
        public int damage_remainder { get; set; }

        public int cancel_count { get; set; }
        public int cancel_yielding { get; set; }
        public int cancel_remainder { get; set; }
        public int cancel_cancled { get; set; }
        
        public int OrderCount { get; set; }
        public int DamageCount { get; set; }
        public int CancellationCount { get; set; }

        public string month { get; set; }
    }
    public class RegionNameID
    {
       public string data { get; set; }
       public int id { get; set; }
    }
    public class RegionView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }


    public class _Card
    {
        public _Card()
        {
            //this.subscribtions=
            //this.tdate = DateTime.Now;
            //this.CallDate = DateTime.Now;
            //this.CasDate = DateTime.Now;
            //this.Discount = 0;
            //this.HasFreeDays = true;
            //this.JuridVerifyStatus = CardJuridicalVerifyStatus.None;
        }


        public int id { get; set; }


        public DateTime tdate { get; set; }


        public string abonent_num { get; set; }

        public string card_num { get; set; }

        public string address { get; set; }


        public string doc_num { get; set; }

        public double discount { get; set; }

        public int group { get; set; }

        public int customer_id { get; set; }


        public CardStatus status { get; set; }


        public string city { get; set; }


        public string village { get; set; }

        public string region { get; set; }

        public DateTime close_date { get; set; }


        public DateTime finish_date { get; set; }

      public DateTime rent_finish_date { get; set; }


        public DateTime pause_date { get; set; }


        public int pause_days { get; set; }


        public bool closed_is_pen { get; set; }

        public int user_id { get; set; }



        public int tower_id { get; set; }

        public int receiver_id { get; set; }


        public double? latitude { get; set; }

        public double? longitude { get; set; }


        public string mux1_level { get; set; }

        public string mux2_level { get; set; }

        public string mux3_level { get; set; }


        public string mux1_quality { get; set; }

        public string mux2_quality { get; set; }

        public string mux3_quality { get; set; }

        public string juridical_verify_status { get; set; }


        public virtual User User { get; set; }


        public Customer Customer { get; set; }

        public Tower Tower { get; set; }


        public Receiver Receiver { get; set; }


        public string LogReceiver { get; set; }


        public string LogTower { get; set; }

        public DateTime cas_date { get; set; }


        public int mode { get; set; }


        public int approve_status { get; set; }

        public bool auto_invoice { get; set; }

        public bool HasFreeDays { get; set; }


        public CardVerifyStatus verify_status { get; set; }

        public CardJuridicalVerifyStatus juridical_verification { get; set; }

        public CardBlockedCardsStatus blocked_cards_verifiction { get; set; }

        public string info { get; set; }

        public string desc { get; set; }


        public StoppedCardStatus stopped_status { get; set; }

        public PauseType last_pause_type { get; set; }


        public bool has_used_free_pause_days { get; set; }


        public DateTime call_date { get; set; }

        public short none_free_pause_count_per_month { get; set; }

        public List<_Subscribtion> subscribtions { get; set; }

        public List<_Customer> _Customers { get; set; }
    }
    public class _Subscribtion
    {

        public int id { get; set; }

        public DateTime tdate { get; set; }

        public int card_id { get; set; }

        public double amount { get; set; }

        public bool status { get; set; }

        public int user_id { get; set; }
    }

    public class _Customer
    {


        public int id { get; set; }

        public DateTime tdate { get; set; }


        public string name { get; set; }

        public string lastname { get; set; }


        public string code { get; set; }


        public string address { get; set; }


        public CustomerType type { get; set; }


        public int juridical_type { get; set; }


        public bool is_budget { get; set; }


        public DateTime? jurid_finish_date { get; set; }


        public string city { get; set; }


        public string village { get; set; }

        public string district { get; set; }

        public string email { get; set; }

        public bool is_facktura { get; set; }


        public string region { get; set; }

        public string phone1 { get; set; }


        public string phone2 { get; set; }

        public string desc { get; set; }


        public string security_code { get; set; }


        public int user_id { get; set; }


        public AbonentVerifyStatus verify_status { get; set; }


        public string info { get; set; }


        public short attachment_approve_status { get; set; }

        public AbonentBuyReason buy_reason { get; set; }


        public AbonentSatisfiedStatus is_satisfied { get; set; }

        public string signature { get; set; }

        public int temporary_use { get; set; }

    }
}