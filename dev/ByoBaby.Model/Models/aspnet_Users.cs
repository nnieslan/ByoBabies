using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
   
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public partial class aspnet_Users : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Factory Method

        /// <summary>
        /// Create a new aspnet_Users object.
        /// </summary>
        /// <param name="applicationId">Initial value of the ApplicationId property.</param>
        /// <param name="userId">Initial value of the UserId property.</param>
        /// <param name="userName">Initial value of the UserName property.</param>
        /// <param name="loweredUserName">Initial value of the LoweredUserName property.</param>
        /// <param name="isAnonymous">Initial value of the IsAnonymous property.</param>
        /// <param name="lastActivityDate">Initial value of the LastActivityDate property.</param>
        public static aspnet_Users Createaspnet_Users(global::System.Guid applicationId, global::System.Guid userId, string userName, string loweredUserName, global::System.Boolean isAnonymous, DateTime lastActivityDate)
        {
            aspnet_Users aspnet_Users = new aspnet_Users();
            aspnet_Users.ApplicationId = applicationId;
            aspnet_Users.UserId = userId;
            aspnet_Users.UserName = userName;
            aspnet_Users.LoweredUserName = loweredUserName;
            aspnet_Users.IsAnonymous = isAnonymous;
            aspnet_Users.LastActivityDate = lastActivityDate;
            return aspnet_Users;
        }

        #endregion
        #region Primitive Properties

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public Guid ApplicationId
        {
            get
            {
                return _ApplicationId;
            }
            set
            {
                OnApplicationIdChanging(value);
                ReportPropertyChanging("ApplicationId");
                _ApplicationId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ApplicationId");
                OnApplicationIdChanged();
            }
        }
        private Guid _ApplicationId;
        partial void OnApplicationIdChanging(global::System.Guid value);
        partial void OnApplicationIdChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                if (_UserId != value)
                {
                    OnUserIdChanging(value);
                    ReportPropertyChanging("UserId");
                    _UserId = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }
        private Guid _UserId;
        partial void OnUserIdChanging(Guid value);
        partial void OnUserIdChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                OnUserNameChanging(value);
                ReportPropertyChanging("UserName");
                _UserName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("UserName");
                OnUserNameChanged();
            }
        }
        private string _UserName;
        partial void OnUserNameChanging(string value);
        partial void OnUserNameChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public string LoweredUserName
        {
            get
            {
                return _LoweredUserName;
            }
            set
            {
                OnLoweredUserNameChanging(value);
                ReportPropertyChanging("LoweredUserName");
                _LoweredUserName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("LoweredUserName");
                OnLoweredUserNameChanged();
            }
        }
        private string _LoweredUserName;
        partial void OnLoweredUserNameChanging(string value);
        partial void OnLoweredUserNameChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public string MobileAlias
        {
            get
            {
                return _MobileAlias;
            }
            set
            {
                OnMobileAliasChanging(value);
                ReportPropertyChanging("MobileAlias");
                _MobileAlias = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("MobileAlias");
                OnMobileAliasChanged();
            }
        }
        private string _MobileAlias;
        partial void OnMobileAliasChanging(string value);
        partial void OnMobileAliasChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public global::System.Boolean IsAnonymous
        {
            get
            {
                return _IsAnonymous;
            }
            set
            {
                OnIsAnonymousChanging(value);
                ReportPropertyChanging("IsAnonymous");
                _IsAnonymous = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("IsAnonymous");
                OnIsAnonymousChanged();
            }
        }
        private bool _IsAnonymous;
        partial void OnIsAnonymousChanging(bool value);
        partial void OnIsAnonymousChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        
        [DataMemberAttribute()]
        public DateTime LastActivityDate
        {
            get
            {
                return _LastActivityDate;
            }
            set
            {
                OnLastActivityDateChanging(value);
                ReportPropertyChanging("LastActivityDate");
                _LastActivityDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastActivityDate");
                OnLastActivityDateChanged();
            }
        }
        private DateTime _LastActivityDate;
        partial void OnLastActivityDateChanging(DateTime value);
        partial void OnLastActivityDateChanged();

        #endregion

        #region Navigation Properties

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        //[XmlIgnoreAttribute()]
        //[SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        public ICollection<Person> People
        {
            get; set; 
        }

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        //[XmlIgnoreAttribute()]
        //[SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        public ICollection<SecurityRoleUser> SecurityRoleUsers
        {
            get; set;
        }

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        //[XmlIgnoreAttribute()]
        //[SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        public aspnet_Membership aspnet_Membership
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
