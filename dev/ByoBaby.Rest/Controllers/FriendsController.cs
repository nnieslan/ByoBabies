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
    public class FriendsController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/{userId}/Friends
        [Authorize()]
        public IEnumerable<ProfileViewModel> GetFriends(long userId)
        {
            Person existingProfile = db.People.Include("Friends").FirstOrDefault(u => u.Id == userId);
            return existingProfile.Friends.Select(f => ProfileViewModel.FromPerson(f));
        }

        public HttpResponseMessage PostNewFriend(long userId, long friendId, string message)
        {
            ByoBabiesUserPrincipal currentUser =
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

            var id = currentUser.GetUserId();
            if (userId == currentUser.GetPersonId().Value)
            {
                Person existingProfile = this.db.People
                    .Include(p => p.Friends.Select(f => f.Id))
                    .FirstOrDefault(u => u.Id == userId);
                
                if (existingProfile == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                
                Person friend = db.People.Find(friendId);
                if (friend == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                try
                {
                    var request = new FriendRequest()
                    {
                        Audience = friend,
                        Requestor = existingProfile,
                        Title = "Can we be friends?",
                        Description = message
                    };
                    friend.PendingRequests.Add(request);
                    this.db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}
