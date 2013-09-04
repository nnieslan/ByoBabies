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
    public class ProfileViewModel : RequestorViewModel
    {
        #region ctor
        
        public ProfileViewModel()
        {
            this.MemberOf = new List<Group>();
            this.Children = new List<ChildViewModel>();
        }

        #endregion

        [DataMember()]
        public List<Group> MemberOf { get; set; }

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

        [DataMember()]
        public string ProfilePictureUrl { get; set; }

        [DataMember()]
        public List<string> Interests { get; set; }

        [DataMember()]
        public List<ChildViewModel> Children { get; set; }

        public DateTime LastUpdated { get; set; }

        public static ProfileViewModel FromPerson(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException("person");
            }

            var profile = new ProfileViewModel()
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
                ProfilePictureUrl = person.ProfilePictureUrl,
                MemberSince = person.MemberSince,
                LastUpdated = person.LastUpdated
            };
            if (profile.ProfilePictureUrl == null)
            {
                profile.ProfilePictureUrl = "http://maplaze.com/jpg/empty-profile.jpg";
            }
            if (person.Children != null)
            {
                profile.Children = new List<ChildViewModel>(
                    person.Children.Select(c => new ChildViewModel() { Name = c.Name, Age = c.Age, Gender = c.Gender, Id = c.Id }));
            }
            
            return profile;
        }

    }
}
