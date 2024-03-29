﻿/// <reference path="..//_references.js" />

/*globals ko*/

function LogonViewModel(svcUrl, id) {
    /// <summary>
    /// The view model that manages the logon status of a user
    /// </summary>
    NavViewModel.apply(this, [svcUrl]);
    var self = this;
    self.template = "welcomeView";

    /// <summary>
    /// An observable containing the user's logon username.
    /// </summary>
    self.user = ko.observable("").extend({ required: "Please enter your username" });

    /// <summary>
    /// An observable containing the user's password.
    /// </summary>
    self.password = ko.observable("").extend({ required: "Please enter your password" });

    self.providers = ko.observableArray([]);

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
    /// An observable containing the logged in status of the user.
    /// </summary>
    self.loggedIn = ko.observable(false);

    /// <summary>
    /// A computed negation of logged in status.
    /// </summary>
    self.loggedOut = ko.computed(function () {
        return !self.loggedIn();
    }, self);

    self.getLoginProviders = function () {
        application.isProcessing(true);
        var url = self.baseUrl + 'api/account/externallogins?generateState=true';
        var jqxhr = $.ajax({
            url: url,
            type: 'GET',
            cache: false,
            crossDomain: true,
            success: function (data) {
                if (data !== undefined) {
                    ko.utils.arrayForEach(data, function (item) {
                        self.providers.push(item);
                    });
                }
                $('#' + self.template + '-content').trigger('create');
            },
            error: function (jqxHR, exception) {
                application.isProcessing(false);
                if (jqxHR.responseText !== '') {
                    utilities.notifyUser(jqxHR.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to get the list of providers.  Please try again later.', 'Error');
                }
            }
        });

    };

    self.loginExternal = function (selected) {
        application.isProcessing(true);

        var url = self.baseUrl + selected.Url;
        var jqxhr = $.ajax({
            url: url,
            type: 'GET',
            dataType:'json',
            cache: false,
            callback: '?',
            headers: { 'Access-Control-Allow-Origin': '*' },
            crossDomain: true,
            dataFilter: function (data, dataType) {
                return data;
            },
            success: function (data) {
                application.isProcessing(false);
            },
            error: function (jqxHR, exception) {
                var authUrl = jqxHR.getResponseHeader('Location');
                //var exCookieVal = $.cookie(".AspNet.ExternalCookie"),
                //    cookieVal = $.cookie(".AspNet.Cookies");
                //if (cookieVal === undefined && exCookieVal !== undefined) {
                //    console.log('need to register');
                //}
                if (jqxHR.status === 401 && authUrl !== undefined) {
                    console.log(authUrl);
                    self.redirectLogin(authUrl);
                } else {
                    if (jqxHR.responseText !== '') {
                        utilities.notifyUser(jqxHR.responseText, 'Error');
                    } else {
                        utilities.notifyUser('Unable to login.  Please try again later.', 'Error');
                    }
                }
            }
        });
    };


    self.redirectLogin = function (url) {
      var child = window.open(url, "_blank")
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
        var url = self.baseUrl + 'api/account/login';
        var input = {
            username: self.user,
            password: self.password,
            rememberMe: self.remember()[0]
        };

        var jqxhr = $.post(url, input, function (data) {
            self.loggedIn(true);
        })
            .error(function (jqxHR, exception) {
                application.isProcessing(false);
                if (jqxHR.responseText !== '') {
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
    /// A function that logs out the current user via REST api.
    /// </summary>
    self.logout = function () {

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/account/logout';
        var jqxhr = $.post(url, function (data) {
            self.loggedIn(false);
            application.viewModelBackStack([]);
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('An unknown error occurred while logging out.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);
                application.isComplete(false);
                if (self.remember()[0] === 'no') {
                    self.user(null);
                }
            });
    };
}
