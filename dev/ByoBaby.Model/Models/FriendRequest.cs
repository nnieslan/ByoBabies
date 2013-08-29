using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{

    public class FriendRequest : Request
    {
        public long RequestorId { get; set; }
        
        protected override void HandleAccept()
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var requestor = entityContext.People.Find(this.RequestorId);
                var acceptor = entityContext.People.Find(this.TargetId);
                requestor.Friends.Add(acceptor);
                acceptor.Friends.Add(requestor);

                entityContext.Requests.Remove(this);

                var associatedNotifications = entityContext.Notifications
                    .Where(n => n.Originator.Id == this.Id).ToList();

                foreach (var notification in associatedNotifications)
                {
                    entityContext.Notifications.Remove(notification);
                }

                entityContext.SaveChanges();
            }
        }

        protected override void HandleDeny()
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                entityContext.Requests.Remove(this);
                
                var associatedNotifications = entityContext.Notifications
                    .Where(n => n.Originator.Id == this.Id).ToList();

                foreach (var notification in associatedNotifications)
                {
                    entityContext.Notifications.Remove(notification);
                }

                entityContext.SaveChanges();
            }
        }
    }
}
