using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class PaymentData
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("pass")]
        public string pass { get; set; }

        [JsonIgnore]
        public long Id { get; set; }

        [JsonProperty("secure")]
        public SecureRequest Secure { get; set; }

        [JsonIgnore]
        [DisplayName("გადახდის სახეობა:")]
        [Required(ErrorMessage = "აირჩიეთ გადახდის სახეობა")]
        public int PayType { get; set; }

        [JsonProperty("rent")]
        [DisplayName("იაჯარა:")]
        public int PayRent { get; set; }

        [JsonProperty("cards")]
        public List<int> Cards { get; set; }

        [JsonProperty("amount")]
        [DisplayName("თანხა:")]
        [Required(ErrorMessage = "შეიყვანეთ თანხა")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეიყვანეთ თანხა")]
        public decimal Amount { get; set; }

        [JsonProperty("rent_amount")]
        [DisplayName("თანხა:")]
        [Required(ErrorMessage = "შეიყვანეთ თანხა")]
        [Range(0, Int32.MaxValue, ErrorMessage = "შეიყვანეთ თანხა")]
        public decimal RentAmount { get; set; }
        //[JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("client_id")]
        public string ClientID { get; set; }

        [JsonIgnore]
        public string Logging { get; set; }

        [JsonIgnore]
        public decimal MinRentAmount { get; set; }
        [JsonIgnore]
        public decimal MinAmount { get; set; }
    }

    public class PaymentCheck
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("pass")]
        public string pass { get; set; }

        [JsonProperty("secure")]
        public SecureRequest Secure { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }
    }
}