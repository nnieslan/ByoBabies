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
    public class Role
    {
        [DataMember]
        [Key]
        public virtual Guid RoleId { get; set; }

        [DataMember]
        [Required]
        public virtual string RoleName { get; set; }

        [DataMember]
        public virtual string Description { get; set; }

        [DataMember]
        public virtual ICollection<User> Users { get; set; }
    }
}