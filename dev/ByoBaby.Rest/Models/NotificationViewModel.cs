using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Rest.Models
{
    [DataContract]
    public class NotificationViewModel
    {
        [Required]
        [DataMember(IsRequired = true)]
        public long Id { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public long OriginatorId { get; set; }

        [DataMember()]
        public string Title { get; set; }

        [DataMember()]
        public string Body { get; set; }

        [DataMember()]
        public string OriginatorType { get; set; }

        public static NotificationViewModel FromNotification(Notification source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var vm = new NotificationViewModel()
            {
                Id = source.Id,
                Title = source.Originator.Title,
                Body = source.Originator.Description,
                OriginatorId = source.Originator.Id,
                OriginatorType = source.Originator.GetType().Name
            };

            return vm;
        }
    }
}