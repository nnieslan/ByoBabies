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

    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public partial class SecurityRoleUser : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Factory Method

        /// <summary>
        /// Create a new SecurityRoleUser object.
        /// </summary>
        /// <param name="securityRoleUserId">Initial value of the SecurityRoleUserId property.</param>
        /// <param name="securityRoleId">Initial value of the SecurityRoleId property.</param>
        /// <param name="userId">Initial value of the UserId property.</param>
        public static SecurityRoleUser CreateSecurityRoleUser(long securityRoleUserId, long securityRoleId, Guid userId)
        {
            SecurityRoleUser securityRoleUser = new SecurityRoleUser();
            securityRoleUser.SecurityRoleUserId = securityRoleUserId;
            securityRoleUser.SecurityRoleId = securityRoleId;
            securityRoleUser.UserId = userId;
            return securityRoleUser;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        [Key]
        public long SecurityRoleUserId
        {
            get
            {
                return this.securityRoleUserId;
            }
            set
            {
                if (this.securityRoleUserId != value)
                {
                    ReportPropertyChanging("SecurityRoleUserId");
                    this.securityRoleUserId = value;
                    ReportPropertyChanged("SecurityRoleUserId");
                }
            }
        }
        private long securityRoleUserId;
        
        [DataMemberAttribute()]
        public long SecurityRoleId
        {
            get
            {
                return this.securityRoleId;
            }
            set
            {
                ReportPropertyChanging("SecurityRoleId");
                this.securityRoleId = value;
                ReportPropertyChanged("SecurityRoleId");
            }
        }
        private long securityRoleId;

        [DataMemberAttribute()]
        public long GroupId
        {
            get
            {
                return this.groupId;
            }
            set
            {
                ReportPropertyChanging("GroupId");
                this.groupId = value;
                ReportPropertyChanged("GroupId");
            }
        }
        private long groupId;
        

        [DataMemberAttribute()]
        public Guid UserId
        {
            get
            {
                return this.userId;
            }
            set
            {
                ReportPropertyChanging("UserId");
                this.userId = value;
                ReportPropertyChanged("UserId");
            }
        }
        private Guid userId;
        
        #endregion

        #region Navigation Properties

        //[DataMemberAttribute()]
        //public aspnet_Users aspnet_Users
        //{
        //    get; set;
        //}

        
        [DataMemberAttribute()]
        public SecurityRole SecurityRole
        {
            get; set; 
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void ReportPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging
        public event PropertyChangingEventHandler PropertyChanging;

        protected void ReportPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = this.PropertyChanging;
            if (null != handler)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        #endregion
    }
    
}
