//background services controller
$.support.cors = true;

var bgvm = new BackgroundViewModel();

document.addEventListener("deviceready", function() {
  var ajaxFactory = new ByoBabies.Ajax("http://dev.byobabies.com:8080/",
    function(jqxhr) {
      //noop
    });
  window.ByoBabies.Api = window.ByoBabies.ApiFactory(ajaxFactory);

  bgvm.initialize();
}, false);
