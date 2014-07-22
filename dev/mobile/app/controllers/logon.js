//logon controller
$.support.cors = true;

console.log("Executing the logon view controller");

var vm = new LogonViewModel();

document.addEventListener("DOMContentLoaded", function() {
  ko.applyBindings(vm);
});

document.addEventListener("visibilitychange", function() {

    if(document.visibilityState == 'visible'){
      steroids.view.navigationBar.show("log in");
    }
    console.log(JSON.stringify(vm.loginProviders()));
}, false);

  //var signupButton = new steroids.buttons.NavigationBarButton();
  //signupButton.title = "sign up";
  //signupButton.onTap = function() {
  //  console.log("Navigating to register view...");
  //  var registerWebView = new steroids.views.WebView({location: "/views/register/index.html", id: "registerView"});
  //  steroids.layers.push(registerWebView);
  //};
  //steroids.view.navigationBar.update({
  //  overrideBackButton: true,
  //  buttons: {
  //        right: [signupButton]
  //  }
  //});
