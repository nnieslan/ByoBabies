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
    public abstract class Notification : INotification
    {
        public long Id { get; set; }

        public long TargetId { get; set; }

        public string TargetIdType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        protected abstract void HandleAccept();

        protected abstract void HandleDeny();

        /// <summary>
        /// Performs the Accept action on a request by delegating to the implementor.
        /// </summary>
        public void Accept()
        {
            this.HandleAccept();
        }

        /// <summary>
        /// Performs the Deny action on a request by delegating to the implementor.
        /// </summary>
        public void Deny()
        {
            this.HandleDeny();
        }

    }
}
