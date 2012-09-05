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
                return "Custom"; 
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
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                User person = entityContext.Users.FirstOrDefault(p => p.Username == name);
                if (person != null)
                {
                    this.UserId = person.UserId;
                    //this.PersonId = person.Id;
                    //this.DisplayName = person.GetDisplayName();
                    //this.EmailAddress = person.GetEmailAddress(EmailType.Personal);
                }
            }
        }
    }
}