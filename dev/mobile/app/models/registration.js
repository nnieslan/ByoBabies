function RegistrationViewModel() {
  console.log("Initailizing a RegistrationViewModel instance");

  NavViewModel.apply(this);

  var self = this;
  self.template = "registerView";
  self.user = ko.observable("").extend({
    required: "Please enter your username"
  });
  self.displayName = ko.observable("");
  self.password = ko.observable("").extend({
    required: "Please enter your password"
  });
  self.confirmPassword = ko.observable("").extend({
    required: "Please re-enter your password"
  });
  self.registered = ko.observable(false);
  self.valid = ko.observable(true);

  self.register = function() {
    //validate the form before processing.
    self.valid(!self.user.hasError() && !self.password.hasError() && !self.confirmPassword
      .hasError());
    if (!self.valid()) {
      return false;
    }

    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', 'Error');
      return false;
    }

    application.isProcessing(true);
    var url = self.baseUrl + 'api/account/register';
    var input = {
      Email: self.user,
      DisplayName: self.displayName,
      Password: self.password
    };

    var jqxhr = $.post(url, input, function(data) {
      self.registered(true);
    })
      .error(function(jqxHR, exception) {
        application.isProcessing(false);
        if (jqxHR.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
          //TODO - we probably need an error case for 'Account exists'
        } else {
          ByoBabies.Utilities.notifyUser('Unable to register.  Please try again later.',
            'Error');
        }
      })
      .complete(function() {
        self.password(null); //clear the pw post-register-login
        self.confirmPassword(null);
      });
  };

  self.afterViewRender = function(elements) {
    console.log("registrationViewModel.afterViewRender()");

    //refreshing jquery themes post view render via .trigger().
    //This is due to timing of knockout template rendering.  To accomplish
    //this I had to put a base div in each template with an id based
    //on the template name itself so I could ensure only the current
    //template is themed.
    var view = '#' + self.template + '-content';
    $(view).trigger('create');
    //refreshing the ui-content div size after the header appears post-login.
    $('#ui-content').trigger('resize');

  };
}
