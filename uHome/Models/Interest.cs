using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace uHome.Models
{
    public class Interest
    {
        public int ID { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public virtual ICollection<InterestMessage> InterestMessages { get; set; }
    }
}