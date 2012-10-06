/// <reference path="..//_references.js" />

/*globals ko*/

function LogonViewModel(svcUrl, id) {
    /// <summary>
    /// The view model that manages the logon status of a user
    /// </summary>

    var self = this;

    self.template = "welcomeView";
    //view model properties
    self.baseUrl = svcUrl;

    /// <summary>
    /// An observable containing the user's logon username.
    /// </summary>
    self.user = ko.observable("").extend({ required: "Please enter your username" });

    /// <summary>
    /// An observable containing the user's password.
    /// </summary>
    self.password = ko.observable("").extend({ required: "Please enter your password" });


    /// <summary>
    /// An observable array containing the rememberMe options.
    /// </summary>
    self.rememberOptions = ko.observableArray(['yes', 'no']);

    /// <summary>
    /// An observable containing an indicator denoting if the username should be persisted.
    /// </summary>
    self.remember = ko.observableArray(['yes']);

    /// <summary>
    /// An observable containing an indicator denoting if the user entered data is valid.
    /// This is "hack" to ensure that knockout validation doesn't render in the UI until the user
    /// clicks the Logon button the first time.
    /// </summary>
    self.valid = ko.observable(true);

    /// <summary>
    /// An observable containing the logged on user's profile.
    /// </summary>
    self.profile = ko.observable();

    //computed values that determine current display info and user state.
    self.firstname = ko.computed(function () {
        if (self.profile() != undefined && self.profile() != null) {
            return self.profile().FirstName;
        }
        else {
            return '';
        }
    }, self);
    self.lastname = ko.computed(function () {
        if (self.profile() != undefined && self.profile() != null) {
            return self.profile().LastName;
        }
        else {
            return '';
        }
    }, self);
    self.fullname = ko.computed(function () {
        return self.firstname() + ' ' + self.lastname();
    }, self);
    self.loggedIn = ko.computed(function () {
        return (!(self.profile() == undefined || self.profile() == null));
    }, self);
    self.loggedOut = ko.computed(function () {
        return !self.loggedIn();
    }, self);

    /// <summary>
    /// A function that registers the new user via REST api.
    /// </summary>
    self.register = function () {
        //validate the form before processing.
        self.valid(!self.user.hasError() && !self.password.hasError());
        if (!self.valid()) {
            return false;
        }

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        application.isProcessing(true);
        var url = self.baseUrl + '/api/account/register';
        var input = {
            email: self.user,
            displayname: self.fullname,
            password: self.password
        };

        var jqxhr = $.post(url, input, function (data) {
            //TODO - login?
            self.getProfile();

        })
        .error(function (jqxHR, exception) {
            application.isProcessing(false);
            if (jqxHR.responseText != '') {
                utilities.notifyUser(jqxHR.responseText, 'Error');
                //TODO - we probably need an error case for 'Account exists'
            } else {
                utilities.notifyUser('Unable to register.  Please try again later.', 'Error');
            }
        })
        .complete(function () {
            self.password(null); //clear the pw post-register-login
        });
    };

    /// <summary>
    /// A function that logs in the current user via REST api.
    /// </summary>
    self.login = function () {
        //validate the form before processing.
        self.valid(!self.user.hasError() && !self.password.hasError());
        if (!self.valid()) {
            return false;
        }

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        application.isProcessing(true);
        var url = self.baseUrl + '/api/account/login';
        var input = {
            username: self.user,
            password: self.password,
            rememberMe: self.remember()[0]
        };

        var jqxhr = $.post(url, input, function (data) {
            self.getProfile();

        })
        .error(function (jqxHR, exception) {
            application.isProcessing(false);
            if (jqxHR.responseText != '') {
                utilities.notifyUser(jqxHR.responseText, 'Error');
            } else {
                utilities.notifyUser('Unable to login.  Please try again later.', 'Error');
            }
        })
        .complete(function () {
            self.password(null);
        });

    };

    /// <summary>
    /// A function that fetches the profile of the current user.
    /// </summary>
    self.getProfile = function () {

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + '/api/account';
        var jqxhr = $.get(url, function (data) {
            self.profile(data);
            application.navigateHome();

        })
        .error(function (jqxhr, exception) {
            if (jqxhr.status == '401') {
                application.clear();
                return;
            }

            if (jqxhr.responseText != '') {
                utilities.notifyUser(jqxhr.responseText, 'Error');
            } else {
                utilities.notifyUser('Unable to load your profile.  Please try again later.', 'Error');
            }
        })
        .complete(function () {
            application.isProcessing(false);

        });
    };

    /// <summary>
    /// A function that logs out the current user via REST api.
    /// </summary>
    self.logout = function () {

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        var url = self.baseUrl + '/api/account/logout';
        var jqxhr = $.post(url, function (data) {
            self.profile(null);
            application.viewModelBackStack([]);
        })
        .error(function (jqxhr, exception) {
            if (jqxhr.responseText != '') {
                utilities.notifyUser(jqxhr.responseText, 'Error');
            } else {
                utilities.notifyUser('An unknown error occurred while logging out.', 'Error');
            }
        })
        .complete(function () { 
            application.isProcessing(false); 
            application.isComplete(false); 
            if (self.remember()[0] == 'no') {
                self.user(null);
            }
        });
    };
};
