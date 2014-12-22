using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uHome.Annotations;

namespace uHome.Models
{
    public class DownloadItem
    {
        public int ID { get; set; }
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        [LocalizedUnique]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
    }
}