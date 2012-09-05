using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Threading;
using ByoBaby.Model;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a custom principal for the FlowPay web application.
    /// </summary>
    public class ByoBabiesUserPrincipal : IPrincipal
    {
        /// <summary>
        /// The <see cref="UserIdentity"/> for the principal.
        /// </summary>
        private UserIdentity identity;

        /// <summary>
        /// The list of security roles for the user.
        /// </summary>
        private List<SecurityRoleUser> securityRoles;

        /// <summary>
        /// Indicates whether the user is a system administrator.
        /// </summary>
        protected bool isSystemAdministrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByoBabiesUserPrincipal"/> class.
        /// </summary>
        /// <param name="identity">
        /// The <see cref="UserIdentity"/> for the principal.
        /// </param>
        public ByoBabiesUserPrincipal(UserIdentity identity)
        {
            this.identity = identity;
        }

        /// <summary>
        /// Gets the identity for the principal.
        /// </summary>
        public IIdentity Identity
        {
            get 
            {
                return this.identity;
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the user is a system administrator.
        /// </summary>
        public bool IsSystemAdministrator 
        {
            get
            {
                PopulateRoles();

                return this.isSystemAdministrator;
            }
        }

        /// <summary>
        /// Indicates whether the principal is in the specified role.
        /// </summary>
        /// <param name="role">
        /// The name of the role to check.
        /// </param>
        /// <returns>
        /// <b>true</b> if the user is in the role; <b>false</b> otherwise.
        /// </returns>
        public bool IsInRole(string role)
        {
            PopulateRoles();

            // A system administrator is assumed to be in all roles.
            return this.IsSystemAdministrator 
                || this.securityRoles.Exists(sr => sr.SecurityRole.Name == role);
        }

        /// <summary>
        /// Gets the current <see cref="ByoBabiesUserPrincipal"/> or <b>null</b> if no
        /// FlowPayUserPrincipal is present.
        /// </summary>
        public static ByoBabiesUserPrincipal Current
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.User as ByoBabiesUserPrincipal;
                }
                else
                {
                    return Thread.CurrentPrincipal as ByoBabiesUserPrincipal;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }

                Thread.CurrentPrincipal = value;
            }
        }

        /// <summary>
        /// Populates an internal list of roles for the current principal.
        /// </summary>
        protected internal virtual void PopulateRoles()
        {
            if (this.securityRoles == null)
            {
                this.securityRoles = SecurityRole.GetUserRoles(this.identity.UserId);

                // Initialize system administrator membership.
                this.isSystemAdministrator = this.securityRoles.Exists(sr =>
                    sr.SecurityRole.Name == SecurityRole.SystemAdministratorRoleName);
            }
        }
    }
}