//background services controller
$.support.cors = true;

var bgvm = new BackgroundViewModel();

document.addEventListener("deviceready", function() {
  var ajaxFactory = new ByoBabies.Ajax("http://dev.byobabies.com:8080/api/",
    function(jqxhr, exception) {
      if (!jqxhr.cancelGlobalError) {
        if (jqxhr.responseText && jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to perform the requested action at this time. Please try again later.',
            'Error');
        }
      }
    });
  window.ByoBabies.Api = window.ByoBabies.ApiFactory(ajaxFactory);

  bgvm.initialize();
}, false);
