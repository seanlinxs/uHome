using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using uHome.Annotations;
using System.Configuration;

namespace uHome.Models
{
    public class CreateCaseViewModel
    {
        [LocalizedRequired]
        [LocalizedStringLength(50)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "UploadAttachments", ResourceType = typeof(Resources.Resources))]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
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
        public IEnumerable<CaseListViewModel> Cases { get; private set; }

        public CaseGroupViewModel(CaseState s, IQueryable<CaseListViewModel> cases)
        {
            State = s;
            Cases = cases;
        }
    }
}