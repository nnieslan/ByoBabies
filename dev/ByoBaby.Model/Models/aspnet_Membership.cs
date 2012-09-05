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
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public partial class aspnet_Membership : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Factory Method

        /// <summary>
        /// Create a new aspnet_Membership object.
        /// </summary>
        /// <param name="applicationId">Initial value of the ApplicationId property.</param>
        /// <param name="userId">Initial value of the UserId property.</param>
        /// <param name="password">Initial value of the Password property.</param>
        /// <param name="passwordFormat">Initial value of the PasswordFormat property.</param>
        /// <param name="passwordSalt">Initial value of the PasswordSalt property.</param>
        /// <param name="isApproved">Initial value of the IsApproved property.</param>
        /// <param name="isLockedOut">Initial value of the IsLockedOut property.</param>
        /// <param name="createDate">Initial value of the CreateDate property.</param>
        /// <param name="lastLoginDate">Initial value of the LastLoginDate property.</param>
        /// <param name="lastPasswordChangedDate">Initial value of the LastPasswordChangedDate property.</param>
        /// <param name="lastLockoutDate">Initial value of the LastLockoutDate property.</param>
        /// <param name="failedPasswordAttemptCount">Initial value of the FailedPasswordAttemptCount property.</param>
        /// <param name="failedPasswordAttemptWindowStart">Initial value of the FailedPasswordAttemptWindowStart property.</param>
        /// <param name="failedPasswordAnswerAttemptCount">Initial value of the FailedPasswordAnswerAttemptCount property.</param>
        /// <param name="failedPasswordAnswerAttemptWindowStart">Initial value of the FailedPasswordAnswerAttemptWindowStart property.</param>
        public static aspnet_Membership Createaspnet_Membership(global::System.Guid applicationId, global::System.Guid userId, string password, int passwordFormat, string passwordSalt, global::System.Boolean isApproved, global::System.Boolean isLockedOut, DateTime createDate, DateTime lastLoginDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, int failedPasswordAttemptCount, DateTime failedPasswordAttemptWindowStart, int failedPasswordAnswerAttemptCount, DateTime failedPasswordAnswerAttemptWindowStart)
        {
            aspnet_Membership aspnet_Membership = new aspnet_Membership();
            aspnet_Membership.ApplicationId = applicationId;
            aspnet_Membership.UserId = userId;
            aspnet_Membership.Password = password;
            aspnet_Membership.PasswordFormat = passwordFormat;
            aspnet_Membership.PasswordSalt = passwordSalt;
            aspnet_Membership.IsApproved = isApproved;
            aspnet_Membership.IsLockedOut = isLockedOut;
            aspnet_Membership.CreateDate = createDate;
            aspnet_Membership.LastLoginDate = lastLoginDate;
            aspnet_Membership.LastPasswordChangedDate = lastPasswordChangedDate;
            aspnet_Membership.LastLockoutDate = lastLockoutDate;
            aspnet_Membership.FailedPasswordAttemptCount = failedPasswordAttemptCount;
            aspnet_Membership.FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
            aspnet_Membership.FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
            aspnet_Membership.FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
            return aspnet_Membership;
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
        partial void OnUserIdChanging(global::System.Guid value);
        partial void OnUserIdChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [DataMemberAttribute()]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                OnPasswordChanging(value);
                ReportPropertyChanging("Password");
                _Password = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Password");
                OnPasswordChanged();
            }
        }
        private string _Password;
        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [DataMemberAttribute()]
        public int PasswordFormat
        {
            get
            {
                return _PasswordFormat;
            }
            set
            {
                OnPasswordFormatChanging(value);
                ReportPropertyChanging("PasswordFormat");
                _PasswordFormat = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("PasswordFormat");
                OnPasswordFormatChanged();
            }
        }
        private int _PasswordFormat;
        partial void OnPasswordFormatChanging(int value);
        partial void OnPasswordFormatChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [DataMemberAttribute()]
        public string PasswordSalt
        {
            get
            {
                return _PasswordSalt;
            }
            set
            {
                OnPasswordSaltChanging(value);
                ReportPropertyChanging("PasswordSalt");
                _PasswordSalt = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("PasswordSalt");
                OnPasswordSaltChanged();
            }
        }
        private string _PasswordSalt;
        partial void OnPasswordSaltChanging(string value);
        partial void OnPasswordSaltChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string MobilePIN
        {
            get
            {
                return _MobilePIN;
            }
            set
            {
                OnMobilePINChanging(value);
                ReportPropertyChanging("MobilePIN");
                _MobilePIN = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("MobilePIN");
                OnMobilePINChanged();
            }
        }
        private string _MobilePIN;
        partial void OnMobilePINChanging(string value);
        partial void OnMobilePINChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                OnEmailChanging(value);
                ReportPropertyChanging("Email");
                _Email = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Email");
                OnEmailChanged();
            }
        }
        private string _Email;
        partial void OnEmailChanging(string value);
        partial void OnEmailChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string LoweredEmail
        {
            get
            {
                return _LoweredEmail;
            }
            set
            {
                OnLoweredEmailChanging(value);
                ReportPropertyChanging("LoweredEmail");
                _LoweredEmail = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LoweredEmail");
                OnLoweredEmailChanged();
            }
        }
        private string _LoweredEmail;
        partial void OnLoweredEmailChanging(string value);
        partial void OnLoweredEmailChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string PasswordQuestion
        {
            get
            {
                return _PasswordQuestion;
            }
            set
            {
                OnPasswordQuestionChanging(value);
                ReportPropertyChanging("PasswordQuestion");
                _PasswordQuestion = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("PasswordQuestion");
                OnPasswordQuestionChanged();
            }
        }
        private string _PasswordQuestion;
        partial void OnPasswordQuestionChanging(string value);
        partial void OnPasswordQuestionChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string PasswordAnswer
        {
            get
            {
                return _PasswordAnswer;
            }
            set
            {
                OnPasswordAnswerChanging(value);
                ReportPropertyChanging("PasswordAnswer");
                _PasswordAnswer = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("PasswordAnswer");
                OnPasswordAnswerChanged();
            }
        }
        private string _PasswordAnswer;
        partial void OnPasswordAnswerChanging(string value);
        partial void OnPasswordAnswerChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public bool IsApproved
        {
            get
            {
                return _IsApproved;
            }
            set
            {
                OnIsApprovedChanging(value);
                ReportPropertyChanging("IsApproved");
                _IsApproved = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("IsApproved");
                OnIsApprovedChanged();
            }
        }
        private bool _IsApproved;
        partial void OnIsApprovedChanging(bool value);
        partial void OnIsApprovedChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public bool IsLockedOut
        {
            get
            {
                return _IsLockedOut;
            }
            set
            {
                OnIsLockedOutChanging(value);
                ReportPropertyChanging("IsLockedOut");
                _IsLockedOut = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("IsLockedOut");
                OnIsLockedOutChanged();
            }
        }
        private bool _IsLockedOut;
        partial void OnIsLockedOutChanging(bool value);
        partial void OnIsLockedOutChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                OnCreateDateChanging(value);
                ReportPropertyChanging("CreateDate");
                _CreateDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreateDate");
                OnCreateDateChanged();
            }
        }
        private DateTime _CreateDate;
        partial void OnCreateDateChanging(DateTime value);
        partial void OnCreateDateChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime LastLoginDate
        {
            get
            {
                return _LastLoginDate;
            }
            set
            {
                OnLastLoginDateChanging(value);
                ReportPropertyChanging("LastLoginDate");
                _LastLoginDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastLoginDate");
                OnLastLoginDateChanged();
            }
        }
        private DateTime _LastLoginDate;
        partial void OnLastLoginDateChanging(DateTime value);
        partial void OnLastLoginDateChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime LastPasswordChangedDate
        {
            get
            {
                return _LastPasswordChangedDate;
            }
            set
            {
                OnLastPasswordChangedDateChanging(value);
                ReportPropertyChanging("LastPasswordChangedDate");
                _LastPasswordChangedDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastPasswordChangedDate");
                OnLastPasswordChangedDateChanged();
            }
        }
        private DateTime _LastPasswordChangedDate;
        partial void OnLastPasswordChangedDateChanging(DateTime value);
        partial void OnLastPasswordChangedDateChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime LastLockoutDate
        {
            get
            {
                return _LastLockoutDate;
            }
            set
            {
                OnLastLockoutDateChanging(value);
                ReportPropertyChanging("LastLockoutDate");
                _LastLockoutDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LastLockoutDate");
                OnLastLockoutDateChanged();
            }
        }
        private DateTime _LastLockoutDate;
        partial void OnLastLockoutDateChanging(DateTime value);
        partial void OnLastLockoutDateChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public int FailedPasswordAttemptCount
        {
            get
            {
                return _FailedPasswordAttemptCount;
            }
            set
            {
                OnFailedPasswordAttemptCountChanging(value);
                ReportPropertyChanging("FailedPasswordAttemptCount");
                _FailedPasswordAttemptCount = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FailedPasswordAttemptCount");
                OnFailedPasswordAttemptCountChanged();
            }
        }
        private int _FailedPasswordAttemptCount;
        partial void OnFailedPasswordAttemptCountChanging(int value);
        partial void OnFailedPasswordAttemptCountChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime FailedPasswordAttemptWindowStart
        {
            get
            {
                return _FailedPasswordAttemptWindowStart;
            }
            set
            {
                OnFailedPasswordAttemptWindowStartChanging(value);
                ReportPropertyChanging("FailedPasswordAttemptWindowStart");
                _FailedPasswordAttemptWindowStart = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FailedPasswordAttemptWindowStart");
                OnFailedPasswordAttemptWindowStartChanged();
            }
        }
        private DateTime _FailedPasswordAttemptWindowStart;
        partial void OnFailedPasswordAttemptWindowStartChanging(DateTime value);
        partial void OnFailedPasswordAttemptWindowStartChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public int FailedPasswordAnswerAttemptCount
        {
            get
            {
                return _FailedPasswordAnswerAttemptCount;
            }
            set
            {
                OnFailedPasswordAnswerAttemptCountChanging(value);
                ReportPropertyChanging("FailedPasswordAnswerAttemptCount");
                _FailedPasswordAnswerAttemptCount = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FailedPasswordAnswerAttemptCount");
                OnFailedPasswordAnswerAttemptCountChanged();
            }
        }
        private int _FailedPasswordAnswerAttemptCount;
        partial void OnFailedPasswordAnswerAttemptCountChanging(int value);
        partial void OnFailedPasswordAnswerAttemptCountChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get
            {
                return _FailedPasswordAnswerAttemptWindowStart;
            }
            set
            {
                OnFailedPasswordAnswerAttemptWindowStartChanging(value);
                ReportPropertyChanging("FailedPasswordAnswerAttemptWindowStart");
                _FailedPasswordAnswerAttemptWindowStart = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FailedPasswordAnswerAttemptWindowStart");
                OnFailedPasswordAnswerAttemptWindowStartChanged();
            }
        }
        private DateTime _FailedPasswordAnswerAttemptWindowStart;
        partial void OnFailedPasswordAnswerAttemptWindowStartChanging(DateTime value);
        partial void OnFailedPasswordAnswerAttemptWindowStartChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>

        [DataMemberAttribute()]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                ReportPropertyChanging("Comment");
                _Comment = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private string _Comment;
        partial void OnCommentChanging(string value);
        partial void OnCommentChanged();

        #endregion

        #region Navigation Properties

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        //[XmlIgnoreAttribute()]
        //[SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        public aspnet_Users aspnet_Users
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
