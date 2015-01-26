using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;

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
            var path = string.Format("{0}/{1}.png",
                HostingEnvironment.MapPath("~/Content/Images/Events"), Guid.NewGuid().ToString());
            model.Poster.SaveAs(path);
            Poster = path;
        }

        public void AddEnrollment(Enrollment enrollment)
        {
            if (Enrollments == null)
            {
                Enrollments = new List<Enrollment>();
            }

            enrollment.Event = this;
            Enrollments.Add(enrollment);
        }
    }
}