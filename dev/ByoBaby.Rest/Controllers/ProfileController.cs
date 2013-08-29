using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ByoBaby.Rest.Models;
using ByoBaby.Rest.Helpers;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Rest.Controllers
{
    public class ProfileController : ApiController
    {
        private ByoBabyRepository db = new ByoBabyRepository();

        // GET api/Profile
        [Authorize()]
        public IEnumerable<ProfileViewModel> GetProfiles(long userId)
        {
            throw new NotImplementedException();
            //var people = db.People.AsEnumerable();
        }

        // GET api/Profile/5 - get another user's profile
        [Authorize()]
        public ProfileViewModel GetProfile(long userId, long id)
        {
            Person profile = db.People
                .Include("Children")
                .Include("MemberOf")
                .Include("Friends")
                .FirstOrDefault(u => u.Id == id);
            if (profile == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return ProfileViewModel.FromPerson(profile);
        }

        // POST api/Profile/UploadProfilePicture - add a picture to your profile.
        [Authorize()]
        public Task<IEnumerable<string>> UploadProfilePicture()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string folderName = "uploads";
                string serverPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}", folderName));
                string rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);

                var streamProvider = new CustomMultipartFormDataStreamProvider(serverPath);
                var task = Request.Content
                    .ReadAsMultipartAsync(streamProvider)
                    .ContinueWith<IEnumerable<string>>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);

                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            return string.Format("{0}/{1}/{2}", rootUrl, folderName, i.LocalFileName);
                        });
                        //TODO - assign the new Url to the user's profile object and save it.
                        return fileInfo;
                    });

                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }
        }


        // POST api/Profile - Existing user update
        [Authorize()]
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
                Person existingProfile = db.People.Include("Children").FirstOrDefault(u => u.Id == userId);
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

            //TODO - come up with a better algo here, this is super inefficient, 
            //       but necessary for now as the iterative collection can't be modified during first loop.
            List<Child> toDelete = new List<Child>();
            foreach (var existing in existingProfile.Children)
            {
                if (!profile.Children.Any(c => c.Id == existing.Id))
                {
                    toDelete.Add(existing);
                }
            }
            foreach (var delete in toDelete)
            {
                existingProfile.Children.Remove(delete);
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
                    if (child.Age.HasValue && existingChild.Age != child.Age.Value)
                    {
                        existingChild.Age = child.Age.Value;
                    }
                    if (string.Compare(existingChild.Gender, child.Gender, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        existingChild.Gender = child.Gender;
                    }
                }
                else
                {
                    existingProfile.Children.Add(new Child() { ParentId = existingProfile.Id, Name = child.Name, Age = child.Age.Value, Gender = child.Gender });
                }
            }

            //TODO - home Phone, email, interests, groups
        }
    }
}