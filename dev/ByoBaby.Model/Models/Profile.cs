using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    [DataContract]
    public class Profile
    {
        [DataMember]
        [Key]
        public long Id { get; set; }

        [DataMember]
        public long PersonId { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string MobilePhone { get; set; }

        [DataMember]
        public string HomePhone { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Neighborhood { get; set; }
        
        [DataMember]
        public ICollection<string> Interests { get; set; }

        [DataMember]
        public ICollection<Child> Children { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }
    }
}
