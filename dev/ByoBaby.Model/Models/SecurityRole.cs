using System;
using System.Collections.Generic;
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
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    public partial class SecurityRole : INotifyPropertyChanged, INotifyPropertyChanging
    {

        /// <summary>
        /// The name for the system administrator role.
        /// </summary>
        public const string SystemAdministratorRoleName = "SystemAdministrator";

        /// <summary>
        /// The name for the organization administrator role.
        /// </summary>
        public const string GroupAdministratorRoleName = "GroupAdministrator";

        /// <summary>
        /// The name for the organization member  role.
        /// </summary>
        public const string GroupMemberRoleName = "GroupMember";


        #region Factory Method

        /// <summary>
        /// Create a new SecurityRole object.
        /// </summary>
        /// <param name="securityRoleId">Initial value of the SecurityRoleId property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        public static SecurityRole CreateSecurityRole(long securityRoleId, string name)
        {
            SecurityRole securityRole = new SecurityRole();
            securityRole.SecurityRoleId = securityRoleId;
            securityRole.Name = name;
            return securityRole;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        [Key]
        public long SecurityRoleId
        {
            get
            {
                return _SecurityRoleId;
            }
            set
            {
                if (_SecurityRoleId != value)
                {
                    ReportPropertyChanging("SecurityRoleId");
                    _SecurityRoleId = value;
                    ReportPropertyChanged("SecurityRoleId");
                }
            }
        }
        private long _SecurityRoleId;
        
        [DataMemberAttribute()]
        [Required]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                ReportPropertyChanging("Name");
                _Name = value;
                ReportPropertyChanged("Name");
            }
        }
        private string _Name;
        
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ICollection<SecurityRolePermission> SecurityRolePermissions
        {
            get; set;
        }

        [DataMemberAttribute()]
        public ICollection<SecurityRoleUser> SecurityRoleUsers
        {
            get; set; 
        }

        #endregion

        #region methods

        /// <summary>
        /// Adds the specified user to a role within the context of an organization.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to which a user should be added.
        /// </param>
        /// <param name="userId">
        /// The Id of the user to add to the role.
        /// </param>
        /// <param name="groupId">
        /// The Id of the group in which the user has the role.
        /// </param>
        public static void AddUserToRole(string roleName, Guid userId, long groupId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                long? roleId = GetRoleId(entityContext, roleName);

                if (roleId.HasValue)
                {
                    // Ensure the user does not already have the role.
                    SecurityRoleUser roleUser = entityContext.SecurityRoleUsers
                        .FirstOrDefault(ru => ru.SecurityRoleId == roleId.Value
                        && ru.UserId == userId
                        && ru.GroupId == groupId);

                    if (roleUser == null)
                    {
                        roleUser = new SecurityRoleUser()
                        {
                            SecurityRoleId = roleId.Value,
                            UserId = userId,
                            GroupId = groupId
                        };

                        entityContext.SecurityRoleUsers.Add(roleUser);
                        entityContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified user from a role within the context of an organization.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role from which to remove a user.
        /// </param>
        /// <param name="userId">
        /// The Id of the user to remove from the role.
        /// </param>
        /// <param name="groupId">
        /// The Id of the group in which the user has the role.
        /// </param>
        public static void RemoveUserFromRole(string roleName, Guid userId, long groupId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                long? roleId = GetRoleId(entityContext, roleName);

                if (roleId.HasValue)
                {
                    // Ensure the user does not already have the role.
                    SecurityRoleUser roleUser = entityContext.SecurityRoleUsers
                        .FirstOrDefault(ru => ru.SecurityRoleId == roleId.Value
                        && ru.UserId == userId
                        && ru.GroupId == groupId);

                    if (roleUser == null)
                    {
                        entityContext.SecurityRoleUsers.Remove(roleUser);
                        entityContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the roles for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The Id of the user for which to get the roles.
        /// </param>
        /// <returns>
        /// A list of <see cref="SecurityRoleUser"/> instances.
        /// </returns>
        public static List<SecurityRoleUser> GetUserRoles(Guid userId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                return GetUserRoles(entityContext, userId);
            }
        }

        /// <summary>
        /// Gets the roles for the specified user.
        /// </summary>
        /// <param name="entityContext">
        /// The entity context.
        /// </param>
        /// <param name="userId">
        /// The Id of the user for which to get the roles.
        /// </param>
        /// <returns>
        /// A list of <see cref="SecurityRoleUser"/> instances.
        /// </returns>
        public static List<SecurityRoleUser> GetUserRoles(ByoBabyRepository entityContext, Guid userId)
        {
            return (entityContext.SecurityRoleUsers
                            .Include("SecurityRole")
                            .Include("Group")
                            .Where(ru => ru.UserId == userId))
                            .ToList();
        }

        /// <summary>
        /// Gets the Id of the role with the specified name.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> to use for database interactions.
        /// </param>
        /// <param name="roleName">
        /// The name of the role.
        /// </param>
        /// <returns>
        /// The role Id or null if no role is found with the specified Id.
        /// </returns>
        public static long? GetRoleId(ByoBabyRepository entityContext, string roleName)
        {
            SecurityRole role = entityContext.SecurityRoles.FirstOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                return role.SecurityRoleId;
            }

            return null;
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
