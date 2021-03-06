﻿using System;
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
        public static long MAX_FILE_SIZE = 10 * 1024 * 1024;
        public static long MAX_STORAGE_SIZE = 10 * 1024 * 1024;

        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public CaseState State { get; set; }

        public CaseState OldState { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public int CaseAssignmentID { get; set; }

        public long StorageSize { get; private set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual CaseAssignment CaseAssignment { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public Attachment AddFile(HttpPostedFileBase file)
        {
            this.Attachments = this.Attachments ?? new List<Attachment>();
            var size = file.InputStream.Length;

            if (size > 10 * 1024 * 1024)
            {
                throw new Exception(string.Format(
                    Resources.Resources.FileTooBig,
                    Path.GetFileName(file.FileName),
                    MAX_FILE_SIZE / 1024 / 1024));
            }

            if (this.StorageSize + size > MAX_STORAGE_SIZE)
            {
                throw new Exception(string.Format(
                    Resources.Resources.ExceedStorageSize,
                    Path.GetFileName(file.FileName),
                    MAX_STORAGE_SIZE / 1024 / 1024));
            }
            
            var path = string.Format("{0}Uploads/{1}", AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            file.SaveAs(path);
            Attachment attachment = new Attachment();
            attachment.Case = this;
            attachment.Name = Path.GetFileName(file.FileName);
            attachment.UploadAt = System.DateTime.Now;
            attachment.Path = path;
            attachment.Size = size;
            this.Attachments.Add(attachment);
            this.StorageSize += size;

            return attachment;
        }

        public void DelAttachment(Attachment attachment)
        {
            File.Delete(attachment.Path);
            this.StorageSize -= attachment.Size;
        }

        public CommentViewModel AddComment(string value, ApplicationUser createdBy)
        {
            this.Comments = this.Comments ?? new List<Comment>();
            var comment = new Comment();
            comment.Content = value;
            comment.CreatedAt = System.DateTime.Now;
            comment.CreatedBy = createdBy;
            this.Comments.Add(comment);

            return new CommentViewModel(comment);
        }
    }
}