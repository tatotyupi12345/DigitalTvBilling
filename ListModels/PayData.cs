using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class PayData
    {
        [JsonProperty("errorcode")]
        public int errorcode { get; set; }

        //[JsonProperty("sumbalance")]
        //public int sumbalance { get; set; }

        //[JsonProperty("summinprice")]
        //public int summinprice { get; set; }

        [JsonProperty("info")]
        public string info { get; set; }

        [JsonProperty("abonent")]
        public PayDataAbonentInfo PayDataAbonentInfo { get; set; }

        [JsonProperty("groups")]
        public List<PayDataGroup> Groups { get; set; }
    }

    public class PayDataAbonentInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class PayDataGroup
    {
        [JsonProperty("cards")]
        public List<PayDataCard> PayDataCards { get; set; }
    }

    public class PayDataCard
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("abonent_num")]
        public string AbonentNum { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_id")]
        public int StatusId { get; set; }

        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("rent_balance")]
        public decimal RentBalance { get; set; }

        [JsonProperty("rent_min_price")]
        public decimal RentMinPrice { get; set; }

        [JsonProperty("min_price")]
        public double MinPrice { get; set; }

        [JsonProperty("recomended_price")]
        public double RecomendedPrice { get; set; }
    }

    public enum CardFilterType
    {
        DocNum,
        AbonentCode,
        AbonentNum
    }

    public class PayCardFind
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("pass")]
        public string pass { get; set; }

        //[JsonProperty("secure")]
        public SecureRequest Secure { get; set; }
        //[JsonProperty("type")]
        public CardFilterType Type { get; set; }
        //[JsonProperty("value")]
        public string Value { get; set; }
    }

}