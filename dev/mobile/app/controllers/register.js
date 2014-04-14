//register controller

console.log("Executing the register view controller");

var vm = new RegistrationViewModel();

document.addEventListener("DOMContentLoaded", function() {
  ko.applyBindings(vm);
});


document.addEventListener("visibilitychange", function() {

  if (document.visibilityState == 'visible') {
    steroids.view.navigationBar.show("sign up");
  }

}, false);
