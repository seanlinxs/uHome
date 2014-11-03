using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace uHome.Annotations
{
    public class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        public LocalizedStringLengthAttribute(int maximumLength, int minimumLength = 0)
            : base(maximumLength)
        {
            ErrorMessageResourceType = typeof(Resources.Resources);

            MinimumLength = minimumLength;

            if (MinimumLength > 0)
            {
                ErrorMessageResourceName = "StringLengthMustBetween";
            }
            else
            {
                ErrorMessageResourceName = "StringLengthMustLessThan";
            }
        }
    }
}