using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using uHome.Annotations;

namespace uHome.Models
{
    public class CreateEventViewModel
    {
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [LocalizedRequired]
        public string OpenAt { get; set; }
        [LocalizedRequired]
        public string Address { get; set; }
        [LocalizedRequired]
        public HttpPostedFileBase Poster { get; set; }
    }

    public class EventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime OpenAt { get; set; }
        public string Address { get; set; }
        public string PosterUrl { get; set; }

        public EventViewModel() { }

        public EventViewModel(Event @event)
        {
            Title = @event.Title;
            Description = @event.Description;
            OpenAt = @event.OpenAt;
            Address = @event.Address;
            PosterUrl = string.Format("/Content/Images/Events/{0}", Path.GetFileName(@event.Poster));
        }
    }
}