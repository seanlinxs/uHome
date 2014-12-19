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
        public static long MAX_STORAGE_SIZE = 10 * 1024 * 1024; // Allow maximum 50MB attachments

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

        public string AddFiles(HttpFileCollectionBase files)
        {
            this.Attachments = this.Attachments ?? new List<Attachment>();

            for (int i = 0; i < files.Count; i++ )
            {
                var file = files[i];

                if (this.StorageSize + file.InputStream.Length > MAX_STORAGE_SIZE)
                {
                    return file.FileName;
                }
                else
                {
                    Attachment attachment = new Attachment();
                    attachment.Case = this;
                    attachment.Name = file.FileName;
                    attachment.UploadAt = System.DateTime.Now;
                    attachment.FileStream = new byte[file.InputStream.Length];
                    file.InputStream.Read(attachment.FileStream, 0, attachment.FileStream.Length);
                    this.Attachments.Add(attachment);
                    this.StorageSize += attachment.FileStream.LongLength;
                }
            }

            return null;
        }

        public void DelAttachment(Attachment attachment)
        {
            this.StorageSize -= attachment.FileStream.LongLength;
            this.Attachments.Remove(attachment);
        }
    }
}