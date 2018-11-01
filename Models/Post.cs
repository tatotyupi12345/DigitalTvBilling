using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalTVBilling.Models
{
    public class Post
    {
        //public string Title { get; set; }

        [AllowHtml]
        [UIHint("tinymce_jquery_full")]
        public string Description { get; set; }
    }
}