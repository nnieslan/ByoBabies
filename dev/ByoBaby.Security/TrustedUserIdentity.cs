using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents a trusted user identity.
    /// </summary>
    internal class TrustedUserIdentity : UserIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrustedUserIdentity"/> class.
        /// </summary>
        internal TrustedUserIdentity()
        {
            //this.DisplayName = "Trusted";
        }

        /// <summary>
        /// Gets the name for the user.
        /// </summary>
        public override string Name
        {
            get { return "trusted"; }
        }
    }
}
