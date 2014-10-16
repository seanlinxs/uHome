using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class VideoClip
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime UploadedAt { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser UploadedBy { get; set; }
    }
}