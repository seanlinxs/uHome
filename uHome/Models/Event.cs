using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class Event
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime OpenAt { get; set; }
        public string Address { get; set; }
        public string Poster { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}