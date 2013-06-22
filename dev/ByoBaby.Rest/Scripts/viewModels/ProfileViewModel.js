/// <reference path="..//_references.js" />

/*globals ko*/


function Country(name, code) {
    /// <summary>
    /// The view model that represents a state or province
    /// </summary>

    this.DisplayName = name;
    this.Code = code;
};

function ChildViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
};

function GroupViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
};

function ProfileViewModel(svcUrl, id) {
    /// <summary>
    /// The view model that manages the user's profile
    /// </summary>

    var self = this;

    self.template = "profileView";

    //view model properties
    self.baseUrl = (svcUrl == undefined ? '' : svcUrl);

    self.ProfilePictureUrl = 'http://m.c.lnkd.licdn.com/mpr/mpr/shrink_200_200/p/3/000/029/338/0961cc9.jpg';

    /// <summary>
    /// An observable containing an indicator denoting if the user-entered data is valid.
    /// This is "hack" to ensure that knockout validation doesn't render in the UI until the user
    /// clicks an action button the first time.
    /// </summary>
    self.valid = ko.observable(true);

    self.availableStates = [new Country('California', 'CA'), new Country('Colorado', 'CO')];

    self.selectedState = ko.observable();

    ///// <summary>
    ///// An observable containing the logged on user's profile.
    ///// </summary>
    //self.profile = ko.observable();

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
        }
    }

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
            ko.mapping.fromJS(data, mapping, self);
            if (self.State !== undefined && self.State() !== '') {
                ko.utils.arrayForEach(self.availableStates, function (value) {
                    if (value.Code == self.State()) {
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

    self.saveProfile = function () {

        console.log("ProfileViewModel.saveProfile() called");

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }
        application.isProcessing(true);
        
        var url = self.baseUrl + 'api/' + self.Id() + '/profile';
        
        var input = ko.mapping.toJS(self);
        if (input.Children == null) { input.Children = []; }
        if (input.MemberOf == null) { input.MemberOf = []; }
        if (input.Interests == null) { input.Interests = []; }

        var jqxhr = $.post(url, input, function (data) {
            console.log("ProfileViewModel.saveProfile() - ajax call complete");
            ko.mapping.fromJS(data, self);

        })
        .error(function (jqxhr, exception) {
            console.log("ProfileViewModel.saveProfile() - ajax POST errored : " + jqxhr.repsonseText);

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
        //$(list).listview("refresh");
        //$(form).submit(function () {
        //    self.saveProfile();
        //    return false;
        //});
    }

    self.addChild = function () {

        console.log("Adding new child");
        var list = '#profileView-childrenlist';
        self.Children.push(new ChildViewModel({
            'Id': null, 'Name': '', 'Age': '', 'Gender': ''
        }));
        //redraw the list to show the new child correctly.
        $(list).trigger('create')
        $(list).listview("refresh");

    };

    self.removeChild = function (value) {
        console.log("Removing child : " + value.Id);

    }
};
