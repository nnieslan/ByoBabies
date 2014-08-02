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
    var providersPromise = ByoBabies.Api.promiseLoginProviders;
    $.when(providersPromise())
      .then(function(data) {

      });
  };

  self.loginExternalComplete = function(data) {
    console.log(
      'BackgroundViewModel - storing token data from external login - ' + data.provider
    );
    window.localStorage.setItem('access_token', JSON.stringify(data));
    self.getUserInfo();

  };

  self.login = function(creds) {
    console.log('BackgroundViewModel.login start');

    if (!ByoBabies.Utilities.checkConnection()) {
      console.log('LogonViewModel.login - no network connection');
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', 'Error');
      return false;
    }

    // var url = self.baseUrl + 'api/account/login';
    //console.log('LogonViewModel.login - POST URL - ' + url);
    var input = {
      UserName: creds.user,
      Password: creds.pw,
      RememberMe: true
    };

    $.when(ByoBabies.Api.login(input))
      .then(function() {

      });

    // var jqxhr = $.ajax({
    //   url: url,
    //   type: 'POST',
    //   cache: false,
    //   crossDomain: true,
    //   data: input,
    //   success: function(data) {
    //     console.log('LogonViewModel.login - ajax login successful.');
    //     var msg = {
    //       src: 'logon',
    //       action: 'loggedin'
    //     };
    //     console.log('notifying UI of logon status.')
    //     window.postMessage(msg, '*');
    //     self.saveCredentials(creds);
    //     self.getProfile();
    //     // self.handleNavigation({
    //     //  id: 'nearby',
    //     //  url: '/views/nearby/index.html'
    //     //});
    //   },
    //   error: function(jqxHR, exception) {
    //     console.log('BackgroundViewModel.login - ajax login unsuccessful - ' +
    //       jqxHR.responseText);
    //     console.log('BackgroundViewModel.login - ajax login unsuccessful - ' +
    //       exception);
    //     console.log(exception);
    //     console.log(jqxHR);
    //
    //     if (jqxHR.responseText !== '') {
    //       ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
    //     } else {
    //       ByoBabies.Utilities.notifyUser(
    //         'Unable to login.  Please try again later.',
    //         'Error');
    //     }
    //   }
    // });
  };

  self.logout = function() {

    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', 'Error');
      return false;
    }

    $.when(ByoBabies.Api.logout())
      .then(function() {
        self.handleNavigation({
          id: 'home',
          url: '/views/home/index.html'
        });
      });
    // var url = self.baseUrl + 'api/account/logout';
    // var jqxhr = $.post(url, function(data) {
    //
    //   //window.localStorage.setItem('credentials', '');
    // })
    //   .error(function(jqxhr, exception) {
    //     if (jqxHR.responseText && jqxhr.responseText !== '') {
    //       ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
    //     } else {
    //       ByoBabies.Utilities.notifyUser(
    //         'An unknown error occurred while logging out.',
    //         'Error');
    //     }
    //   })
    //   .complete(function() {});
  };

  self.getUserInfo = function() {
    console.log(
      "BackgroundViewModel - fetching userInfo to verify registration");
    $.when(ByoBabies.Api.promiseUserInfo())
      .then(function() {

      });
  };

  self.register = function(userName) {
    console.log(
      "BackgroundViewModel - registering user " + userName);
    var data = {
      'UserName': userName
    };
    $.when(ByoBabies.Api.registerUser(data))
      .then(function() {

      });
    // var url = self.baseUrl + 'api/account/registerexternal',
    //   jqxhr = $.ajax({
    //     url: url,
    //     type: 'POST',
    //     cache: false,
    //     crossDomain: true,
    //     withCredentials: true,
    //     beforeSend: addHeaders,
    //     data: {
    //       'UserName': userName
    //     },
    //     success: function(data) {
    //       console.log(
    //         "BackgroundViewModel - successfully registered user");
    //
    //       var msg = {
    //         src: 'logon',
    //         action: 'registered'
    //       };
    //       console.log('notifying UI of register status.');
    //       window.postMessage(msg, '*');
    //     },
    //     error: function(jqxHR, exception) {
    //       if (jqxHR.responseText && jqxHR.responseText !== '') {
    //         ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
    //       } else {
    //         ByoBabies.Utilities.notifyUser(
    //           'Unable to register you at this time. Please try again later.',
    //           'Error');
    //       }
    //     }
    //   });
  };

  self.getProfile = function() {
    console.log('BackgroundViewModel.getProfile start');
    $.when(ByoBabies.Api.promiseProfile())
      .then(function() {

      });
    // if (!ByoBabies.Utilities.checkConnection()) {
    //   ByoBabies.Utilities.notifyUser(
    //     'No data connection is available. Please try again later.',
    //     function() {},
    //     'Error');
    //   return false;
    // }

    // var url = self.baseUrl + 'api/account/get'; console.log(
    //   'BackgroundViewModel.getProfile : url - ' + url);
    // var jqxhr = $.ajax({
    //   url: url,
    //   type: 'GET',
    //   cache: false,
    //   crossDomain: true,
    //   success: function(data) {
    //     console.log(
    //       "BackgroundViewModel.getProfile() - ajax call complete");
    //
    //     window.localStorage.setItem("profile", JSON.stringify(data));
    //     var msg = {
    //       src: 'profile',
    //       action: 'loaded'
    //     };
    //     console.log('notifying UI of profile fetched.')
    //     window.postMessage(msg, '*');
    //   },
    //   error: function(jqxhr, exception) {
    //     console.log(
    //       'BackgroundViewModel.getProfile - ajax login unsuccessful - ' +
    //       jqxhr.responseText);
    //     console.log(
    //       'BackgroundViewModel.getProfile - ajax login unsuccessful - ' +
    //       exception);
    //     console.log(exception);
    //     console.log(jqxhr);
    //     if (jqxhr.status === 401) {
    //       return;
    //     }
    //
    //     if (jqxhr.responseText !== '') {
    //       ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
    //     } else {
    //       ByoBabies.Utilities.notifyUser(
    //         'Unable to load your profile.  Please try again later.',
    //         'Error');
    //     }
    //   },
    //   complete: function() {}
    // });
  };

  // self.saveCredentials = function(creds) {
  //   var mode = 'ECB',
  //     cipherPwText = byteArrayToHex(
  //       rijndaelEncrypt(creds.pw, creds.user, mode));
  //
  //   window.localStorage.setItem('credentials', JSON.stringify({
  //     user: creds.user,
  //     pw: cipherPwText
  //   }));
  // };

  // self.fetchCredentials = function() {
  //   var credsJson,
  //     mode = 'ECB',
  //     creds = window.localStorage.getItem('credentials');
  //   if (creds) {
  //     credsJson = JSON.parse(creds);
  //     var decryptedText = byteArrayToString(
  //       rijndaelDecrypt(hexToByteArray(credsJson.pw), credsJson.user, mode)
  //     );
  //     credsJson.pw = decryptedText;
  //   }
  //   return credsJson;
  // };
};
