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
    public class ProfileController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Profile
        public IEnumerable<ProfileViewModel> GetProfiles(long userId)
        {
            throw new NotImplementedException();
            //var people = db.People.AsEnumerable();
        }

        // GET api/Profile/5 - get another user's profile
        public ProfileViewModel GetProfile(long userId, long id)
        {
            Person profile = db.People.Find(id);
            if (profile == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return ProfileViewModel.FromPerson(profile);
        }

        // POST api/Profile - Existing user update
        public HttpResponseMessage PostProfile(
            [FromUri] long userId, 
            [FromBody] ProfileViewModel profile)
        {
            ByoBabiesUserPrincipal currentUser =
                    HttpContext.Current.User as ByoBabiesUserPrincipal;

            var id = currentUser.GetUserId();
            //ensure the profile passed in is the current user's profile.
            //TODO - consider a more robust validation check here and some null assignment handling.
            if (ModelState.IsValid 
                && profile.Id == userId 
                && userId == currentUser.GetPersonId().Value)
            {
                Person existingProfile = db.People.Find(userId);
                if (existingProfile == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                MapProfileToPerson(profile, existingProfile);

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

        private static void MapProfileToPerson (ProfileViewModel profile, Person existingProfile)
        {
            if (string.Compare(existingProfile.LastName, profile.LastName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.LastName = profile.LastName;
            }
            if (string.Compare(existingProfile.FirstName, profile.FirstName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.FirstName = profile.FirstName;
            }
            if (string.Compare(existingProfile.City, profile.City, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.City = profile.City;
            }
            if (string.Compare(existingProfile.State, profile.State, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.State = profile.State;
            }
            if (string.Compare(existingProfile.Neighborhood, profile.Neighborhood, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.Neighborhood = profile.Neighborhood;
            }
            if (string.Compare(existingProfile.MobilePhone, profile.MobilePhone, StringComparison.OrdinalIgnoreCase) != 0)
            {
                existingProfile.MobilePhone = profile.MobilePhone;
            }

            foreach (var child in profile.Children)
            {
                if (child.Id.HasValue)
                {
                    var existingChild = existingProfile.Children.First(c => c.Id == child.Id.Value);
                    if (string.Compare(existingChild.Name, child.Name, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        existingChild.Name = child.Name;
                    }
                    if (existingChild.Age != child.Age)
                    {
                        existingChild.Age = child.Age;
                    }
                    if (string.Compare(existingChild.Gender, child.Gender, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        existingChild.Gender = child.Gender;
                    }
                }
                else
                {
                    existingProfile.Children.Add(new Child() { ParentId = existingProfile.Id, Name = child.Name, Age = child.Age, Gender = child.Gender });
                }
            }

            //TODO - home Phone, email, interests, groups
        }
    }
}