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
    public class BlurbController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Blurb
        public IEnumerable<Blurb> GetBlurbs(long conversationId)
        {
            return db.Blurbs.Where(b => b.ConversationId == conversationId).OrderByDescending(b=> b.Id);
        }

        // GET api/conversation/1/Blurb/5
        public Blurb GetBlurb(long conversationId, long id)
        {
            Blurb blurb = db.Blurbs.FirstOrDefault(b => b.ConversationId == conversationId && b.Id == id);
            if (blurb == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return blurb;
        }

        // PUT api/conversation/1/Blurb/5
        public HttpResponseMessage PutBlurb(long conversationId, long id, Blurb blurb)
        {
            if (ModelState.IsValid && id == blurb.Id)
            {
                db.Entry(blurb).State = EntityState.Modified;

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

        // POST api/conversation/1/Blurb
        public HttpResponseMessage PostBlurb(long conversationId, Blurb blurb)
        {
            if (ModelState.IsValid)
            {
                blurb.ConversationId = conversationId;
                db.Blurbs.Add(blurb);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, blurb);
                response.Headers.Location = new Uri(Url.Link("ConversationsApi", new { conversationId = conversationId, id = blurb.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/conversation/1/blurb/5
        public HttpResponseMessage DeleteBlurb(long conversationId, long id)
        {
            Blurb blurb = db.Blurbs.FirstOrDefault(b => b.ConversationId == conversationId && b.Id == id);
            if (blurb == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Blurbs.Remove(blurb);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, blurb);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}