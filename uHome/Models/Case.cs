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
        public const long MAX_STORAGE_SIZE = 100 * 1024 * 1024;

        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public CaseState State { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public int CaseAssignmentID { get; set; }

        public long StorageSize { get; private set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual CaseAssignment CaseAssignment { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public Attachment AddFile(HttpPostedFileBase file)
        {
            if (this.StorageSize + file.InputStream.Length > MAX_STORAGE_SIZE)
            {
                return null;
            }

            this.Attachments = this.Attachments ?? new List<Attachment>();
            Attachment attachment = new Attachment();
            attachment.Case = this;
            attachment.Name = file.FileName;
            attachment.UploadAt = System.DateTime.Now;
            attachment.FileStream = new byte[file.InputStream.Length];
            file.InputStream.Read(attachment.FileStream, 0, attachment.FileStream.Length);
            this.Attachments.Add(attachment);
            this.StorageSize += attachment.FileStream.LongLength;

            return attachment;
        }
    }
}