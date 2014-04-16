using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using System.Diagnostics;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a membership service for managing user accounts using the 
    /// <see cref="MembershipProvider"/>.
    /// </summary>
    public class AccountMembershipService : IMembershipService
    {
        /// <summary>
        /// The current instance of the <see cref="MembershipProvider"/>.
        /// </summary>
        private readonly MembershipProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        public AccountMembershipService()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> using the
        /// specified instance of the <see cref="MembershipProvider"/> class.
        /// </summary>
        /// <param name="provider">
        /// The <see cref="MembershipProvider"/> instance for the service.
        /// </param>
        public AccountMembershipService(MembershipProvider provider)
        {
            this.provider = this.provider ?? Membership.Provider;
        }

        /// <summary>
        /// Gets the minimum password length for the service.
        /// </summary>
        public int MinPasswordLength
        {
            get
            {
                return provider.MinRequiredPasswordLength;
            }
        }

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
        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return provider.ValidateUser(userName, password);
        }

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
        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;
            provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }



        /// <summary>
        /// Determines if the user already exists.
        /// </summary>
        /// <param name="userName">
        /// The user name of the account to do an existence check on.
        /// </param>
        /// <returns>
        /// True if the user exists, else false.
        /// </returns>
        public bool UserExists(string userName)
        {
            var user = provider.GetUser(userName, false);
            return (user != null);
        }

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="tokenValue">
        /// The password reset token identifying the user for which to change the password.
        /// </param>
        /// <param name="ipAddress">
        /// The IP address from which the password reset token was requested.
        /// </param>
        /// <param name="newPassword">
        /// The new password for the user.
        /// </param>
        /// <param name="removeTokenOnSuccess">
        /// Whether to remove the password reset token if the password change is successful.
        /// </param>
        /// <returns>
        /// <b>true</b> if the password change was successful; <b>false</b> otherwise.
        /// </returns>
        /// <remarks>
        /// Allows anonymous users to reset their password.
        /// </remarks>
        public bool ChangePassword(string tokenValue, 
            string ipAddress,
            string newPassword, 
            bool removeTokenOnSuccess = true)
        {
            if (String.IsNullOrWhiteSpace(tokenValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    Strings.ArgumentMissing,
                    "tokenValue"),
                    "tokenValue"
                    );
            }

            if (String.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    Strings.ArgumentMissing,
                    "newPassword"),
                    "newPassword"
                    );
            }

            long personId;
            if (PasswordResetToken.ValidateTokenValue(tokenValue, ipAddress, out personId))
            {
                string userName = Person.GetUserName(personId);
                if (userName != null)
                {
                    // Elevate to a trusted principle for the reset operation because the user is not logged on.
                    using (TrustedPrincipal trustedPrinciple = TrustedPrincipal.Impersonate())
                    {
                        bool result = ChangePassword(userName, newPassword);
                        
                        // Remove the token if desired.
                        if (result && removeTokenOnSuccess)
                        {
                            PasswordResetToken.RemovePasswordResetToken(personId);
                        }

                        return result;
                    }
                }
                else
                {
                    Trace.TraceError("The password reset token supplied by IP address {0} could not be validated because the person Id {1} is not associated with a username: {2}",
                        ipAddress,
                        personId,
                        tokenValue);
                    return false;
                }
            }
            else
            {
                Trace.TraceError("The password reset token supplied by IP address {0} could not be validated: {1}",
                    ipAddress,
                    tokenValue);
                return false;
            }
        }

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="userName">
        /// The user name for which to change the password.
        /// </param>
        /// <param name="newPassword">
        /// The new password for the user.
        /// </param>
        /// <returns>
        /// <b>true</b> if the password change was successful; <b>false</b> otherwise.
        /// </returns>
        /// <remarks>
        /// Only system administrators are permitted to change other user's passwords.
        /// </remarks>
        public bool ChangePassword(string userName, string newPassword)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                if (!Thread.CurrentPrincipal.IsSystemAdministrator() &&
                    Thread.CurrentPrincipal.Identity.Name != userName)
                {
                    throw new UnauthorizedAccessException("You are not permitted to perform this action.");
                }

                MembershipUser currentUser = provider.GetUser(userName, true /* userIsOnline */);
                string tempPassword = currentUser.ResetPassword();
                return currentUser.ChangePassword(tempPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="userName">
        /// The user name for which to change the password.
        /// </param>
        /// <param name="oldPassword">
        /// The current password for the user.
        /// </param>
        /// <param name="newPassword">
        /// The new password for the user.
        /// </param>
        /// <returns>
        /// <b>true</b> if the password change was successful; <b>false</b> otherwise.
        /// </returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }        

        /// <summary>
        /// Sends a password reset email for the specified user name or email address.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP address from which the password reset request originated.
        /// </param>
        /// <param name="userName">
        /// The user name for which to send a reset email.
        /// </param>
        /// <param name="emailAddress">
        /// The email address for which to send a reset email.
        /// </param>
        public void SendPasswordResetEmail(string ipAddress, string userName, string emailAddress)
        {
            if (String.IsNullOrWhiteSpace(userName) && String.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException(Strings.UserNameOrEmailAddressRequired);
            }
                
            // Attempt to get the current user name by email address.
            if (String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(emailAddress))
            {
                userName = provider.GetUserNameByEmail(emailAddress);
            }

            if(String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(Strings.InvalidUserNameOrEmailAddress);
            }

            // Attempt to get the current user and confirm it is approved for use.
            MembershipUser user = provider.GetUser(userName, false);
            if (user == null || !user.IsApproved)
            {
                throw new ArgumentException(Strings.InvalidUserNameOrEmailAddress);
            }

            // Attempt to get the person Id associated with the user.
            PasswordResetToken token = null;
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                long? personId = Person.GetPersonId(entityContext, userName);
                if (!personId.HasValue)
                {
                    throw new ArgumentException(Strings.InvalidUserNameOrEmailAddress);
                }

                // Get a password reset antiforgery token.
                const int twentyFourHoursInMinutes = 1440;
                token = PasswordResetToken.CreateToken(entityContext, personId.Value, ipAddress, twentyFourHoursInMinutes);
            }

            //TODO - Reenable Email notification later.
            //var notifier = new ResetPasswordNotifier(userName, emailAddress, token.Value, Strings.TwentyFourHours);
            //notifier.SendNotification();
        }

        /// <summary>
        /// Unlocks the user account associated with the specified person Id.
        /// </summary>
        /// <param name="personId">The unique identifier for the person whose account will be unlocked.</param>
        /// <returns><b>true</b> if the account unlock was successful or the account is already unlocked; <b>false</b> otherwise.</returns>
        public bool UnlockAccount(long personId)
        {
            return UnlockAccount(Person.GetUserName(personId));
        }

        /// <summary>
        /// Unlocks the user account associated with the specified user name.
        /// </summary>
        /// <param name="userName">The user name of the person whose account will be unlocked.</param>
        /// <returns><b>true</b> if the account unlock was successful or the account is already unlocked; <b>false</b> otherwise.</returns>
        public bool UnlockAccount(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            try
            {
                MembershipUser currentUser = provider.GetUser(userName, true /* userIsOnline */);
                if (currentUser.IsLockedOut)
                {
                    //only unlock if the account is currently locked
                    return currentUser.UnlockUser();
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
