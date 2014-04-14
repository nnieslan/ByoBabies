//background services controller
$.support.cors = true;

var utilities = new Helpers();
var bgvm = new BackgroundViewModel();

document.addEventListener("deviceready", function() {
  bgvm.initialize();
}, false);
