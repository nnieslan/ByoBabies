/// <reference path="..//_references.js" />

/*globals ko*/

function Country(name, code) {
    /// <summary>
    /// The view model that represents a state or province
    /// </summary>

    this.DisplayName = name;
    this.Code = code;
}

function NotificationsViewModel(data) {
    NavViewModel.apply(this, [data.baseUrl]);

    var self = this;
    self.button(new BackButtonModel());
    self.template = "notificationsView";
    self.profile = data;

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };

    self.navigateToOrigin = function (selected) {
        if (selected !== null && selected.OriginatorType === 'FriendRequest') {
            var url = self.baseUrl + 'api/requests/' + selected.OriginatorId;
            var jqxhr = $.get(url, function (data) {
                console.log("NotificationsViewModel.navigateToOrigin() - ajax call for origin request is complete");
                application.navigateTo(new RequestViewModel(self.baseUrl, data));
            })
                .error(function (jqxhr, exception) {
                    if (jqxhr.status === '401') {
                        application.clear();
                        return;
                    }

                    if (jqxhr.responseText !== '') {
                        utilities.notifyUser(jqxhr.responseText, 'Error');
                    } else {
                        utilities.notifyUser('Unable to load notifications.  Please try again later.', 'Error');
                    }
                })
                .complete(function () {
                    application.isProcessing(false);

                });
        }
    };
}

function RequestViewModel(svcUrl, data) {
    NavViewModel.apply(this, [svcUrl]);

    var self = this;
    self.button(new BackButtonModel());
    self.template = "requestView";

    ko.mapping.fromJS(data, {}, self);

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };

    self.accept = function () {
        console.log("RequestViewModel.accept() called");
        self.postResponse('accept');
    };

    self.deny = function () {
        console.log("RequestViewModel.deny() called");
        self.postResponse('deny');
    };

    self.postResponse = function (actionValue) {
        console.log("RequestViewModel.postResponse() called");

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }
        application.isProcessing(true);

        var url = self.baseUrl + 'api/requests/' + self.Id();
        var jqxhr = $.post(url, '=' + actionValue, function (data) {
            console.log("RequestViewModel.postResponse() - ajax call complete");
            application.back();
            application.loggedInUserProfile().refreshNotifications();
        })
            .error(function (jqxhr, exception) {
                console.log("RequestViewModel.postResponse() - ajax POST errored : " + jqxhr.repsonseText);

                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to respond to request.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };
}

function ChildViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
}

function GroupViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
}

function FriendsViewModel(data) {
    NavViewModel.apply(this, [data.baseUrl]);

    var self = this;
    self.button(new BackButtonModel());
    self.template = "friendsListView";
    self.profile = data;

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };
}

