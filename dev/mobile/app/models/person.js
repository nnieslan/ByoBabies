function PersonViewModel(svcUrl, data) {

    NavViewModel.apply(this, [svcUrl]);

    var self = this;
    //self.button(new BackButtonModel());
    self.template = "personView";
    self.profile = data;
    //view model properties
    var mapping = {
        'Children': {
            create: function (options) {
                return new ChildViewModel(options.data);
            },
            key: function (data) {
                return ko.utils.unwrapObservable(data.Id);
            }
        },
        'MemberOf': {
            create: function (options) {
                return new GroupViewModel(options.data);
            },
            key: function (data) {
                return ko.utils.unwrapObservable(data.Id);
            }
        },
        'Friends': {
            create: function (options) {
                return new FriendViewModel(options.data);
            },
            key: function (data) {
                return ko.utils.unwrapObservable(data.Id);
            }
        }
    };

    self.update = function (data) {
        if (data !== undefined && data !== null) {
            ko.mapping.fromJS(data, mapping, self);
        }
    };
    self.update(data);
    self.ProfilePictureUrl = ko.observable('http://communications.iu.edu/img/photos/people/placeholder.jpg');
    self.Friends = ko.observableArray([]);
    //todo - make this a comprehensive and
    self.availableStates = [new Country('California', 'CA'), new Country('Colorado', 'CO')];
    self.selectedState = ko.observable();
    self.viewDetails = function () {
        self.getProfile(function () { application.navigateTo(self); });
    };

    /// <summary>
    /// A function that fetches the profile of the current user.
    /// </summary>
    self.getProfile = function (navigateDelegate) {

        if (!ByoBabies.Utilities.checkConnection()) {
            ByoBabies.Utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        application.isProcessing(true);
        var url = self.baseUrl + 'api/' + application.loggedInUserProfileId() + '/profile/' + self.Id();
        var jqxhr = $.get(url, function (data) {
            console.log("PersonViewModel.getProfile() - ajax call complete");
            self.update(data);
            if (navigateDelegate !== undefined) { navigateDelegate(); }
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === 401) {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    ByoBabies.Utilities.notifyUser('Unable to load your profile.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);
            });
    };

    self.viewFriends = function () {
        if (!ByoBabies.Utilities.checkConnection()) {
            ByoBabies.Utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/' + self.Id() + '/friends';
        var jqxhr = $.get(url, function (data) {
            console.log("PersonViewModel.viewFriends() - ajax call complete");
            self.Friends([]);
            var i, max = data.length;
            for (i = 0; i < max; i++) {
                var current = new FriendViewModel(new PersonViewModel(self.baseUrl, data[i]));
                self.Friends.push(current);
            }
            application.navigateTo(new FriendsViewModel(self));
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === 401) {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    var errorMsg = 'Unable to the friends of the currently selected person. Please try again later.';
                    ByoBabies.Utilities.notifyUser(errorMsg, 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        var list = '#' + self.template + '-list';
        $(view).trigger('create');

        //refreshing the ui-content div size after the header appears post-login.
        $('#ui-content').trigger('resize');

        self.afterAdd();
    };
}
