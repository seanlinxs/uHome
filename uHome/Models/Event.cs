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

        public static Event CreateEvent(CreateEventViewModel model)
        {
            Event e = new Event();
            e.Title = model.Title;
            e.Description = model.Description;
            e.OpenAt = model.OpenAt;
            e.Address = model.Address;
            e.Enrollments = new List<Enrollment>();
            
            // Poster
            var path = string.Format("{0}Uploads/{1}", AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            model.Poster.SaveAs(path);
            e.Poster = path;

            return e;
        }
    }
}