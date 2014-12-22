using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Extensions
{
    public static class LongExtensions
    {
        public static string ToFileSize(this long Size)
        {
            if (Size < 1024)
            {
                return Size.ToString();
            }
            else if (Size >= 1024 && Size < 1024 * 1024)
            {
                return string.Format("{0}KB", Size / 1024);
            }
            else
            {
                return string.Format("{0}MB", Size / 1024 / 1024);
            }
        }
    }
}