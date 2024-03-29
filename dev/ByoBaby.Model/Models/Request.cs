﻿using System;
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
    public abstract class Request : NotificationOriginator
    {
        public Person Requestor { get; set; }

        public Person Audience { get; set; }

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
