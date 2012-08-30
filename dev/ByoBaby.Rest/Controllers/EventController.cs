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
    public class EventController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Event
        public IEnumerable<Event> GetEvents()
        {
            return db.Events.AsEnumerable();
        }

        // GET api/Event/5
        public Event GetEvent(long id)
        {
            Event evnt = db.Events.Find(id);
            if (evnt == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return evnt;
        }

        // PUT api/Event/5
        public HttpResponseMessage PutEvent(long id, Event evnt)
        {
            if (ModelState.IsValid && id == evnt.Id)
            {
                db.Entry(evnt).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
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

        // POST api/Event
        public HttpResponseMessage PostEvent(Event evnt)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(evnt);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, evnt);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = evnt.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Event/5
        public HttpResponseMessage DeleteEvent(long id)
        {
            Event evnt = db.Events.Find(id);
            if (evnt == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Events.Remove(evnt);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, evnt);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}