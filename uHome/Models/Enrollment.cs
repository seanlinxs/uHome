using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace uHome.Models
{
    public class Enrollment
    {
        public Enrollment() { }

        public Enrollment(CreateEnrollmentViewModel model)
        {
            Email = model.Email;
            Number = model.Number;
            FullName = model.FullName;
            Country = model.Country;
            State = model.State;
            City = model.City;
            Address = model.Address;
            EnrollAt = System.DateTime.Now;
        }

        public int ID { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime EnrollAt { get; set; }
        public int EventID { get; set; }

        public virtual Event Event { get; set; }
    }
}