using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Person> Members { get; set; }

    }
}
