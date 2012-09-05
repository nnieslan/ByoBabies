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
    public class Group
    {
        [DataMember]
        [Key]
        public long Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public ICollection<Person> Members { get; set; }

    }
}
