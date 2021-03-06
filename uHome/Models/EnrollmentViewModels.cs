﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using uHome.Annotations;

namespace uHome.Models
{
    public class CreateEnrollmentViewModel
    {
        [LocalizedRequired]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "InvalidEmail",
            ErrorMessage = null)]
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        public string Email { get; set; }
        public string Number { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime EnrollAt { get; set; }
    }

    public class ListEnrollmentViewModel
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime EnrollAt { get; set; }

        public ListEnrollmentViewModel() { }

        public ListEnrollmentViewModel(Enrollment e)
        {
            ID = e.ID;
            FullName = e.FullName;
            Country = e.Country;
            City = e.City;
            EnrollAt = e.EnrollAt;
        }
    }
}