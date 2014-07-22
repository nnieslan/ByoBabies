function BackgroundViewModel() {

  console.log("Initailizing a BackgroundViewModel instance");

  //NavViewModel.apply(this);

  var self = this;
  self.authView;
  self.authTimer;
  self.preloadedViews = [];
  self.user = ko.observable();

  self.initialize = function() {
    var leftDrawerWebView = new steroids.views.WebView({
      location: "/views/tasks/index.html",
      id: "leftDrawerView"
    });
    var logonWebView = new steroids.views.WebView({
      location: "/views/logon/index.html",
      id: "logonView"
    });
    var registerWebView = new steroids.views.WebView({
      location: "/views/register/index.html",
      id: "registerView"
    });

    if (self.baseUrl === undefined || self.baseUrl === null) {
      self.baseUrl = "http://dev.byobabies.com:8080/"
      window.localStorage.setItem("svcUrl", self.baseUrl);
    }
    logonWebView.preload({}, {
      onSuccess: function() {
        self.getLoginProviders();
      }
    });
    registerWebView.preload();
    leftDrawerWebView.preload({}, {
      onSuccess: function() {
        steroids.drawers.enableGesture(leftDrawerWebView);
      }
    });
    self.preloadedViews.push(logonWebView);
    self.preloadedViews.push(registerWebView);
    self.preloadedViews.push(leftDrawerWebView);

    window.addEventListener("message", self.handleMessage);
  };

  self.handleMessage = function(msg) {
    console.log('BackgroundViewModel - Message received from ' + msg.data.src);
    var source = msg.data.src || '';
    if (source === 'drawer') {
      console.log("message received from drawer.");
      self.handleNavigation(msg);
    } else if (source === 'logon') {
      self.handleAuthentication(msg);
    }
  };

  self.handleAuthentication = function(msg) {
    var action = msg.data.action || '';
    if (action === 'login') {
      if (msg.data.credentials !== undefined) {
        self.login(msg.data.credentials);
      }
    } else if (msg.data.action === 'externallogin') {
      self.loginExternalComplete(msg.data.data);
    } else if (msg.data.action === 'logout') {
      self.logout();
    }
  };

  self.handleNavigation = function(selection) {
    var loop, currentView, found = false;
    for (loop = 0; loop < self.preloadedViews.length; ++loop) {
      currentView = self.preloadedViews[loop];
      if (currentView.id === selection.id) {
        console.log("found preloaded view : " + selection.id);
        found = true;
        steroids.layers.replace(currentView);
      }
    }
    if (!found) {
      console.log(
        "BackgroundViewModel - unable to find a previously preloaded view, creating one - " +
        selection.id);
      currentView = new steroids.views.WebView({
        location: selection.url,
        id: selection.id
      });
      currentView.preload({}, {
        onSuccess: function() {
          self.preloadedViews.push(currentView);
          steroids.layers.replace(currentView);
        }
      })
    }
  }

  self.getLoginProviders = function() {
    console.log(
      "BackgroundViewModel - fetching list of supported login providers");
    var url = self.baseUrl + 'api/account/externallogins?generateState=true';
    var jqxhr = $.ajax({
      url: url,
      type: 'GET',
      cache: false,
      crossDomain: true,
      success: function(data) {
        console.log(
          "BackgroundViewModel - fetched list of supported login providers - " +
          JSON.stringify(data));
        if (data !== undefined) {
          window.localStorage.setItem('loginProviders', JSON.stringify(data));
          var msg = {
            src: 'logon',
            action: 'providersloaded'
          };
          console.log('notifying UI of logon providers.')
          window.postMessage(msg, '*');
        }
      },
      error: function(jqxHR, exception) {
        if (jqxHR.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
        } else {
        ByoBabies.Utilities.notifyUser(
            'Unable to get the list of providers.  Please try again later.',
            'Error');
        }
      }
    });

  };

  self.loginExternalComplete = function(data) {
    console.log(
      'BackgroundViewModel - storing token data from external login - ' + data.provider
    );
    window.localStorage.setItem('access_token', JSON.stringify(data));
    self.getUserInfo();
    //TODO Eval registration status
    // self.getProfile();

    // self.authView = new steroids.views.WebView({
    //  location: authUrl,
    //  id: "authView-" + provider.Name
    // });

    //if (self.authView !== undefined) {
    //  self.authView.unload();
    //}
    //self.authView.preload({}, {
    //  onSuccess: function() {
    //    console.log(
    //      'BackgroundViewModel - Successfully preloaded external auth view - ' +
    //      provider.Name);
    //    console.log(self.authView.location);
    //    steroids.layers.push(self.authView);
    //    self.authTimer = setInterval(self.checkModal, 1000);
    //    //timeout the wait for 3rd party auth at 2 min for now
    //    setTimeout(self.terminateModal, 24000);
    //  },
    //  onFailure: function(error) {
    //    utilities.notifyUser(error.errorDescription, 'Error');
    //  }
    //});
  };

  // self.checkModal = function() {
  //   if (self.authView !== undefined) {
  //     console.log(self.authView.location);
  //     console.log(self.authView.params);
  //     if (self.authView.params.authToken !== undefined) {
  //
  //     }
  //   }
  // };
  //
  // self.terminateModal = function() {
  //   console.log("BackgroundViewModel - in terminateModal");
  //   if (self.authView !== undefined) {
  //     console.log("Terminating modal dialog...");
  //     steroids.layers.pop();
  //     self.authView.unload();
  //   }
  //   clearInterval(self.authTimer);
  // };

  self.login = function(creds) {
    console.log('BackgroundViewModel.login start');

    if (!ByoBabies.Utilities.checkConnection()) {
      console.log('LogonViewModel.login - no network connection');
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', 'Error');
      return false;
    }

    var url = self.baseUrl + 'api/account/login';
    console.log('LogonViewModel.login - POST URL - ' + url);
    var input = {
      UserName: creds.user,
      Password: creds.pw,
      RememberMe: true
    };

    var jqxhr = $.ajax({
      url: url,
      type: 'POST',
      cache: false,
      crossDomain: true,
      data: input,
      success: function(data) {
        console.log('LogonViewModel.login - ajax login successful.');
        var msg = {
          src: 'logon',
          action: 'loggedin'
        };
        console.log('notifying UI of logon status.')
        window.postMessage(msg, '*');
        self.saveCredentials(creds);
        self.getProfile();
        // self.handleNavigation({
        //  id: 'nearby',
        //  url: '/views/nearby/index.html'
        //});
      },
      error: function(jqxHR, exception) {
        console.log('BackgroundViewModel.login - ajax login unsuccessful - ' +
          jqxHR.responseText);
        console.log('BackgroundViewModel.login - ajax login unsuccessful - ' +
          exception);
        console.log(exception);
        console.log(jqxHR);

        if (jqxHR.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser('Unable to login.  Please try again later.',
            'Error');
        }
      }
    });
  };

  self.logout = function() {

    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', 'Error');
      return false;
    }

    var url = self.baseUrl + 'api/account/logout';
    var jqxhr = $.post(url, function(data) {
      self.handleNavigation({
        id: 'home',
        url: '/views/home/index.html'
      });
      //window.localStorage.setItem('credentials', '');
    })
      .error(function(jqxhr, exception) {
        if (jqxHR.responseText && jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'An unknown error occurred while logging out.',
            'Error');
        }
      })
      .complete(function() {});
  };

  self.getUserInfo = function() {
    console.log(
      "BackgroundViewModel - fetching userInfo to verify registration");
    var url = self.baseUrl + 'api/account/userinfo',
      jqxhr = $.ajax({
        url: url,
        type: 'GET',
        cache: false,
        crossDomain: true,
        withCredentials: true,
        beforeSend: addHeaders,

        success: function(data) {
          console.log(
            "BackgroundViewModel - fetched authenticated user info - " +
            JSON.stringify(data));

          if (data !== undefined && data.HasRegistered != true) {
            console.log('User has not registered');
            self.register(data.UserName);
          } else if (data) {
            var msg = {
              src: 'logon',
              action: 'loggedin'
            };
            console.log('notifying UI of login status.')
            window.postMessage(msg, '*');
          }

        },
        error: function(jqxHR, exception) {
          if (jqxHR.responseText && jqxHR.responseText !== '') {
            ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
          } else {
            ByoBabies.Utilities.notifyUser(
              'Unable to get your registration status. Please try again later.',
              'Error');
          }
        }
      });

  };

  var addHeaders = function(xhr) {
    var authData = JSON.parse(window.localStorage.getItem('access_token'));
    if (authData && authData !== null) {
      xhr.setRequestHeader("Authorization", "Bearer " + authData['access_token']);
    }
  };

  self.register = function(userName) {
    console.log(
      "BackgroundViewModel - registering user " + userName);
    var url = self.baseUrl + 'api/account/registerexternal',
      jqxhr = $.ajax({
        url: url,
        type: 'POST',
        cache: false,
        crossDomain: true,
        withCredentials: true,
        beforeSend: addHeaders,
        data: {
          'UserName': userName
        },
        success: function(data) {
          console.log(
            "BackgroundViewModel - successfully registered user");

          var msg = {
            src: 'logon',
            action: 'registered'
          };
          console.log('notifying UI of register status.');
          window.postMessage(msg, '*');
        },
        error: function(jqxHR, exception) {
          if (jqxHR.responseText && jqxHR.responseText !== '') {
            ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
          } else {
            ByoBabies.Utilities.notifyUser(
              'Unable to register you at this time. Please try again later.',
              'Error');
          }
        }
      });
  };

  self.getProfile = function() {
    console.log('BackgroundViewModel.getProfile start');
    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', function() {},
        'Error');
      return false;
    }

    var url = self.baseUrl + 'api/account/get';
    console.log('BackgroundViewModel.getProfile : url - ' + url);
    var jqxhr = $.ajax({
      url: url,
      type: 'GET',
      cache: false,
      crossDomain: true,
      success: function(data) {
        console.log("BackgroundViewModel.getProfile() - ajax call complete");

        window.localStorage.setItem("profile", JSON.stringify(data));
        var msg = {
          src: 'profile',
          action: 'loaded'
        };
        console.log('notifying UI of profile fetched.')
        window.postMessage(msg, '*');
      },
      error: function(jqxhr, exception) {
        console.log(
          'BackgroundViewModel.getProfile - ajax login unsuccessful - ' +
          jqxhr.responseText);
        console.log(
          'BackgroundViewModel.getProfile - ajax login unsuccessful - ' +
          exception);
        console.log(exception);
        console.log(jqxhr);
        if (jqxhr.status === 401) {
          return;
        }

        if (jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to load your profile.  Please try again later.',
            'Error');
        }
      },
      complete: function() {}
    });
  };

  self.saveCredentials = function(creds) {
    var mode = 'ECB',
      cipherPwText = byteArrayToHex(
        rijndaelEncrypt(creds.pw, creds.user, mode));

    window.localStorage.setItem('credentials', JSON.stringify({
      user: creds.user,
      pw: cipherPwText
    }));
  };

  self.fetchCredentials = function() {
    var credsJson,
      mode = 'ECB',
      creds = window.localStorage.getItem('credentials');
    if (creds) {
      credsJson = JSON.parse(creds);
      var decryptedText = byteArrayToString(
        rijndaelDecrypt(hexToByteArray(credsJson.pw), credsJson.user, mode));
      credsJson.pw = decryptedText;
    }
    return credsJson;
  };
};
