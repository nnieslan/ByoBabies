using System;
using System.Linq;
using System.Security.Principal;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a custom identity for the FlowPay web application.
    /// </summary>
    public abstract class UserIdentity : IIdentity
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        public UserIdentity()
        {
        }

        /// <summary>
        /// Gets the authentication type for the identity.
        /// </summary>
        public string AuthenticationType
        {
            get 
            { 
                return "Forms"; 
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the user has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the user name for the authenticated user.
        /// </summary>
        public abstract string Name { get; }       

        /// <summary>
        /// Gets the display name for the identity.
        /// </summary>
        //public string DisplayName { get; protected set; }

        /// <summary>
        /// Gets the user Id for the identity.
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the person Id for the identity.
        /// </summary>
        public long PersonId { get; protected set; }

        /// <summary>
        /// Gets the email address for the identity.
        /// </summary>
        public string EmailAddress { get; protected set; }

        /// <summary>
        /// Populates the properties for the identity instance.
        /// </summary>
        protected void PopulateIdentity(string name)
        {
            using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
            {
                var user = entityContext.aspnet_Users.FirstOrDefault(p => p.UserName == name);
                if (user != null)
                {
                    this.UserId = user.UserId;
                    var person = Person.GetPersonId(name);
                    this.PersonId = (person ?? -1 );
                    //this.DisplayName = Person.GetDisplayName(this.PersonId);
                    //this.EmailAddress = person.GetEmailAddress(EmailType.Personal);
                }
            }
        }
    }
}