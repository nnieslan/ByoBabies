using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.ServiceModel;
using ByoBaby.Rest.Models;
using ByoBaby.Rest.Helpers;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Controllers
{
    /// <summary>
    /// An <see cref="ApiController"/> for user account actions such as logging in and out and fetching profile information.
    /// </summary>
    public class AccountController : ApiController
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="IFormsAuthenticationService"/> for the controller.
        /// </summary>
        public IFormsAuthenticationService FormsService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMembershipService"/> for the controller.
        /// </summary>
        public IMembershipService MembershipService { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Overrides the initialization of the current controller to include initialization of the authentication and authorization services.
        /// </summary>
        /// <param name="controllerContext">The current controller context.</param>
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            Log.LogInformation("AccountController.Initialize() - Initializing authentication services.");

            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(controllerContext);
        }


        /// <summary>
        /// Gets the currently logged in user's <see cref="Person"/> containing all profile information.
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Authorize()]
        public Person Get()
        {
            Log.LogInformation("Entering AccountController.Get()");
            try
            {
                ByoBabiesUserPrincipal currentUser = 
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

                var id = currentUser.GetUserId();
                using (ByoBabyRepository context = new ByoBabyRepository())
                {
                    var person = context.People.FirstOrDefault(p => p.UserId == id);
                    return person;
                }
            }
            finally
            {
                Log.LogInformation("Exiting AccountController.Get()");
            }
        }


        public HttpResponseMessage Register(RegisterViewModel userInformation)
        {
            Log.LogInformation("Entering AccountController.Register()");

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            
           var status = MembershipService.CreateUser(userInformation.Email, userInformation.Password, userInformation.Email);
            

            if(status == System.Web.Security.MembershipCreateStatus.DuplicateUserName)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ReasonPhrase = "A user with this email address already exists.";
            }
            else if (status != System.Web.Security.MembershipCreateStatus.Success)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.ReasonPhrase = string.Format("The user could not be created. Error : {0}", status.ToString());
            }
            else
            {
                response = this.Login(
                    new LogOnModel()
                    {
                        UserName = userInformation.Email,
                        Password = userInformation.Password
                    });
            }
    
            return response;
        }

        /// <summary>
        /// An HTTP GET action for logging in the user using the credentials indicated.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public HttpResponseMessage Login(LogOnModel userCredentials)
        {
            Log.LogInformation("Entering AccountController.PostLogin()");

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            if (MembershipService.ValidateUser(userCredentials.UserName, userCredentials.Password))
            {
                FormsService.SignIn(userCredentials.UserName, (userCredentials.RememberMe == "yes" ? true : false));
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                if (Person.GetIsLockedOut(userCredentials.UserName))
                {
                    response.ReasonPhrase = "For security reasons, this account has been locked.";
                }
                else
                {
                    response.ReasonPhrase = "The UserName or Password is invalid.";
                }
                response.Content = new StringContent(response.ReasonPhrase);
            }

            Log.LogInformation("Exiting AccountController.PostLogin()");
            return response;
        }

        /// <summary>
        /// An HTTP POST action for logging out the current user.
        /// </summary>
        public HttpResponseMessage Logout()
        {
            Log.LogInformation("Entering AccountController.PostLogout()");

            FormsService.SignOut();

            Log.LogInformation("Exiting AccountController.PostLogout()");
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        #endregion
    }
}
