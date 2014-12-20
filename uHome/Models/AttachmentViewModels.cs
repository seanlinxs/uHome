using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class AttachmentViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime UploadAt { get; set; }
        public string Size { get; set; }

        public AttachmentViewModel(Attachment attachment)
        {
            ID = attachment.ID;
            Name = attachment.Name;
            UploadAt = attachment.UploadAt;
            var length = attachment.Size;

            if (length < 1024)
            {
                Size = length.ToString();
            }
            else if (length >= 1024 && length < 1024 * 1024)
            {
                Size = string.Format("{0}KB", length / 1024);
            }
            else
            {
                Size = string.Format("{0}MB", length / 1024 / 1024);
            }
        }
    }
}