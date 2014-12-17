using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public enum CaseState
    {
        NEW, ACTIVE, CLOSED
    }

    public class Case
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public CaseState State { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public int CaseAssignmentID { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual CaseAssignment CaseAssignment { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public Attachment AddFile(HttpPostedFileBase file)
        {
            this.Attachments = this.Attachments ?? new List<Attachment>();
            Attachment attachment = new Attachment();
            attachment.Case = this;
            attachment.Name = file.FileName;
            attachment.UploadAt = System.DateTime.Now;
            attachment.FileStream = new byte[file.InputStream.Length];
            file.InputStream.Read(attachment.FileStream, 0, attachment.FileStream.Length);
            this.Attachments.Add(attachment);

            return attachment;
        }
    }
}