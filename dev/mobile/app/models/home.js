function HomeViewModel() {
  console.log("Initailizing a HomeViewModel instance");

  NavViewModel.apply(this);

  var self = this;

  self.isUserLoggedIn = ko.observable(false);

  self.loginClick = function() {
    self.loadLogin();
  }
  self.registerClick = function() {
    self.loadRegister();
  }

  self.loadLogin = function() {
    var logonWebView = new steroids.views.WebView({
      location: '/views/logon/index.html',
      id: 'logonView'
    });
    steroids.layers.push(logonWebView);
  };

  self.loadRegister = function() {
    var registerWebView = new steroids.views.WebView({
      location: '/views/register/index.html',
      id: 'registerView'
    });
    steroids.layers.push(registerWebView);
  };

}
