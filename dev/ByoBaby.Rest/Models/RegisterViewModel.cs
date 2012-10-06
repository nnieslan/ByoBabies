using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByoBaby.Rest.Models
{
    public class RegisterViewModel
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }
        
        public string Password { get; set; }

    }
}