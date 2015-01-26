using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<EventViewModel> Events { get; set; }
        public IEnumerable<VideoClip> VideoClips { get; set; }

        public HomeIndexViewModel() { }

        public HomeIndexViewModel(IEnumerable<EventViewModel> events, IEnumerable<VideoClip> videoClips)
        {
            Events = events;
            VideoClips = videoClips;
        }
    }
}