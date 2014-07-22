 (function() {

   if (!window.ByoBabies) window.ByoBabies = {};
   if (ByoBabies.Ajax !== undefined) return;

   ByoBabies.Ajax = function(baseUrl, errorHandler) {

     console.log("ByoBabies.Ajax - Initializing an Ajax instance with baseUrl : " + baseUrl);

     $.extend(this, {
       baseApiUrl: baseUrl || '',
       addAuthHeader: function(xhr) {
         var authData = JSON.parse(window.localStorage.getItem(
           'access_token'));
         if (authData && authData !== null) {
           xhr.setRequestHeader("Authorization", "Bearer " + authData[
             'access_token']);
         }
       },
       defaultAjaxOptions: {
         cancelGlobalError: false,
         processData: false,
         dataType: 'json',
         contentType: 'application/json',
         crossDomain: true,
         withCredentials: true,
         xhrFields: {
           withCredentials: true
         },
         error: errorHandler || $.noop()
       }
     });
   };

   ByoBabies.Ajax.prototype.request = function(options, useAuth) {
     if (options.url && options.url.indexOf('://') === -1) {
       options.url = this.baseApiUrl + options.url;
     }
     if (useAuth) {
       options.beforeSend = this.addAuthHeader;
     }

     return $.ajax($.extend({}, this.defaultAjaxOptions, options));
   };

 })();
