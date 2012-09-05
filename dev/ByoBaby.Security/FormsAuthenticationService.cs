using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a class to handle forms authentication.
    /// </summary>
    public class FormsAuthenticationService : IFormsAuthenticationService
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
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException(Strings.InvalidEmptyArgumentMessage, "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// Performs a sign-out operation for the current user.
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
