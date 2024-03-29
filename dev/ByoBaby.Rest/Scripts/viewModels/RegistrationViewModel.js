﻿/// <reference path="..//_references.js" />

/*globals ko*/

function RegistrationViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the logon status of a user
    /// </summary>
    /// 
    NavViewModel.apply(this, [svcUrl]);

    var self = this;

    self.template = "registerView";

    /// <summary>
    /// An observable containing the user's logon username.
    /// </summary>
    self.user = ko.observable("").extend({ required: "Please enter your username" });

    /// <summary>
    /// An observable containing the user's display name.
    /// </summary>
    self.displayName = ko.observable("");

    /// <summary>
    /// An observable containing the user's password.
    /// </summary>
    self.password = ko.observable("").extend({ required: "Please enter your password" });

    /// <summary>
    /// An observable containing the user's password.
    /// </summary>
    self.confirmPassword = ko.observable("").extend({ required: "Please re-enter your password"});

    /// <summary>
    /// An observable containing the user's registration status.
    /// </summary>
    self.registered = ko.observable(false);

    /// <summary>
    /// An observable containing an indicator denoting if the user entered data is valid.
    /// This is "hack" to ensure that knockout validation doesn't render in the UI until the user
    /// clicks the Logon button the first time.
    /// </summary>
    self.valid = ko.observable(true);

    /// <summary>
    /// A function that registers the new user via REST api.
    /// </summary>
    self.register = function () {
        //validate the form before processing.
        self.valid(!self.user.hasError() && !self.password.hasError() && !self.confirmPassword.hasError());
        if (!self.valid()) {
            return false;
        }

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        application.isProcessing(true);
        var url = self.baseUrl + 'api/account/register';
        var input = {
            Email: self.user,
            DisplayName: self.displayName,
            Password: self.password
        };

        var jqxhr = $.post(url, input, function (data) {
            self.registered(true);
        })
            .error(function (jqxHR, exception) {
                application.isProcessing(false);
                if (jqxHR.responseText !== '') {
                    utilities.notifyUser(jqxHR.responseText, 'Error');
                    //TODO - we probably need an error case for 'Account exists'
                } else {
                    utilities.notifyUser('Unable to register.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                self.password(null); //clear the pw post-register-login
                self.confirmPassword(null);
            });
    };

    self.afterViewRender = function (elements) {
        console.log("registrationViewModel.afterViewRender()");
        //TODO - figure out how to apply this functionality inside the view models.
        $('#new-username').watermark('what\'s your email?');
        $('#new-pw').watermark('what\'s your password?');
        $('#confirm-pw').watermark('confirm your password');
        $('#displayname').watermark('what\'s your name?');

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