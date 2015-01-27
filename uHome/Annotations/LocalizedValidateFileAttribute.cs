using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using uHome.Models;

namespace uHome.Annotations
{
    public class LocalizedValidateFileAttribute : ValidationAttribute
    {
        public LocalizedValidateFileAttribute()
        {
            ErrorMessageResourceType = typeof(Resources.Resources);
        }

        public override bool IsValid(object value)
        {
            long MaxFileSize = 10 * 1024 * 1024; // 10 MB
            string[] AllowedFileExtensions = { ".doc", ".docx", ".pdf", ".jpg", ".gif", ".png" };

            var file = value as HttpPostedFileBase;
            var fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

            if (!AllowedFileExtensions.Contains(fileExtension))
            {
                ErrorMessageResourceName = "ExtensionNotAllowed";
                
                return false;
            }

            if (file.InputStream.Length >= MaxFileSize)
            {
                ErrorMessageResourceName = "FileTooBig";

                return false;
            }

            return true;
        }
    }


}