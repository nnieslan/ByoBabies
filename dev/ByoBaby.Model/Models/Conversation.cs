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
    public class Conversation
    {
        [DataMember]
        [Key]
        public long Id { get; set; }
        
        [DataMember]
        public string Topic { get; set; }
        
        [DataMember]
        public ICollection<Blurb> Graph { get; set; }
        
        [DataMember]
        public DateTime LastUpdated { get; set; }
        
        [DataMember]
        public Person StartedBy { get; set; }
            
    }
}
