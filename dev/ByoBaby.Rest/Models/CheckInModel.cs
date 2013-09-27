using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using ByoBaby.Model;

namespace ByoBaby.Rest.Models
{
    [DataContract]
    public class NearByCheckInModel
    {
        [DataMember(IsRequired = true)]
        public CheckInModel Checkin { get; set; }
        
        [DataMember(IsRequired = true)]
        public double Distance { get; set; }
    }

    [DataContract]
    public class CheckInModel
    {
        [DataMember(IsRequired = true)]
        public LocationModel Location { get; set; }

        [DataMember(IsRequired = true)]
        public int Duration { get; set; }

        [DataMember(IsRequired = true)]
        public string Note { get; set; }

        [DataMember]
        public string DisplayStartTime { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public int EstimatedTimeRemaining { get; set; }

        [DataMember]
        public ProfileViewModel Owner { get; set; }

        public static CheckInModel FromCheckIn(CheckIn ci)
        {
            var remaining = ci.Duration - (DateTime.Now.Subtract(ci.StartTime).Minutes);
            return new CheckInModel()
            {
                Location = new LocationModel() { YelpId = ci.LocationId, Latitude = ci.Latitude, Longitude = ci.Longitude },
                Note = ci.Note,
                Duration = ci.Duration,
                StartTime = ci.StartTime,
                DisplayStartTime = ci.StartTime.ToShortTimeString(),
                EstimatedTimeRemaining = remaining,
                Owner = ProfileViewModel.FromPerson(ci.Owner)
            };
        }
    }
}