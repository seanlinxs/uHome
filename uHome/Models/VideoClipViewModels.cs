using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uHome.Annotations;
using System.ComponentModel.DataAnnotations;

namespace uHome.Models
{
    public class VideoClipViewModel
    {
        public int Id { get; set; }

        [LocalizedRequired]
        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [LocalizedRequired]
        public string Path { get; set; }
    }
}