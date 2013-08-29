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
    public class RequestsController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        [Authorize()]
        public IEnumerable<RequestViewModel> Get()
        {
            ByoBabiesUserPrincipal currentUser =
            HttpContext.Current.User as ByoBabiesUserPrincipal;

            var personId = currentUser.GetPersonId();

            Person existingProfile = db.People
                .Include("PendingRequests")
                .FirstOrDefault(u => u.Id == personId.Value);

            return existingProfile.PendingRequests.Select(s => RequestViewModel.FromRequest(s));
        }
        
        [Authorize()]
        public RequestViewModel Get(long id)
        {
            ByoBabiesUserPrincipal currentUser =
            HttpContext.Current.User as ByoBabiesUserPrincipal;

            var personId = currentUser.GetPersonId();

            Person existingProfile = db.People
                .Include("PendingRequests")
                .FirstOrDefault(u => u.Id == personId.Value);

            return RequestViewModel.FromRequest(
                existingProfile.PendingRequests.FirstOrDefault(s => s.Id == id));
        }

        [Authorize()]
        public HttpResponseMessage PostReply(long id, string action)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            ByoBabiesUserPrincipal currentUser =
            HttpContext.Current.User as ByoBabiesUserPrincipal;

            var personId = currentUser.GetPersonId();

            Person existingProfile = db.People
                .Include("PendingRequests")
                .FirstOrDefault(u => u.Id == personId.Value);

            var request = existingProfile.PendingRequests.FirstOrDefault(s => s.Id == id);

            if (request != null)
            {
                if (string.Compare(action, "accept", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    request.Accept();
                }
                else if (string.Compare(action, "deny", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    request.Deny();
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.ReasonPhrase = string.Format("The specified action '{0}' is invalid.", action);
                }
            }

            return response;
        }
    }
}