using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int CaseId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Proxy
        public virtual Case Case { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}