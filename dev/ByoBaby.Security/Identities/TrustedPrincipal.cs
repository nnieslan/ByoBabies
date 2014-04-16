using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a trusted <see cref="ByoBabiesUserPrincipal"/> instance which has rights
    /// to perform any action.
    /// </summary>
    internal sealed class TrustedPrincipal : ByoBabiesUserPrincipal, IDisposable
    {
        /// <summary>
        /// The previous principal when the <see cref="Impersonate"/> method was called.
        /// </summary>
        private IPrincipal previousPrincipal;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrustedPrincipal"/> class.
        /// </summary>
        internal TrustedPrincipal()
            : base(new TrustedUserIdentity())
        {
            this.isSystemAdministrator = true;
        }

        /// <summary>
        /// Impersonates a trusted principal with rights to perform any action.
        /// </summary>
        /// <returns>
        /// The impersonated principal.
        /// </returns>
        /// <remarks>
        /// This method should typically be called within a using statement to ensure the previous
        /// principal gets reset after the block is finished.
        /// </remarks>
        internal static TrustedPrincipal Impersonate()
        {
            var principal = new TrustedPrincipal();
            
            // Allow a non-FlowPay principal to be preserved.
            if (HttpContext.Current != null)
            {
                principal.previousPrincipal = HttpContext.Current.User;
            }
            else
            {
                principal.previousPrincipal = Thread.CurrentPrincipal;
            }

            ByoBabiesUserPrincipal.Current = principal;

            return principal;
        }

        /// <summary>
        /// Disposes of the trusted principal, setting the previous principal as the
        /// current principal.
        /// </summary>
        public void Dispose()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = this.previousPrincipal;
            }
            Thread.CurrentPrincipal = this.previousPrincipal;
        }

        /// <summary>
        /// Populates an internal list of roles for the current principal.
        /// </summary>
        protected internal override void PopulateRoles()
        {
            // Do nothing. No roles should be added.
        }
    }
}
