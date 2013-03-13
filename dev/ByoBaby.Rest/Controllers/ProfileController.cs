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

namespace ByoBaby.Rest.Controllers
{
    public class ProfileController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Profile
        public IEnumerable<Person> GetProfiles()
        {
            return db.People.AsEnumerable();
        }

        // GET api/Profile/5
        public Person GetProfile(long id)
        {
            Person profile = db.People.Find(id);
            if (profile == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return profile;
        }

        // PUT api/Profile/5
        public HttpResponseMessage PutProfile(long id, Person profile)
        {
            if (ModelState.IsValid && id == profile.Id)
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

        // POST api/Profile
        public HttpResponseMessage PostProfile(Person profile)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(profile);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, profile);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = profile.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Profile/5
        public HttpResponseMessage DeleteProfile(long id)
        {
            Person profile = db.People.Find(id);
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