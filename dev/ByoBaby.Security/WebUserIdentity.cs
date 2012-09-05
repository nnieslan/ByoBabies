using System;
using System.Web.Security;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a custom forms identity for the FlowPay web application.
    /// </summary>
    public class WebUserIdentity : UserIdentity
    {
        /// <summary>
        /// The <see cref="FormsAuthenticationTicket"/> for the identity.
        /// </summary>
        private FormsAuthenticationTicket ticket;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowPayUserIdentity"/> class.
        /// </summary>
        /// <param name="ticket">
        /// The <see cref="FormsAuthenticationTicket"/> for the identity.
        /// </param>
        public WebUserIdentity(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }

            this.ticket = ticket;

            this.PopulateIdentity(this.ticket.Name);
        }

        /// <summary>
        /// Gets the user name for the authenticated user.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.ticket.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="FormsAuthenticationTicket"/> for the identity.
        /// </summary>
        public FormsAuthenticationTicket Ticket
        {
            get
            {
                return this.ticket;
            }
        }
    }
}
