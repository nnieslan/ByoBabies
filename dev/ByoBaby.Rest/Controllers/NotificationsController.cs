using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ByoBaby.Rest.Models;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Controllers
{
    public class NotificationsController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();


        // GET api/notifications
        [Authorize()]
        public IEnumerable<NotificationViewModel> GetNotifications()
        {
            ByoBabiesUserPrincipal currentUser =
                HttpContext.Current.User as ByoBabiesUserPrincipal;

            var id = currentUser.GetPersonId();

            Person existingProfile = db.People
                .Include("Notifications")
                .Include("Notifications.Originator")
                .FirstOrDefault(u => u.Id == id.Value);

            return existingProfile.Notifications
                .Select(s => NotificationViewModel.FromNotification(s));
        }

    }
}
