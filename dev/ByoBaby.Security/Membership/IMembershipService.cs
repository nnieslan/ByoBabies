using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ByoBaby.Security
{
    /// <summary>
    /// Defines an interface for a membership service.
    /// </summary>
    public interface IMembershipService
    {
        /// <summary>
        /// Gets the minimum password length for the service.
        /// </summary>
        int MinPasswordLength { get; }

        /// <summary>
        /// Validates the user name and password for an account.
        /// </summary>
        /// <param name="userName">
        /// The user name for the account.
        /// </param>
        /// <param name="password">
        /// The password for the account.
        /// </param>
        /// <returns>
        /// <b>true</b> if the user name and password are valid; <b>false</b> otherwise.
        /// </returns>
        bool ValidateUser(string userName, string password);

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="userName">
        /// The user name for the new account.
        /// </param>
        /// <param name="password">
        /// The password for the new account.
        /// </param>
        /// <param name="email">
        /// The email address for the new account.
        /// </param>
        /// <returns>
        /// The <see cref="MembershipCreateStatus"/> indicating the status of the 
        /// account creation process.
        /// </returns>
        MembershipCreateStatus CreateUser(string userName, string password, string email);

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="userName">
        /// The user name of the account for which to change the password.
        /// </param>
        /// <param name="password">
        /// The new password for the account.
        /// </param>
        /// <returns>
        /// <b>true</b> if the password was changed successfully; <b>false</b> otherwise.
        /// </returns>
        /// <remarks>
        /// When implemented in a derived type, only system administrators 
        /// should be permitted to change another user's passwords.
        /// </remarks>
        bool ChangePassword(string userName, string password);

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="userName">
        /// The user name of the account for which to change the password.
        /// </param>
        /// <param name="oldPassword">
        /// The current password for the account.
        /// </param>
        /// <param name="newPassword">
        /// The new password for the account.
        /// </param>
        /// <returns>
        /// <b>true</b> if the password was changed successfully; <b>false</b> otherwise.
        /// </returns>
        bool ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Determines if the user already exists.
        /// </summary>
        /// <param name="userName">
        /// The user name of the account to do an existence check on.
        /// </param>
        /// <returns>
        /// True if the user exists, else false.
        /// </returns>
        bool UserExists(string userName);

    }
}
