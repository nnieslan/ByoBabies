using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Rest.Models
{
    public class RequestViewModel
    {
        [Required]
        [DataMember(IsRequired = true)]
        public long Id { get; set; }

        [DataMember()]
        public string Title { get; set; }

        [DataMember()]
        public string Body { get; set; }

        [DataMember]
        public string RequestType { get; set; }

        [DataMember]
        public RequestorViewModel Requestor { get; set; }

        
        public static RequestViewModel FromRequest(Request source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var vm = new RequestViewModel()
            {
                Id = source.Id,
                Title = source.Title,
                Body = source.Description,
                RequestType = source.GetType().Name
            };

            if (source.TargetIdType.Equals("person", StringComparison.OrdinalIgnoreCase))
            {
                using (var repo = new ByoBabyRepository())
                {
                    var person = repo.People.Find(source.TargetId);
                    vm.Requestor = ProfileViewModel.FromPerson(person);
                }
            }

            //TODO - implement fetching the other possible target types (group, event

            return vm;
        }
    }
}