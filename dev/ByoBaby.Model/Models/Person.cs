using System;
using System.Collections.Generic;
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

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Profile UserProfile { get; set; }

        [DataMember]
        public ICollection<Group> MemberOf { get; set; }

        [DataMember]
        public DateTime MemberSince { get; set; }

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
            var person = (from u in entityContext.Users
                          join p in entityContext.People
                          on u.UserId equals p.UserId into usermap
                          from p1 in usermap
                          where u.Username == userName
                          select p1).FirstOrDefault();

            if (person != null)
            {
                return person.Id;
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
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {

                var person = (from u in entityContext.Users
                              where u.Username == userName
                              select u).FirstOrDefault();

                if (person != null)
                {
                    return GetDisplayName(person);
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
                var person = (from u in entityContext.Users
                              join p in entityContext.People
                              on u.UserId equals p.UserId
                              where p.Id == personId
                              select u).FirstOrDefault();
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
        public static string GetDisplayName(User person)
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
                var person = (from u in entityContext.Users
                              join p in entityContext.People
                              on u.UserId equals p.UserId
                              where p.Id == personId
                              select u).FirstOrDefault();

                if (person != null)
                {
                    return person.Username;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the account locked out indicator for the specified person.
        /// </summary>
        /// <param name="personId">
        /// The Id of the person for which to get an the account locked out indicator.
        /// </param>
        /// <returns>
        /// The account locked out indicator for the person. <b>True</b> indicates locked out; <b>false</b> otherwise.
        /// </returns>
        public static bool GetIsLockedOut(long personId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var person = (from u in entityContext.Users
                              join p in entityContext.People
                              on u.UserId equals p.UserId
                              where p.Id == personId
                              select u).FirstOrDefault();
                if (person != null)
                {
                    return person.IsLockedOut;
                }
            }
            return false;
        }

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
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                var person = (from u in entityContext.Users
                              where u.Username == userName
                              select u).FirstOrDefault();
                if (person != null)
                {
                    return person.IsLockedOut;
                }
            }
            return false;
        }

    }
}
