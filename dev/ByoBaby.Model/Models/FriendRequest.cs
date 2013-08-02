using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{

    public class FriendRequest : Notification
    {
        public long RequestorId { get; set; }
        
        protected override void HandleAccept()
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var requestor = entityContext.People.Find(this.RequestorId);
                requestor.Friends.Add(entityContext.People.Find(this.TargetId));

                entityContext.Notifications.Remove(this);

                entityContext.SaveChanges();
            }
        }

        protected override void HandleDeny()
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                entityContext.Notifications.Remove(this);
                entityContext.SaveChanges();
            }
        }
    }
}
