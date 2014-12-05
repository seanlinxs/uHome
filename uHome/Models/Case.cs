using System;
using System.Collections.Generic;
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

        public string ApplicationUserId { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual CaseAssignment CaseAssignment { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public void AddFiles(IEnumerable<HttpPostedFileBase> Files)
        {
            var now = System.DateTime.Now;

            if (Files.Count() > 0 && Files.First() != null)
            {
                this.Attachments = this.Attachments ?? new List<Attachment>();

                foreach (var file in Files)
                {
                    var attachment = new Attachment();
                    attachment.Case = this;
                    attachment.Name = file.FileName;
                    attachment.UploadAt = now;
                    attachment.FileStream = new byte[file.InputStream.Length];
                    file.InputStream.Read(attachment.FileStream, 0, attachment.FileStream.Length);
                    this.Attachments.Add(attachment);
                }
            }
        }
    }
}