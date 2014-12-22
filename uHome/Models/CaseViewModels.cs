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
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
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
        public int ID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }
        public CaseState State { get; set; }
        public string StateAction { get; set; }
        public string Assignee { get; set; }
        public ICollection<AttachmentViewModel> Attachments { get; set; }

        public EditCaseViewModel(Case @case)
        {
            ID = @case.ID;
            Title = @case.Title;
            Description = @case.Description;
            CreatedBy = @case.CreatedBy.UserName;
            CreatedAt = @case.CreatedAt;
            UpdatedAt = @case.UpdatedAt;
            State = @case.State;
            StateAction = State == CaseState.CLOSED ? "Reopen" : "Close";
            Assignee = @case.CaseAssignment == null ? "Unassigned" : @case.CaseAssignment.Assignee.UserName;
            Attachments = new List<AttachmentViewModel>();
            
            foreach (var attachment in @case.Attachments)
            {
                Attachments.Add(new AttachmentViewModel(attachment));
            }
        }
    }
}