console.log("Executing the profile view controller");


var vm = new ProfileViewModel();

document.addEventListener("DOMContentLoaded", function() {
  console.log("Binding the home view to knockout viewmodel");
  ko.applyBindings(vm);
});


document.addEventListener("visibilitychange", function() {
  if (document.visibilityState == 'visible') {
    steroids.view.navigationBar.show("profile");
  }
}, false);
