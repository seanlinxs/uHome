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
    }

    public class ListCaseViewModel
    {
        public int ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public CaseState State { get; private set; }

        public ListCaseViewModel(Case @case)
        {
            ID = @case.ID;
            Title = @case.Title;
            var desc = @case.Description;
            var len = int.Parse(ConfigurationManager.AppSettings["MaxDisplayedChars"]);
            Description = desc.Length > len ? desc.Substring(0, len) : desc;
            CreatedAt = @case.CreatedAt;
            State = @case.State;
        }
    }
}