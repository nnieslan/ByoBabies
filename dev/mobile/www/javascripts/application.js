
// steroids.view.navigationBar.show("Hello World");


$.support.cors = true;

function showCat() {
  var webView = new steroids.views.WebView("showCat.html");
  steroids.layers.push(webView);
}

var authvm = new LogonViewModel('http://dev.egoboom.com:81/');
var utilities = new Helpers();
var loginText = null;

document.addEventListener("DOMContentLoaded", function() {
  loginText = document.querySelector("#login-text");
  ko.applyBindings(authvm);

});

var loginButton = new steroids.buttons.NavigationBarButton();
loginButton.title = "Log in";

var logoutButton = new steroids.buttons.NavigationBarButton();
logoutButton.title = "Log out";

loginButton.onTap = function() {
    loginText.textContent = "Welcome, Agnus M.!";
    steroids.view.navigationBar.update({
        buttons: {
          right: [logoutButton]
        }
    });
};

logoutButton.onTap = function() {
    loginText.textContent = "Please log in.";
    steroids.view.navigationBar.update({
        buttons: {
          right: [loginButton]
        }
    });
};

steroids.view.navigationBar.update({
    overrideBackButton: true,
    buttons: {
      right: [loginButton]
    }
});

steroids.view.navigationBar.show("egoBoom");