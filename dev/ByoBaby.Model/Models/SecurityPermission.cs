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
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public partial class SecurityPermission : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Factory Method

        /// <summary>
        /// Create a new SecurityPermission object.
        /// </summary>
        /// <param name="securityPermissionId">Initial value of the SecurityPermissionId property.</param>
        public static SecurityPermission CreateSecurityPermission(long securityPermissionId)
        {
            SecurityPermission securityPermission = new SecurityPermission();
            securityPermission.SecurityPermissionId = securityPermissionId;
            return securityPermission;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        [Key]
        public long SecurityPermissionId
        {
            get
            {
                return this.securityPermissionId;
            }
            set
            {
                if (this.securityPermissionId != value)
                {
                    ReportPropertyChanging("SecurityPermissionId");
                    this.securityPermissionId = value;
                    ReportPropertyChanged("SecurityPermissionId");
                }
            }
        }
        private long securityPermissionId;

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ICollection<SecurityRolePermission> SecurityRolePermissions
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
