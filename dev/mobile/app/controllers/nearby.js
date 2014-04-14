//nearby controller
console.log("Executing the nearby view controller");

var utilities = new Helpers();
var vm = new NearByViewModel();

document.addEventListener("DOMContentLoaded", function() {
  console.log("Binding the near-by view to knockout viewmodel");
  ko.applyBindings(vm);
});

document.addEventListener("visibilitychange", function() {
  if (document.visibilityState == 'visible') {
    steroids.view.navigationBar.show("near by");
    vm.initialize();
    var logoutButton = new steroids.buttons.NavigationBarButton();
    logoutButton.title = "log out";
    logoutButton.onTap = function() {
      console.log("logging the user out...");
      var msg = {
        src: 'logon',
        action: 'logout'
      };
      console.log('notifying background to attempt logout.')
      window.postMessage(msg, '*');
    };
    steroids.view.navigationBar.update({
      buttons: {
            right: [logoutButton]
      }
    });
  }
}, false);
