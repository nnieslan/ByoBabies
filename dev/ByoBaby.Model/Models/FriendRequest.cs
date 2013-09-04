using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{

    public class FriendRequest : Request
    {

        protected override void HandleAccept()
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                entityContext.People.Attach(this.Requestor);
                entityContext.People.Attach(this.Audience);
                entityContext.Requests.Attach(this);

                if (this.Requestor.Friends == null)
                {
                    this.Requestor.Friends = new Collection<Person>() { this.Audience };
                }
                else
                {
                    this.Requestor.Friends.Add(this.Audience);
                }
                if (this.Audience.Friends == null)
                {
                    this.Audience.Friends = new Collection<Person>() { this.Requestor };
                }
                else
                {
                    this.Audience.Friends.Add(this.Requestor);
                }

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
