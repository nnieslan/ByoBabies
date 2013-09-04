using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{
    public class Notification
    {
        [DataMember]
        [Key]
        public long Id { get; set; }

        [DataMember]
        public NotificationOriginator Originator { get; set; }

        [DataMember]
        public Person Audience { get; set; }
    }
}
