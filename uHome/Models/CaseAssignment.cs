using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uHome.Models
{
    public class CaseAssignment
    {
        [Key]
        [ForeignKey("Case")]
        public int CaseID { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public DateTime AssignmentDate { get; set; }

        public virtual Case Case { get; set; }

        public virtual ApplicationUser Assignee { get; set; }
    }
}