using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Authorization
{
    public class UhomeResources
    {
        public const string VideoClip = "VideoClip";
        public const string Case = "Case";
        public const string User = "User";
        public const string DownloadItem = "DownloadItem";

        public class Actions
        {
            public const string View = "View";
            public const string Edit = "Edit";
            public const string List = "List";
            public const string AdminEdit = "AdminEdit";
        }

        public class VideoClipActions : Actions
        {
        }
        public class CaseActions : Actions
        {
        }

        public class UserActions : Actions
        {
        }

        public class DownloadItemActions : Actions
        {
        }
    }
}