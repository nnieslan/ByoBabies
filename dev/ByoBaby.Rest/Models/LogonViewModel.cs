using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByoBaby.Rest.Models
{

    /// <summary>
    /// Represents a view model to support logon requests.
    /// </summary>
    public class LogOnModel
    {
        /// <summary>
        /// Gets or sets the user name for the logon attempt.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password for the logon attempt.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the remember me status for the attempt.
        /// </summary>
        public string RememberMe { get; set; }
    }
}
