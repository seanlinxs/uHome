using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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

        public Attachment AddFile(HttpPostedFileBase file)
        {
            this.Attachments = this.Attachments ?? new List<Attachment>();
            var size = file.InputStream.Length;

            if (this.StorageSize + size > MAX_STORAGE_SIZE)
            {
                return null; // Can not upload this file but might be able to upload smaller ones
            }
            else
            {
                var path = string.Format("{0}Uploads/{1}", AppDomain.CurrentDomain.BaseDirectory, file.FileName);
                file.SaveAs(path);
                Attachment attachment = new Attachment();
                attachment.Case = this;
                attachment.Name = file.FileName;
                attachment.UploadAt = System.DateTime.Now;
                attachment.Path = path;
                attachment.Size = size;
                this.Attachments.Add(attachment);
                this.StorageSize += size;
                
                return attachment;
            }

        }

        public void DelAttachment(Attachment attachment)
        {
            File.Delete(attachment.Path);
            this.StorageSize -= attachment.Size;
        }
    }
}