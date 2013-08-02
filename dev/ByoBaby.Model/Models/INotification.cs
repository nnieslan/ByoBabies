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
    public interface INotification
    {
        [DataMember]
        [Key]
        long Id { get; set; }

        [DataMember]
        long TargetId { get; set; }

        [DataMember]
        string TargetIdType { get; set; }

        [DataMember]
        string Title { get; set; }

        [DataMember]
        string Description { get; set; }
    }
}
