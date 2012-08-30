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
    public class ConversationController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Conversation
        public IQueryable<Conversation> GetConversations()
        {
            return db.Conversations;
        }

        // GET api/Conversation/5
        public Conversation GetConversation(long id)
        {
            Conversation conversation = db.Conversations.Find(id);
            if (conversation == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return conversation;
        }

        // PUT api/Conversation/5
        public HttpResponseMessage PutConversation(long id, Conversation conversation)
        {
            if (ModelState.IsValid && id == conversation.Id)
            {
                db.Entry(conversation).State = EntityState.Modified;

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

        // POST api/Conversation
        public HttpResponseMessage PostConversation(Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                db.Conversations.Add(conversation);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, conversation);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = conversation.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Conversation/5
        public HttpResponseMessage DeleteConversation(long id)
        {
            Conversation conversation = db.Conversations.Find(id);
            if (conversation == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Conversations.Remove(conversation);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, conversation);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}