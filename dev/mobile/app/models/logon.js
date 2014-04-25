function LogonViewModel() {

  console.log("Initailizing a LogonViewModel instance");

  NavViewModel.apply(this);

  var self = this;
  self.template = "welcomeView";

  self.loginProviders = ko.observableArray();

  self.user = ko.observable("").extend({
    required: "Please enter your username"
  });

  self.password = ko.observable("").extend({
    required: "Please enter your password"
  });

  self.rememberOptions = ko.observableArray(['yes', 'no']);

  self.remember = ko.observableArray(['yes']);

  self.valid = ko.observable(true);

  self.loggedIn = ko.observable(false);

  self.loggedOut = ko.computed(function() {
    return !self.loggedIn();
  }, self);

  self.login = function() {
    console.log('LogonViewModel.login start');
    //validate the form before processing.
    self.valid(!self.user.hasError() && !self.password.hasError());
    if (!self.valid()) {
      console.log('LogonViewModel.login - parameters invalid');
      console.log('LogonViewModel.login - username - ' + self.user());
      return false;
    }

    var base = window.localStorage.getItem('svcUrl');
    var url = base + 'api/account/login';
    console.log('LogonViewModel.login - POST URL - ' + url);
    var input = {
      UserName: self.user(),
      Password: self.password(),
      RememberMe: (self.remember()[0] === 'yes')
    };

    var msg = {
      src: 'logon',
      action: 'login',
      credentials: {
        user: self.user(),
        pw: self.password()
      }
    };
    //TODO - handle loading UI open
    console.log('notifying background to attempt logon.')
    window.postMessage(msg, '*');

  };

  self.loginExternal = function(data) {
    var authUrl = self.baseUrl + data.Url,
      ref = window.open(authUrl, '_blank', 'location=yes');
    console.log('Launching auth window with url - ' + authUrl);
    ref.addEventListener('loadstop', function(event) {
      console.log(event.url);
      var query = utilities.parseUrlQueryString(event.url);
      console.log(query);
      if(query !== undefined && query['access_token'] !== undefined){
        console.log('Access Token found : ' + query['access_token']);

        var msg = {
          src: 'logon',
          action: 'externallogin',
          data: query
        };
        //TODO - handle loading UI open
        console.log('notifying background of external logon success.')
        window.postMessage(msg, '*');
        ref.close();
      }
    });
  };

  self.handleMessage = function(msg) {
    console.log("LogonViewModel - received msg.");
    //TODO - handle loading UI close
    var source = msg.data.src || '',
      action = msg.data.action || '';

    if (source === 'logon' && action === 'providersloaded') {
      var providers = window.localStorage.getItem('loginProviders');
      ko.utils.arrayForEach(JSON.parse(providers), function(item) {
        self.loginProviders.push(item);
      });
    }
  };
  window.addEventListener("message", self.handleMessage);
  //  var signupButton = new steroids.buttons.NavigationBarButton();
  //  signupButton.title = "sign up";
  //  signupButton.onTap = function() {
  //    console.log("Navigating to register view...");
  //    var registerWebView = new steroids.views.WebView({location: "/views/register/index.html", id: "registerView"});
  //    steroids.layers.replace({view: registerWebView});
  //  };
  //  steroids.view.navigationBar.update({
  //    //overrideBackButton: true,
  //    buttons: {
  //          right: [signupButton]
  //    }
  //  });
}
