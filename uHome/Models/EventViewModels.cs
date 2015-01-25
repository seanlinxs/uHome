using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using uHome.Annotations;

namespace uHome.Models
{
    public class CreateEventViewModel
    {
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [LocalizedRequired]
        public string OpenAt { get; set; }
        [LocalizedRequired]
        public string Address { get; set; }
        [LocalizedRequired]
        public HttpPostedFileBase Poster { get; set; }
    }
}