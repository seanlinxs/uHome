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
        public string Assignee { get; set; }
        public DateTime CreatedAt { get; set; }

        public CaseListViewModel(Case c)
        {
            ID = c.ID;
            Title = c.Title;
            CreatedBy = c.CreatedBy.UserName;
            Description = c.Description;
            Assignee = c.CaseAssignment == null ? "Unassigned" : c.CaseAssignment.Assignee.UserName;
            CreatedAt = c.CreatedAt;
        }
    }

    public class CaseGroupViewModel
    {
        public CaseState State { get; private set; }
        public int Count { get; set; }
        public IEnumerable<CaseListViewModel> Cases { get; private set; }

        public CaseGroupViewModel(CaseState s, IEnumerable<CaseListViewModel> cases)
        {
            State = s;
            Cases = cases;
            Count = cases.Count();
        }
    }

    public class BaseEditCaseViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CaseState State { get; set; }
        public string StateAction { get; set; }
        [Display(Name = "Assignee", ResourceType = typeof(Resources.Resources))]
        public string Assignee { get; set; }
        public ICollection<AttachmentViewModel> Attachments { get; set; }

        public BaseEditCaseViewModel(Case @case)
        {
            ID = @case.ID;
            Title = @case.Title;
            Description = @case.Description;
            CreatedBy = @case.CreatedBy.UserName;
            CreatedAt = @case.CreatedAt;
            UpdatedAt = @case.UpdatedAt;
            State = @case.State;
            Assignee = @case.CaseAssignment == null ? "Unassigned" : @case.CaseAssignment.Assignee.UserName;
            Attachments = new List<AttachmentViewModel>();

            foreach (var attachment in @case.Attachments)
            {
                Attachments.Add(new AttachmentViewModel(attachment));
            }
        }
    }
    public class EditCaseViewModel : BaseEditCaseViewModel
    {
        public EditCaseViewModel(Case @case) : base(@case)
        {
            StateAction = State == CaseState.CLOSED ? "Reopen" : "Close";
        }
    }

    public class StaffEditCaseViewModel : BaseEditCaseViewModel
    {
        public StaffEditCaseViewModel(Case @case) : base(@case)
        {
            StateAction = State == CaseState.ASSIGNED ? "Start" : "Stop";
        }
    }
}