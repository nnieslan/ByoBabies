using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByoBaby.Model
{
    public class Blurb
    {
        public long ConversationId { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }
        public Person WrittenBy { get; set; }
        public DateTime WrittenOn { get; set; }
    }
}
