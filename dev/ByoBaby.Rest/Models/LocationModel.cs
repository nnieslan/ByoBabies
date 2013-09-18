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
    public class LocationModel
    {
        [DataMember(IsRequired = true)]
        public string YelpId { get; set; }

        [DataMember(IsRequired = true)]
        public double? Latitude { get; set; }

        [DataMember(IsRequired = true)]
        public double? Longitude { get; set; }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string PhoneNumber { get; set; }

        [DataMember(IsRequired = true)]
        public string Address { get; set; }
    }
}