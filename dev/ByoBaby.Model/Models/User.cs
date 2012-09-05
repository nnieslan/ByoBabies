using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ByoBaby.Model
{
    [DataContract]
    public class User
    {
        [DataMember]
        [Key]
        public virtual Guid UserId { get; set; }

        [DataMember]
        [Required]
        public virtual String Username { get; set; }

        [DataMember]
        [Required]
        public virtual String Email { get; set; }

        [DataMember]
        [Required, DataType(DataType.Password)]
        public virtual String Password { get; set; }

        [DataMember]
        public virtual String FirstName { get; set; }
        [DataMember]
        public virtual String LastName { get; set; }

        [DataMember]
        [DataType(DataType.MultilineText)]
        public virtual String Comment { get; set; }

        [DataMember]
        public virtual Boolean IsApproved { get; set; }
        [DataMember]
        public virtual int PasswordFailuresSinceLastSuccess { get; set; }
        [DataMember]
        public virtual DateTime? LastPasswordFailureDate { get; set; }
        [DataMember]
        public virtual DateTime? LastActivityDate { get; set; }
        [DataMember]
        public virtual DateTime? LastLockoutDate { get; set; }
        [DataMember]
        public virtual DateTime? LastLoginDate { get; set; }
        [DataMember]
        public virtual String ConfirmationToken { get; set; }
        [DataMember]
        public virtual DateTime? CreateDate { get; set; }
        [DataMember]
        public virtual Boolean IsLockedOut { get; set; }
        [DataMember]
        public virtual DateTime? LastPasswordChangedDate { get; set; }
        [DataMember]
        public virtual String PasswordVerificationToken { get; set; }
        [DataMember]
        public virtual DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        [DataMember]
        public virtual ICollection<Role> Roles { get; set; }
    }
}