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
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Controllers
{
    public class ProfileController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Profile
        public IEnumerable<Person> GetProfiles(long userId)
        {
            return db.People.AsEnumerable();
        }

        // GET api/Profile/5 - get another user's profile
        public Person GetProfile(long userId, long id)
        {
            Person profile = db.People.Find(id);
            if (profile == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return profile;
        }

        // POST api/Profile - Existing user update
        public HttpResponseMessage PostProfile(
            [FromUri] long userId, 
            [FromBody] Person profile)
        {
            ByoBabiesUserPrincipal currentUser =
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

            var id = currentUser.GetUserId();
            //ensure the profile passed in is the current user's profile.
            //TODO - consider a more robust validation check here and some null assignment handling.
            if (ModelState.IsValid && id == profile.UserId && profile.Id == userId)
            {
                db.Entry(profile).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, profile);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        //// POST api/Profile - new user profile save
        //public HttpResponseMessage PostProfile(Person profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.People.Add(profile);
        //        db.SaveChanges();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, profile);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = profile.Id }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //}

        // DELETE api/Profile/5
        public HttpResponseMessage DeleteProfile(long userId)
        {
            Person profile = db.People.Find(userId);
            if (profile == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.People.Remove(profile);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, profile);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}