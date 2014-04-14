function LogonViewModel() {

  console.log("Initailizing a LogonViewModel instance");

  NavViewModel.apply(this);

  var self = this;
  self.template = "welcomeView";

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

  self.handleMessage = function(msg) {
    console.log("LogonViewModel - received msg.");
    //TODO - handle loading UI close
  };


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
