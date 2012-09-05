using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using ByoBaby.Model;

namespace ByoBaby.Security
{
    /// <summary>
    /// Defines extension methods for <see cref="IPrincipal"/> instances.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Fetches the currently authenticated user and populates their <see cref="FlowPayUserPrincipal"/>.
        /// </summary>
        /// <param name="context">The WCF <see cref="OperationContext"/>.</param>
        /// <returns>The authenticated user's <see cref="FlowPayUserPrincipal"/>.</returns>
        public static ByoBabiesUserPrincipal GetAuthenticatedServiceUser(this System.ServiceModel.OperationContext context)
        {
            ByoBabiesUserPrincipal currentUser = null;
            if (HttpContext.Current != null)
            {
                currentUser = HttpContext.Current.User as ByoBabiesUserPrincipal;
            }

            if (currentUser == null && context != null)
            {
                var user = context.ServiceSecurityContext;
                if (user != null 
                    && user.PrimaryIdentity.IsAuthenticated
                    && user.PrimaryIdentity.AuthenticationType == typeof(UserValidator).Name
                    && !user.PrimaryIdentity.Name.Equals(
                        ServiceUserIdentity.AnonymousUserId, StringComparison.OrdinalIgnoreCase))
                {
                    var genericIdentity = (GenericIdentity)user.PrimaryIdentity;
                    var flowPayIdentity = new ServiceUserIdentity(genericIdentity);
                    currentUser = new ByoBabiesUserPrincipal(flowPayIdentity);

                }
            }
            return currentUser;
        }

        /// <summary>
        /// Indicates whether the specified principal is a system administrator.
        /// </summary>
        /// <param name="principal">
        /// The <see cref="IPrincipal"/> for which to perform the role check.
        /// </param>
        /// <returns>
        /// <b>true</b> if the principal is a system administrator; <b>false</b> otherwise.
        /// </returns>
        public static bool IsSystemAdministrator(this IPrincipal principal)
        {
            var userPrincipal = principal as ByoBabiesUserPrincipal;
            if (userPrincipal != null)
            {
                return userPrincipal.IsSystemAdministrator;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the specified principal is an organization member.
        /// </summary>
        /// <param name="principal">
        /// The <see cref="IPrincipal"/> for which to perform the role check.
        /// </param>
        /// <param name="organizationId">
        /// The organization for which to perform the role check.
        /// </param>
        /// <returns>
        /// <b>true</b> if the principal is an organization member; <b>false</b> otherwise.
        /// </returns>
        public static bool IsGroupMember(this IPrincipal principal, long groupId)
        {
            return principal.IsInRole(groupId, SecurityRole.GroupMemberRoleName);
        }

        /// <summary>
        /// Gets the unique Id for the person from the specified principal object.
        /// </summary>
        /// <param name="user">
        /// The <see cref="IPrincipal"/> for which to get a person Id.
        /// </param>
        /// <returns>
        /// The Id for the person or null if no person Id is defined on the principal object.
        /// </returns>
        public static long? GetPersonId(this IPrincipal principal)
        {
            if (principal != null && principal.Identity != null)
            {
                var identity = principal.Identity as UserIdentity;
                if (identity != null)
                {
                    return identity.PersonId;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the display name for the person from the specified principal object.
        /// </summary>
        /// <param name="user">
        /// The <see cref="IPrincipal"/> for which to get a person Id.
        /// </param>
        /// <returns>
        /// The display name for the person or null if no person Id is defined on the principal object.
        /// </returns>
        public static string GetDisplayName(this IPrincipal principal)
        {
            if (principal != null && principal.Identity != null)
            {
                var identity = principal.Identity as UserIdentity;
                if (identity != null)
                {
                    return identity.EmailAddress;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the user Id from the specified principal object.
        /// </summary>
        /// <param name="user">
        /// The <see cref="IPrincipal"/> for which to get a user Id.
        /// </param>
        /// <returns>
        /// The user Id for the principal or Guid.Empty if no user Id is
        /// defined on the principal object.
        /// </returns>
        public static Guid GetUserId(this IPrincipal principal)
        {
            if (principal != null && principal.Identity != null)
            {
                var identity = principal.Identity as UserIdentity;
                if (identity != null)
                {
                    return identity.UserId;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Indicates whether the specified principal is in the specified role for an organization.
        /// </summary>
        /// <param name="principal">
        /// The <see cref="IPrincipal"/> for which to perform the role check.
        /// </param>
        /// <param name="organizationId">
        /// The organization for which to perform the role check.
        /// </param>
        /// <param name="role">
        /// The name of the role for which to perform the check.
        /// </param>
        /// <returns>
        /// <b>true</b> if the principal is in the role; <b>false</b> otherwise.
        /// </returns>
        public static bool IsInRole(this IPrincipal principal, long organizationId, string role)
        {
            var userPrincipal = principal as ByoBabiesUserPrincipal;
            if (userPrincipal != null)
            {
                return userPrincipal.IsInRole(organizationId, role);
            }

            return false;
        }
    }
}