function PersonViewModel(svcUrl, data) {

    NavViewModel.apply(this, [svcUrl]);

    var self = this;
    self.button(new BackButtonModel());
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

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
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
                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load your profile.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);
            });
    };

    self.viewFriends = function () {
        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
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
                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    var errorMsg = 'Unable to the friends of the currently selected person. Please try again later.';
                    utilities.notifyUser(errorMsg, 'Error');
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

function FriendViewModel(data) {

    function F() { }
    F.prototype = data;
    F.prototype.template = "friendDetailView";
    var retval = new F();

    return retval;
}

function ProfileViewModel(data) {
    /// <summary>
    /// The view model that manages the user's profile
    /// </summary>
    function P() { }
    P.prototype = data;
    P.prototype.template = "profileView";

    var self = new P();
    self.button(new MenuButtonModel());
    self.ShowUploader = ko.observable(false);
    self.Notifications = ko.observableArray([]);

    /// <summary>
    /// An observable containing an indicator denoting if the user-entered data is valid.
    /// This is "hack" to ensure that knockout validation doesn't render in the UI until the user
    /// clicks an action button the first time.
    /// </summary>
    self.valid = ko.observable(true);

    self.fullname = ko.computed(function () {
        if (self.FirstName !== undefined && self.LastName !== undefined) {
            return self.FirstName() + ' ' + self.LastName();
        }
        return '';
    }, self);

    /// <summary>
    /// A function that fetches the profile of the current user.
    /// </summary>
    self.getProfile = function () {

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/account/get';
        var jqxhr = $.get(url, function (data) {
            console.log("ProfileViewModel.getProfile() - ajax call complete");
            self.update(data);
            //ko.mapping.fromJS(data, self.mapping, self.prototype);
            if (self.State !== undefined && self.State() !== '') {
                ko.utils.arrayForEach(self.availableStates, function (value) {
                    if (value.Code === self.State()) {
                        self.selectedState(value);
                    }
                });
            }
            self.selectedState.subscribe(function (newValue) {
                if (newValue && newValue.Code !== self.State()) {
                    self.State(newValue.Code);
                }
            });

            application.navigateHome();
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load your profile.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);
            });
    };

    self.saveProfile = function () {

        console.log("ProfileViewModel.saveProfile() called");

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }
        application.isProcessing(true);

        var url = self.baseUrl + 'api/' + self.Id() + '/profile';

        var input = ko.mapping.toJS(self);
        if (input.Children === null) { input.Children = []; }
        if (input.MemberOf === null) { input.MemberOf = []; }
        if (input.Interests === null) { input.Interests = []; }
        //if (input.Friends == null) { input.Friends = []; }
        //if (input.Notifications == null) { input.Notifications = []; }
        var jqxhr = $.post(url, input, function (data) {
            console.log("ProfileViewModel.saveProfile() - ajax call complete");
            ko.mapping.fromJS(data, self);

        })
            .error(function (jqxhr, exception) {
                console.log("ProfileViewModel.saveProfile() - ajax POST errored : " + jqxhr.repsonseText);

                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load your profile.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };

    self.afterViewRender = function (elements) {
        console.log("ProfileViewModel.afterViewRender() called");

        $('#mobilePhone').watermark('phone number');
        $('#neighborhood').watermark('neighborhood');
        $('#state').watermark('state');
        $('#city').watermark('city');

        //refreshing jquery themes post view render via .trigger().
        //This is due to timing of knockout template rendering.  To accomplish
        //this I had to put a base div in each template with an id based
        //on the template name itself so I could ensure only the current
        //template is themed.
        var view = '#' + application.currentViewModel().template + '-content';
        var list = '#' + application.currentViewModel().template + '-list';
        $(view).trigger('create');

        //refreshing the ui-content div size after the header appears post-login.
        $('#ui-content').trigger('resize');

        self.afterAdd();
        //$(list).listview("refresh");
        //$(form).submit(function () {
        //    self.saveProfile();
        //    return false;
        //});
    };

    self.toggleUploader = function () {
        self.ShowUploader(!self.ShowUploader());
    };

    self.refreshNotifications = function (navigationActionCallback) {
        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/notifications';
        var jqxhr = $.get(url, function (data) {
            console.log("ProfileViewModel.viewNotifications() - ajax call complete");
            self.Notifications([]);
            var i, max = data.length;
            for (i = 0; i < max; i++) {
                self.Notifications.push(data[i]);
            }
            if (navigationActionCallback !== undefined && navigationActionCallback !== null) {
                navigationActionCallback();
            }
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load notifications.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };

    self.viewNotifications = function () {
        self.refreshNotifications(function () {
            application.navigateTo(new NotificationsViewModel(self));
        });
    };

    self.viewFriends = function () {
        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/' + self.Id() + '/friends';
        var jqxhr = $.get(url, function (data) {
            console.log("ProfileViewModel.viewFriends() - ajax call complete");
            self.Friends([]);
            var i, max = data.length;
            for (i = 0; i < max; i++) {
                var current = new PersonViewModel(self.baseUrl, data[i]);
                self.Friends.push(new FriendViewModel(current));
            }
            application.navigateTo(new FriendsViewModel(self));
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === '401') {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load your friends.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };

    self.addChild = function () {

        console.log("Adding new child");
        var list = '#profileView-childrenlist';
        self.Children.push(new ChildViewModel({
            'Id': null,
            'Name': '',
            'Age': '',
            'Gender': ''
        }));
        //redraw the list to show the new child correctly.
        $(list).trigger('create');
        $(list).listview("refresh");

    };

    self.removeChild = function (value) {
        console.log("Removing child : " + value.Id);
        //use mapped Remove in the case of an existing child
        if (value.Id() !== null && value.Id() !== '') {
            self.Children.mappedRemove(value);
        } else {
            var index = ko.utils.arrayIndexOf(self.Children(), value);
            self.Children.remove(self.Children()[index]);
        }
    };

    return self;
}
