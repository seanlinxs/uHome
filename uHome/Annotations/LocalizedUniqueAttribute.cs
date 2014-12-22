using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using uHome.Models;

namespace uHome.Annotations
{
    public class LocalizedUniqueAttribute : ValidationAttribute
    {
        public LocalizedUniqueAttribute()
        {
            ErrorMessageResourceType = typeof(Resources.Resources);
            ErrorMessageResourceName = "MustBeUnique";
        }

        public override bool IsValid(object value)
        {
            var Database = new ApplicationDbContext();
            if (Database.DownloadItems.Where(di => di.Name == (string)value).Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}