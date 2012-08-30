using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    public class Conversation
    {
        public long Id { get; set; }
        public string Topic { get; set; }
        public ICollection<Blurb> Graph { get; set; }
        public DateTime LastUpdated { get; set; }
        public Person StartedBy { get; set; }
            
    }
}
