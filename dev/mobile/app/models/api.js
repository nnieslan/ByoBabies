(function($) {


  if (!ByoBabies) ByoBabies = {};
  if (ByoBabies.API !== undefined) return;

  var api = function(ajax) {

    var self = $.extend(this, {
      ajaxFactory: ajax,
      ajax: function(options, useAuth) {
        return this.ajaxFactory.request(option, useAuth);
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
      return function(data) {
        var unwrapped = ko.wrap.toJS(data);
        return self.ajax({
          url: url,
          type: 'POST',
          data: JSON.stringify(unwrapped)
        }, useAuth).then(
          function(data) {
            //TODO -mapping of data
            console.log('ByoBabies.API - POST - ' + url + ' : ' + JSON.stringify(
              data));
            if (successAction && $.isFunction(successAction)) {
              successAction(data);
            }
            if (data && publishTo) {
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
      constructGetPromise = function(url, successAction, publishTo) {
        return function() {
          self.ajax({
            url: url,
            type: 'GET',
            error: function(jqxHR, exception) {
              if (jqxHR.responseText && jqxHR.responseText !== '') {
                utilities.notifyUser(jqxHR.responseText, 'Error');
              } else {
                utilities.notifyUser(
                  'Unable to get your registration status. Please try again later.',
                  'Error');
              }
            }
          }).then(function(data) {
            console.log("ByoBabies.API - GET - " + url + ' : ' + JSON.stringify(
              data));
            if (successAction && $.isFunction(successAction)) {
              successAction(data);
            }
            if (data) {
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
      registerUser: constructPostAction('account/registerexternal', true,
        null, {
          src: 'logon',
          action: 'registered'
        }),
      login: constructPostAction('account/login', false, function(data) {
        self.saveCredentials(creds);
        self.getProfile();
      }, {
        src: 'logon',
        action: 'loggedin'
      }),
      logout: constructPostAction('account/logout', false, null, null),
      promiseProfile: constructGetPromise('account/get', true, function(
        data) {
        window.localStorage.setItem("profile", JSON.stringify(data));
      }, {
        src: 'profile',
        action: 'loaded'
      }),
      promiseUserInfo: constructGetPromise('account/userinfo', true,
        function(
          data) {
          if (data !== undefined && data.HasRegistered != true) {
            console.log('User has not registered');
            self.registerUser(data.UserName);
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
  };

  ByoBabies.ApiFactory = api;
})($);
