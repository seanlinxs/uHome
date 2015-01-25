using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Event() { }

        public Event(CreateEventViewModel model)
        {
            Title = model.Title;
            Description = model.Description;
            OpenAt = DateTime.Parse(model.OpenAt);
            Address = model.Address;
            Enrollments = new List<Enrollment>();
            
            // Poster
            var path = string.Format("{0}Uploads/{1}",
                AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            model.Poster.SaveAs(path);
            Poster = path;
        }
    }
}