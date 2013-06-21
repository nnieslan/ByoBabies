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
    public class ChildViewModel
    {
        [DataMember]
        public int? Id { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public string Name { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public int Age { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public string Gender { get; set; }

    }
}