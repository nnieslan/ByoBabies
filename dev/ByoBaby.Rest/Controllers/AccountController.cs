using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ByoBaby.Rest.Models;
using ByoBaby.Rest.Helpers;
using ByoBaby.Rest.Providers;
using ByoBaby.Rest.Results;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Controllers
{
    /// <summary>
    /// An <see cref="ApiController"/> for user account actions such as logging in and out and fetching profile information.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";

        #region ctor
        public AccountController()
            : this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
        {
        }

        public AccountController(UserManager<IdentityUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        #endregion

        #region properties
        public UserManager<IdentityUser> UserManager { get; private set; }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        #endregion

        #region actions

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                UserName = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                UserName = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // GET api/Account/ExternalLoginCallback
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLoginCallback", Name = "ExternalLoginCallback")]
        public async Task<IHttpActionResult> GetExternalLoginCallback(string provider)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await UserManager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok(new { registered = hasRegistered});
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await UserManager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok(new { registered = hasRegistered });
        }

        //[AllowAnonymous]
        //public async Task<IHttpActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
        //    if (result == null || result.Identity == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (idClaim == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var login = new UserLoginInfo(idClaim.Issuer, idClaim.Value);
        //    var name = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", "");

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = name });
        //    }
        //}

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl = "/oauth/logincallback", bool generateState = false) 
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl + "?provider=" + description.AuthenticationType).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityUser user = new IdentityUser
            {
                UserName = model.UserName
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            IdentityUser user = new IdentityUser
            {
                UserName = model.UserName
            };
            user.Logins.Add(new IdentityUserLogin
            {
                LoginProvider = externalLogin.LoginProvider,
                ProviderKey = externalLogin.ProviderKey
            });
            IdentityResult result = await UserManager.CreateAsync(user);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion


        //private ByoBabyRepository db = new ByoBabyRepository();

        //#region Properties

        ///// <summary>
        ///// Gets or sets the <see cref="IFormsAuthenticationService"/> for the controller.
        ///// </summary>
        //public IFormsAuthenticationService FormsService { get; set; }

        ///// <summary>
        ///// Gets or sets the <see cref="IMembershipService"/> for the controller.
        ///// </summary>
        //public IMembershipService MembershipService { get; set; }

        //#endregion

        //#region methods

        ///// <summary>
        ///// Overrides the initialization of the current controller to include initialization of the authentication and authorization services.
        ///// </summary>
        ///// <param name="controllerContext">The current controller context.</param>
        //protected override void Initialize(HttpControllerContext controllerContext)
        //{
        //    Log.LogInformation("AccountController.Initialize() - Initializing authentication services.");

        //    if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
        //    if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

        //    base.Initialize(controllerContext);
        //}


        ///// <summary>
        ///// Gets the currently logged in user's <see cref="Person"/> containing all profile information.
        ///// </summary>
        ///// <returns></returns>
        //[System.Web.Http.Authorize()]
        //public ProfileViewModel Get()
        //{
        //    Log.LogInformation("Entering AccountController.Get()");
        //    try
        //    {
        //        ByoBabiesUserPrincipal currentUser =
        //            HttpContext.Current.User as ByoBabiesUserPrincipal;

        //        var id = currentUser.GetUserId();
        //        using (ByoBabyRepository context = new ByoBabyRepository())
        //        {
        //            var person = context.People
        //                .Include("Children")
        //                .Include("MemberOf")
        //                .FirstOrDefault(p => p.UserId == id);
        //            if (person == null)
        //            {
        //                this.Logout();
        //                return (ProfileViewModel)null;
        //            }
        //            return ProfileViewModel.FromPerson(person);
        //        }
        //    }
        //    finally
        //    {
        //        Log.LogInformation("Exiting AccountController.Get()");
        //    }
        //}


        //public HttpResponseMessage Register(RegisterViewModel userInformation)
        //{
        //    Log.LogInformation("Entering AccountController.Register()");

        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        this.Logout();
        //    }

        //    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

        //    var status = MembershipService.CreateUser(
        //        userInformation.Email, userInformation.Password, userInformation.Email);

        //    if (status == System.Web.Security.MembershipCreateStatus.DuplicateUserName)
        //    {
        //        response.StatusCode = System.Net.HttpStatusCode.Forbidden;
        //        response.ReasonPhrase = "A user with this email address already exists.";
        //    }
        //    else if (status != System.Web.Security.MembershipCreateStatus.Success)
        //    {
        //        response.StatusCode = System.Net.HttpStatusCode.Forbidden;
        //        response.ReasonPhrase = string.Format(
        //            System.Globalization.CultureInfo.InvariantCulture, 
        //            "The user could not be created. Error : {0}", 
        //            status.ToString());
        //    }
        //    else
        //    {
        //        //log in the user
        //        response = this.Login(
        //            new LogOnModel()
        //            {
        //                UserName = userInformation.Email,
        //                Password = userInformation.Password
        //            });

        //        //create a stubbed in profile for the newly logged in user
        //        using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
        //        {
        //            var user = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == userInformation.Email);
        //            if (user != null)
        //            {
        //                var nameParts = userInformation.DisplayName.Split(' ');
        //                var first = nameParts[0];
        //                var last = (nameParts.Length > 1 ? userInformation.DisplayName.Split(' ')[1] : string.Empty);
        //                var profile = new Person()
        //                {
        //                    Email = userInformation.Email,
        //                    FirstName = first,
        //                    LastName = last,
        //                    UserId = user.UserId,
        //                    MemberSince = DateTime.Now,
        //                    LastUpdated = DateTime.Now,
        //                };
        //                db.People.Add(profile);
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                throw new HttpResponseException(
        //                    new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        //                    {
        //                        ReasonPhrase = "Registration failed"
        //                    });
        //            }
        //        }
        //    }

        //    return response;
        //}

        ///// <summary>
        ///// An HTTP GET action for logging in the user using the credentials indicated.
        ///// </summary>
        ///// <param name="credentials"></param>
        ///// <returns></returns>
        //public HttpResponseMessage Login(LogOnModel userCredentials)
        //{
        //    Log.LogInformation("Entering AccountController.PostLogin()");

        //    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        this.Logout();
        //        //HttpContext.Current.User = null;
        //    }

        //    if (MembershipService.ValidateUser(userCredentials.UserName, userCredentials.Password))
        //    {
        //        FormsService.SignIn(userCredentials.UserName, (userCredentials.RememberMe == "yes" ? true : false));
        //    }
        //    else
        //    {
        //        response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
        //        if (Person.GetIsLockedOut(userCredentials.UserName))
        //        {
        //            response.ReasonPhrase = "For security reasons, this account has been locked.";
        //        }
        //        else
        //        {
        //            response.ReasonPhrase = "The UserName or Password is invalid.";
        //        }
        //        response.Content = new StringContent(response.ReasonPhrase);
        //    }

        //    Log.LogInformation("Exiting AccountController.PostLogin()");
        //    return response;
        //}

        ///// <summary>
        ///// An HTTP POST action for logging out the current user.
        ///// </summary>
        //[Authorize()]
        //public HttpResponseMessage Logout()
        //{
        //    Log.LogInformation("Entering AccountController.PostLogout()");

        //    FormsService.SignOut();

        //    Log.LogInformation("Exiting AccountController.PostLogout()");
        //    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        //}

        //#endregion
    }
}
