using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    public class Person
    {
        public long Id { get; set; }    
        public Profile UserProfile { get; set; }
        public ICollection<Group> MemberOf { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
