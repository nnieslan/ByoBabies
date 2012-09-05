using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByoBaby.Security
{
    /// <summary>
    /// Defines an interface to support forms authentication.
    /// </summary>
    public interface IFormsAuthenticationService
    {
        /// <summary>
        /// Performs a sign-in operation for the specified user.
        /// </summary>
        /// <param name="userName">
        /// The user name to sign in.
        /// </param>
        /// <param name="createPersistentCookie">
        /// Whether to create a persistent cookie for the user.
        /// </param>
        void SignIn(string userName, bool createPersistentCookie);

        /// <summary>
        /// Performs a sign-out operation for the current user.
        /// </summary>
        void SignOut();
    }
}
