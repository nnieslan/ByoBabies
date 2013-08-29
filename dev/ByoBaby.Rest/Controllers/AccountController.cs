using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
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
        private ByoBabyRepository db = new ByoBabyRepository();

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
        public ProfileViewModel Get()
        {
            Log.LogInformation("Entering AccountController.Get()");
            try
            {
                ByoBabiesUserPrincipal currentUser =
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

                var id = currentUser.GetUserId();
                using (ByoBabyRepository context = new ByoBabyRepository())
                {
                    var person = context.People
                        .Include("Children")
                        .Include("MemberOf")
                        .FirstOrDefault(p => p.UserId == id);
                    return ProfileViewModel.FromPerson(person);
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

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                this.Logout();
            }

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var status = MembershipService.CreateUser(
                userInformation.Email, userInformation.Password, userInformation.Email);

            if (status == System.Web.Security.MembershipCreateStatus.DuplicateUserName)
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
                //log in the user
                response = this.Login(
                    new LogOnModel()
                    {
                        UserName = userInformation.Email,
                        Password = userInformation.Password
                    });

                //create a stubbed in profile for the newly logged in user
                using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
                {
                    var user = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == userInformation.Email);
                    if (user != null)
                    {
                        var nameParts = userInformation.DisplayName.Split(' ');
                        var first = nameParts[0];
                        var last = (nameParts.Length > 1 ? userInformation.DisplayName.Split(' ')[1] : string.Empty);
                        var profile = new Person()
                        {
                            Email = userInformation.Email,
                            FirstName = first,
                            LastName = last,
                            UserId = user.UserId,
                            MemberSince = DateTime.Now,
                            LastUpdated = DateTime.Now,
                        };
                        db.People.Add(profile);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new HttpResponseException(
                            new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                            {
                                ReasonPhrase = "Registration failed"
                            });
                    }
                }
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
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                this.Logout();
                //HttpContext.Current.User = null;
            }

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
        [Authorize()]
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
