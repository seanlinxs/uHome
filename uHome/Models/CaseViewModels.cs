using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using uHome.Annotations;
using System.Configuration;
using System.Text;

namespace uHome.Models
{
    public class CreateCaseViewModel
    {
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "UploadAttachments", ResourceType = typeof(Resources.Resources))]
        [ValidateFiles]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }

    public class ValidateFilesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            long MaxFileSize = 10 * 1024 * 1024; // 10 MB
            string[] AllowedFileExtensions = { ".doc", ".docx", ".pdf", ".jpg", ".gif", ".png" };

            var files = value as IEnumerable<HttpPostedFileBase>;

            // nothing attached, this is allowed
            if (files.Count() == 1 && files.First() == null)
            {
                return true;
            }

            foreach (var file in files)
            {
                var fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

                if (!AllowedFileExtensions.Contains(fileExtension))
                {
                    ErrorMessage = string.Format("{0} is not in allowed file types: {1}", file.FileName, string.Join(", ", AllowedFileExtensions));
                    return false;
                }

                if (file.InputStream.Length >= MaxFileSize)
                {
                    ErrorMessage = string.Format("{0} is too large, maximum allowed file size is {1}MB", file.FileName, MaxFileSize / 1024 / 1024);
                    return false;
                }
            }

            return true;
        }
    }

    public class CaseListViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string DescriptionThumb { get; set; }
        public string Assignee { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CaseGroupViewModel
    {
        public CaseState State { get; private set; }
        public int Count { get; set; }
        public IEnumerable<CaseListViewModel> Cases { get; private set; }

        public CaseGroupViewModel(CaseState s, IQueryable<CaseListViewModel> cases)
        {
            State = s;
            Cases = cases;
            Count = cases.Count();
        }
    }

    public class EditCaseViewModel
    {
        public int ID { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }
        public CaseState State { get; set; }
        public string Assignee { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        [Display(Name = "UploadAttachments", ResourceType = typeof(Resources.Resources))]
        [ValidateFiles]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}