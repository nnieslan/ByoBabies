using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;

namespace ByoBaby.Security
{
    /// <summary>
    /// Custom username and password validator used to authenticate a forms user to the WCF service endpoint.
    /// </summary>
    public class UserValidator : UserNamePasswordValidator
    {
        #region properties

        /// <summary>
        /// Gets or sets the <see cref="IFormsAuthenticationService"/> for the controller.
        /// </summary>
        public IFormsAuthenticationService FormsService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMembershipService"/> for the controller.
        /// </summary>
        public IMembershipService MembershipService { get; set; }

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowPayUserValidator"/>.
        /// </summary>
        public UserValidator()
            : base()
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
        }

        #endregion

        #region methods

        /// <summary>
        /// Implements the custom username validation logic for FlowPay by
        /// looking up the user in the FlowPay database.
        /// </summary>
        /// <param name="userName">The username entered by the end user to validate.</param>
        /// <param name="password">The password entered by the end user to validate.</param>
        public override void Validate(string userName, string password)
        {
            //return fine on pure anonymous
            if(userName.Equals(ServiceUserIdentity.AnonymousUserId, StringComparison.OrdinalIgnoreCase) 
                && string.IsNullOrWhiteSpace(password)) 
                return;

            //validate the user & password against the MembershipService
            if (!MembershipService.ValidateUser(userName, password))
            {
                throw new SecurityTokenValidationException(Strings.AuthenticationFailedMessage);
            }
            else
            {
                this.FormsService.SignIn(userName, true);
            }
        }

        #endregion
    }
}
