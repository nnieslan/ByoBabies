 (function() {

   if (!window.ByoBabies) window.ByoBabies = {};
   if (ByoBabies.Ajax !== undefined) return;

   ByoBabies.Ajax = function(baseUrl, errorHandler) {

     console.log(
       "ByoBabies.Ajax - Initializing an Ajax instance with baseUrl : " +
       baseUrl);

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
         crossDomain: true,
         cache: false,
         withCredentials: true,
         error: errorHandler || $.noop(),
         statusCode: {
           '404': function() {
             console.log('404 occured');
           },
           '401': function() {
             console.log('401 occured');
           },
           '500': function() {
             console.log('500 occured');
           }
         }
       }
     });
   };

   ByoBabies.Ajax.prototype.request = function(options, useAuth) {
     if (options.url && options.url.indexOf('://') === -1) {
       options.url = this.baseApiUrl + options.url;
     }
     if (useAuth) {
       options.beforeSend = this.addAuthHeader;
     } else {
       options.withCredentials = null;
       options.xhrFields = null;
     }
     console.log('ByoBabies.Ajax - url : ' + options.url);

     var optionsResolved = $.extend({}, this.defaultAjaxOptions, options);
     console.log('ByoBabies.Ajax - options : ' + JSON.stringify(
       optionsResolved));
     return $.ajax(optionsResolved);
   };

 })();
