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
    [DataContract(IsReference = true)]
    public partial class SecurityRolePermission : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Factory Method

        /// <summary>
        /// Create a new SecurityRolePermission object.
        /// </summary>
        /// <param name="securityRolePermission1">Initial value of the SecurityRolePermission1 property.</param>
        /// <param name="securityRoleId">Initial value of the SecurityRoleId property.</param>
        /// <param name="securityPermissionId">Initial value of the SecurityPermissionId property.</param>
        public static SecurityRolePermission CreateSecurityRolePermission(long securityRolePermission1, long securityRoleId, long securityPermissionId)
        {
            SecurityRolePermission securityRolePermission = new SecurityRolePermission();
            securityRolePermission.SecurityRolePermissionId = securityRolePermission1;
            securityRolePermission.SecurityRoleId = securityRoleId;
            securityRolePermission.SecurityPermissionId = securityPermissionId;
            return securityRolePermission;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        [Key]
        public long SecurityRolePermissionId
        {
            get
            {
                return _SecurityRolePermissionId;
            }
            set
            {
                if (_SecurityRolePermissionId != value)
                {
                    ReportPropertyChanging("SecurityRolePermissionId");
                    _SecurityRolePermissionId = value;
                    ReportPropertyChanged("SecurityRolePermissionId");
                }
            }
        }
        private long _SecurityRolePermissionId;
        
        [DataMemberAttribute()]
        public long SecurityRoleId
        {
            get
            {
                return _SecurityRoleId;
            }
            set
            {
                ReportPropertyChanging("SecurityRoleId");
                _SecurityRoleId = value;
                ReportPropertyChanged("SecurityRoleId");
            }
        }
        private long _SecurityRoleId;
        
        [DataMemberAttribute()]
        public long SecurityPermissionId
        {
            get
            {
                return _SecurityPermissionId;
            }
            set
            {
                ReportPropertyChanging("SecurityPermissionId");
                _SecurityPermissionId = value;
                ReportPropertyChanged("SecurityPermissionId");
            }
        }
        private long _SecurityPermissionId;
        
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public SecurityPermission SecurityPermission
        {
            get; set; 
        }
        
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
