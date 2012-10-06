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
    public class Event
    {

        [DataMember]
        [Key]
        public long Id { get; set; }

        [DataMember]
        public long OwnerId { get; set; }

        [DataMember]
        public string Title { get; set; }
        
        [DataMember]
        public string Description { get; set; }
        
        [DataMember]
        public DateTime Start { get; set; }
        
        [DataMember]
        public DateTime End { get; set; }
        
        [DataMember]
        public ICollection<Person> WhosIn { get; set; }
        
    }
}
