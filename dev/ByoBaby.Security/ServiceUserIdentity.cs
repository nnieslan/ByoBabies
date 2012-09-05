using System;
using System.Security.Principal;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a custom WCF identity for the FlowPay web services.
    /// </summary>
    public class ServiceUserIdentity : UserIdentity
    {
        public const string AnonymousUserId = "Anonymous";

        /// <summary>
        /// The <see cref="GenericIdentity"/> of the authenticated WCF user.
        /// </summary>
        private GenericIdentity wcfIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUserIdentity"/> class.
        /// </summary>
        /// <param name="serviceIdentity">
        /// The <see cref="GenericIdentity"/> from the service.
        /// </param>
        public ServiceUserIdentity(GenericIdentity serviceIdentity)
            : base()
        {
            if (serviceIdentity == null)
            {
                throw new ArgumentNullException("serviceIdentity");
            }

            this.wcfIdentity = serviceIdentity;

            this.PopulateIdentity(this.wcfIdentity.Name);
        }

        /// <summary>
        /// Gets the user name for the authenticated user.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.wcfIdentity.Name;
            }
        }

    }
}
