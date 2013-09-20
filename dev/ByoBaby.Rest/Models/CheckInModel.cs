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
    public class CheckInModel
    {
        [DataMember(IsRequired = true)]
        public LocationModel Location { get; set; }

        [DataMember(IsRequired = true)]
        public int Duration { get; set; }

        [DataMember(IsRequired = true)]
        public string Note { get; set; }
    }
}