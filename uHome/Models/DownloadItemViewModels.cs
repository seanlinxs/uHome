using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using uHome.Annotations;
using uHome.Extensions;

namespace uHome.Models
{
    public class CreateDownloadItemViewModel
    {
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [LocalizedRequired]
        public HttpPostedFileBase FileData { get; set; }
    }

    public class ListDownloadItemViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Size { get; set; }

        public ListDownloadItemViewModel(DownloadItem downloadItem)
        {
            ID = downloadItem.ID;
            Name = downloadItem.Name;
            Description = downloadItem.Description;
            FileName = downloadItem.FileName;
            Size = downloadItem.Size.ToFileSize();
        }
    }
}