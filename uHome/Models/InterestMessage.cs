using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class InterestMessage
    {
        public int ID { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }

        public int InterestID { get; set; }

        public virtual Interest Interest { get; set; }
    }
}