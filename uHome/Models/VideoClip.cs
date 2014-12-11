using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class VideoClip
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public DateTime UploadedAt { get; set; }

        public string ApplicationUserId { get; set; }

        [Required]
        public virtual ApplicationUser UploadedBy { get; set; }

        public VideoClip()
        {
        }

        public VideoClip(VideoClipViewModel videoClipViewModel)
        {
            this.Name = videoClipViewModel.Name;
            this.Description = videoClipViewModel.Description;
            this.Path = videoClipViewModel.Path;
            this.UploadedAt = DateTime.Now;
        }
    }
}