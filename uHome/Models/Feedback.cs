using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class Feedback
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }
    }
}