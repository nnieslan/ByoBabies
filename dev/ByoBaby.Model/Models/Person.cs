using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{
    [DataContract]
    public class Person
    {
        [DataMember]
        [Key]
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public string ProfilePictureUrl { get; set; }

        public DateTime MemberSince { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobilePhone { get; set; }

        public string HomePhone { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Neighborhood { get; set; }

        public ICollection<string> Interests { get; set; }

        public ICollection<Child> Children { get; set; }

        public ICollection<Group> MemberOf { get; set; }

        public ICollection<Person> Friends { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<Request> PendingRequests { get; set; }
        
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets the unique Id for the person associated with the specified user name.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> to use for database interactions.
        /// </param>
        /// <param name="userName">
        /// The user name for which to get a person Id.
        /// </param>
        /// <returns>
        /// A <see cref="long?"/> containing the person Id or <b>null</b> if no
        /// person Id is found for the user name.
        /// </returns>
        public static long? GetPersonId(ByoBabyRepository entityContext, string userName)
        {

            using(aspnet_fbaEntities1 authContext = new aspnet_fbaEntities1()) {
                 Guid? userId = (from u in authContext.aspnet_Users 
                                 where u.UserName == userName 
                                 select u.UserId).FirstOrDefault();   
            
                if(userId.HasValue)
                {
                    var person = (from p in entityContext.People
                                  where p.UserId == userId.Value
                                  select p).FirstOrDefault();

                    if (person != null)
                    {
                        return person.Id;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the unique Id for the person associated with the specified user name.
        /// </summary>
        /// <param name="userName">
        /// The user name for which to get a person Id.
        /// </param>
        /// <returns>
        /// A <see cref="long?"/> containing the person Id or <b>null</b> if no
        /// person Id is found for the user name.
        /// </returns>
        public static long? GetPersonId(string userName)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                return GetPersonId(entityContext, userName);
            }
        }


        /// <summary>
        /// Gets the display name associated with the specified user name.
        /// </summary>
        /// <param name="userName">
        /// The user name for which to get the display name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing a person's display name.
        /// </returns>
        public static string GetDisplayName(string userName)
        {
            using (aspnet_fbaEntities1 authContext = new aspnet_fbaEntities1())
            {

                var user = (from u in authContext.aspnet_Users
                              where u.UserName == userName
                              select u).FirstOrDefault();

                if (user != null)
                {

                    using (ByoBabyRepository entityContext = new ByoBabyRepository())
                    {
                        var person = (from p in entityContext.People where p.UserId == user.UserId select p).FirstOrDefault();
                        if (person != null)
                        {
                            return GetDisplayName(person);
                        }
                    }
                }
            }

            return userName;
        }

        /// <summary>
        /// Gets the display name associated with the specified person Id.
        /// </summary>
        /// <param name="personId">
        /// The Id for the person for which to get the display name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing a person's display name or
        /// an empty string if no person is found.
        /// </returns>
        public static string GetDisplayName(long personId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var person = (from p in entityContext.People
                              where p.Id == personId
                              select p).FirstOrDefault();
                if (person != null)
                {
                    return GetDisplayName(person);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the display name for the specified person.
        /// </summary>
        /// <param name="person">
        /// The person for which to get a display name.
        /// </param>
        /// <returns>
        /// The display name for the specified person.
        /// </returns>
        public static string GetDisplayName(Person person)
        {
            if (person != null)
            {
                return GetDisplayName(person.FirstName, person.LastName);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the display name for the specified first and last names.
        /// </summary>
        /// <param name="firstName">
        /// The first name for the person.
        /// </param>
        /// <param name="lastName">
        /// The last name for the person.
        /// </param>
        /// <returns>
        /// The display name for the person.
        /// </returns>
        public static string GetDisplayName(string firstName, string lastName)
        {
            if (firstName == null && lastName == null)
            {
                return string.Empty;
            }

            if (firstName == null)
            {
                return lastName;
            }

            if (lastName == null)
            {
                return firstName;
            }

            return String.Format("{0} {1}", firstName, lastName);
        }

        /// <summary>
        /// Gets the user name associated with the specified person Id.
        /// </summary>
        /// <param name="personId">
        /// The Id for the person for which to get the user name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing a person's user name or
        /// an empty string if no person is found.
        /// </returns>
        public static string GetUserName(long personId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var person = (from p in entityContext.People
                              where p.Id == personId
                              select p).FirstOrDefault();

                if (person != null)
                {
                    using (aspnet_fbaEntities1 authContext = new aspnet_fbaEntities1())
                    {
                        var user = authContext.aspnet_Users.FirstOrDefault(u => u.UserId == person.UserId);
                        if (user != null)
                        {
                            return user.UserName;
                        }
                    }
                }
            }

            return string.Empty;
        }

        ///// <summary>
        ///// Gets the account locked out indicator for the specified person.
        ///// </summary>
        ///// <param name="personId">
        ///// The Id of the person for which to get an the account locked out indicator.
        ///// </param>
        ///// <returns>
        ///// The account locked out indicator for the person. <b>True</b> indicates locked out; <b>false</b> otherwise.
        ///// </returns>
        //public static bool GetIsLockedOut(long personId)
        //{
        //    using (ByoBabyRepository entityContext = new ByoBabyRepository())
        //    {
        //        var person = (from u in entityContext.Users
        //                      join p in entityContext.People
        //                      on u.UserId equals p.UserId
        //                      where p.Id == personId
        //                      select u).FirstOrDefault();
        //        if (person != null)
        //        {
        //            return person.IsLockedOut;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Gets the account locked out indicator for the specified person.
        /// </summary>
        /// <param name="userName">
        /// The user name of the person for which to get an the account locked out indicator.
        /// </param>
        /// <returns>
        /// The account locked out indicator for the person. <b>True</b> indicates locked out; <b>false</b> otherwise.
        /// </returns>
        public static bool GetIsLockedOut(string userName)
        {
            using (aspnet_fbaEntities1 entityContext = new aspnet_fbaEntities1())
            {
                var person = (from u in entityContext.aspnet_Users
                              where u.UserName == userName
                              select u).FirstOrDefault();
                if (person != null)
                {
                    return (person.aspnet_Membership != null ? person.aspnet_Membership.IsLockedOut : false);
                }
            }
            return false;
        }

    }
}
