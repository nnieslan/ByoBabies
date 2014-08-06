(function() {


  if (!ByoBabies) ByoBabies = {};
  if (ByoBabies.API !== undefined) return;

  ByoBabies.ApiFactory = function(ajax) {
    console.log("ByoBabies.Api - creating Api with baseUrl : " + ajax.baseApiUrl);
    var self = $.extend(this, {
      ajaxFactory: ajax,
      ajax: function(options, useAuth) {
        return this.ajaxFactory.request(options, useAuth);
      },
      saveCredentials: function(creds) {
        var mode = 'ECB',
          cipherPwText = byteArrayToHex(
            rijndaelEncrypt(creds.pw, creds.user, mode));

        window.localStorage.setItem('credentials', JSON.stringify({
          user: creds.user,
          pw: cipherPwText
        }));
      },
      fetchCredentials: function() {
        var credsJson,
          mode = 'ECB',
          creds = window.localStorage.getItem('credentials');
        if (creds) {
          credsJson = JSON.parse(creds);
          var decryptedText = byteArrayToString(
            rijndaelDecrypt(hexToByteArray(credsJson.pw), credsJson.user,
              mode));
          credsJson.pw = decryptedText;
        }
        return credsJson;
      }
    });
    var constructPostAction = function(url, useAuth, successAction, publishTo) {
      return function(inputModel) {
        var unwrapped = ko.wrap.toJS(inputModel);
        console.log('POST - unwrapped model : ' + JSON.stringify(unwrapped));
        return self.ajax({
          url: url,
          type: 'POST',
          processData: true,
          data: unwrapped,
        }, useAuth).then(
          function(returnData) {
            var wrapped = {};
            if (returnData) {
              $.extend(wrapped, ko.wrap.fromJS(returnData));
            }
            console.log('ByoBabies.API - POST - ' + url + ' : ' + JSON.stringify(
              wrapped));
            if (successAction && $.isFunction(successAction)) {
              successAction(wrapped);
            }

            if (publishTo) {
              var msg = {
                src: publishTo.src,
                action: publishTo.action
              };
              console.log('notifying UI of POST for : ' + url)
              window.postMessage(msg, '*');
            }
          }
        );
      };
    },
      constructGetPromise = function(url, useAuth, successAction, publishTo) {
        return function() {
          console.log('constructGetPromise - url : ' + url);
          console.log('constructGetPromise - useAuth : ' + useAuth);

          return self.ajax({
            url: url,
            type: 'GET',
            error: function(jqxhr, exception) {
              console.log('ByoBabies.API - GET - ERROR - ' + url + ' : ' +
                JSON.stringify(jqxhr));
              if (exception) {
                console.log('ByoBabies.API - GET - ERROR - ' + url + ' : ' +
                  JSON.stringify(exception));
              }
              if (jqxhr.responseText && jqxhr.responseText !== '') {
                ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
              } else {
                ByoBabies.Utilities.notifyUser(
                  'Unable to perform the requested action at this time. Please try again later.',
                  'Error');
              }
            }
          }, useAuth).then(function(data) {
            console.log("ByoBabies.API - GET - " + url + ' : ' + JSON.stringify(
              data));
            if (successAction && $.isFunction(successAction)) {
              successAction(data);
            }
            if (data && publishTo) {
              var msg = {
                src: publishTo.src,
                action: publishTo.action
              };
              console.log('notifying UI of GET for : ' + url)
              window.postMessage(msg, '*');
            }
          });
        }
      };

    //build the API actions
    $.extend(self, {
      promiseLoginProviders: constructGetPromise(
        'account/externallogins?generateState=true',
        false,
        function(data) {
          if (data !== undefined) {
            window.localStorage.setItem('loginProviders', JSON.stringify(
              data));
          }
        }, {
          src: 'logon',
          action: 'providersloaded'
        }),
      registerUser: constructPostAction(
        'account/registerexternal',
        true,
        null, {
          src: 'logon',
          action: 'registered'
        }),
      login: constructPostAction(
        'account/login',
        false,
        function(data) {
          self.saveCredentials(creds);
          self.getProfile();
        }, {
          src: 'logon',
          action: 'loggedin'
        }),
      logout: constructPostAction(
        'account/logout',
        false,
        null,
        null),
      promiseProfile: constructGetPromise(
        'account/get',
        true,
        function(data) {
          window.localStorage.setItem("profile", JSON.stringify(data));
        }, {
          src: 'profile',
          action: 'loaded'
        }),
      promiseUserInfo: constructGetPromise(
        'account/userinfo',
        true,
        function(data) {
          if (data !== undefined && data.HasRegistered != true) {
            console.log('User has not registered');
            var input = {
              //'UserName': ko.observable(data.UserName),
              'DisplayName': ko.observable(data.UserName)
            };
            self.registerUser(input);
          } else if (data) {
            var msg = {
              src: 'logon',
              action: 'loggedin'
            };
            console.log('notifying UI of login status.')
            window.postMessage(msg, '*');
          }
        }, null)
    });

    return self;
  };

})();
