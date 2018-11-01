using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Models
{
    public class JuridicalList
    {
            public int Id { get; set; }
            public DateTime Tdate { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Abonent_Num { get; set; }
            public string UsName { get; set; }
            public string UsType { get; set; }
            public int JuridicalVerifical { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Num { get; set; }
            public string CardNum { get; set; }
            public string DocNum { get; set; }
            public string ActivePacket { get; set; }
            public CardStatus Status { get; set; }
            public int user_id { get; set; }
    }
}