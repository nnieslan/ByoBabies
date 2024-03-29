﻿using System;
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
    [Serializable()]
    public class Blurb
    {
        [DataMember]
        public long ConversationId { get; set; }
        
        [DataMember]
        [Key]
        public long Id { get; set; }
        
        [DataMember]
        public string Content { get; set; }
        
        [DataMember]
        public Person WrittenBy { get; set; }
        
        [DataMember]
        public DateTime WrittenOn { get; set; }
    }
}
