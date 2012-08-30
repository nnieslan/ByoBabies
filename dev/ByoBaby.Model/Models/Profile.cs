using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    public class Profile
    {

        public long Id { get; set; }

        public long PersonId { get; set; }

        public string Email { get; set; }

        public string MobilePhone { get; set; }

        public string HomePhone { get; set; }

        public string City { get; set; }

        public string Neighborhood { get; set; }
        
        public ICollection<string> Interests { get; set; }

        public ICollection<Child> Children { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
