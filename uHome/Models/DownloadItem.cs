using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class DownloadItem
    {
        public int ID { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
    }
}