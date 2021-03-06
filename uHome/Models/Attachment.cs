﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class Attachment
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public DateTime UploadAt { get; set; }

        public int CaseID { get; set; }

        public virtual Case Case { get; set; }
    }
}