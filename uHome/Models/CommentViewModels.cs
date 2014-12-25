using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class CommentViewModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public CommentViewModel(Comment comment)
        {
            ID = comment.ID;
            Content = comment.Content;
            CreatedBy = comment.CreatedBy.UserName;
            CreatedAt = comment.CreatedAt;
        }
    }
}