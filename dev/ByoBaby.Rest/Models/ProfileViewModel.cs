using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using ByoBaby.Model;

namespace ByoBaby.Rest.Models
{
    [DataContract]
    public class ProfileViewModel
    {
        [Required]
        [DataMember(IsRequired=true)]
        public long Id { get; set; }

        //public ICollection<Group> MemberOf { get; set; }

        public DateTime MemberSince { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string Email { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string FirstName { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string LastName { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string MobilePhone { get; set; }

        public string HomePhone { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string City { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string State { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string Neighborhood { get; set; }

        //public ICollection<string> Interests { get; set; }

        //public ICollection<Child> Children { get; set; }

        public DateTime LastUpdated { get; set; }

        public static ProfileViewModel FromPerson(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException("person");
            }

            return new ProfileViewModel()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                City = person.City,
                State = person.State,
                MobilePhone = person.MobilePhone,
                HomePhone = person.HomePhone,
                Neighborhood = person.Neighborhood,
                Email = person.Email,
                MemberSince = person.MemberSince,
                LastUpdated = person.LastUpdated
            };
        }

    }
}
