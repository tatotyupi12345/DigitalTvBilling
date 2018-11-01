using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class SecureRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}