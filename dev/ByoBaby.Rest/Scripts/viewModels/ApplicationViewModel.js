﻿/// <reference path="..//_references.js" />

/*globals ko*/


function NavButtonModel() {

    var self = this;
    self.href = ko.observable();
    self.icon = ko.observable();
    self.clickAction = function () { };
}

function MenuButtonModel() {
    NavButtonModel.apply(this);

    var self = this;

    self.href('#left-panel');
    self.icon('bars');
}

function BackButtonModel() {
    NavButtonModel.apply(this);

    var self = this;

    self.href('#');
    self.icon('back');
    self.clickAction = function () {
        application.back();
    };
}

function NavViewModel(svcUrl) {
    var self = this;
    self.baseUrl = (svcUrl === undefined ? '' : svcUrl);
    self.button = ko.observable(new MenuButtonModel());

    self.afterAdd = function () {
        $("#navButton").buttonMarkup("refresh");
    };
}


function ApplicationViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the view model back-stack
    /// </summary>

    NavViewModel.apply(this, [svcUrl]);

    var self = this;

    //the default view for the application, shown when a user is not logged in.
    self.template = "welcomeView";

    /// <summary> 
    /// The observable view model managing user logon state.
    /// </summary>
    self.logonViewModel = ko.observable();

    /// <summary> 
    /// The observable view model managing user registration state.
    /// </summary>
    //self.registrationViewModel = ko.observable();

    /// <summary> 
    /// The observable view model managing the logged on user's profile.
    /// </summary>
    //self.profileViewModel = ko.observable();

    self.tasksViewModel = ko.observable(new TasksViewModel(self.baseUrl));

    self.tasksViewModel().loadTasks();

    /// <summary> 
    /// The observable indicator used to toggle progress bars.
    /// </summary>
    self.isProcessing = ko.observable(true);

    /// <summary> 
    /// The observable indicator user to toggle donation wizard UI status.
    /// </summary>
    self.isComplete = ko.observable(false);

    /// <summary> 
    /// The observable array managing the UI navigation back stack for the application.
    /// </summary>
    self.viewModelBackStack = ko.observableArray();

    /// <summary> 
    /// The computed current view model to display in the UI.
    /// </summary>
    self.currentViewModel = ko.computed(function () {
        var model, stacklength = self.viewModelBackStack().length;
        if (stacklength > 0) {
            model = self.viewModelBackStack()[stacklength - 1];
        } else {
            model = this;
        }
        return model;
    }, self);

    /// <summary> 
    /// The computed indicator denoting if a back button should be shown.
    /// </summary>
    self.backButtonRequired = ko.dependentObservable(function () {
        var validStack = (self.viewModelBackStack().length > 0),
            validTemplate = (self.currentViewModel().template !== 'profileView'),
            validStatus = (!self.isProcessing() && !self.isComplete());

        return (validStack && validTemplate && validStatus);
    }, self);


    /// <summary> 
    /// The computed indicator denoting if a logout button should be shown.
    /// </summary>
    self.logoutButtonRequired = ko.dependentObservable(function () {
        var validLogin = (self.logonViewModel() !== undefined && self.logonViewModel() !== null && self.logonViewModel().loggedIn()),
            validStatus = (!self.isProcessing() && !self.isComplete());
        return (validLogin && validStatus);
    }, self);


    self.loggedInUserProfile = function () {
        var i, task;
        if (self.logonViewModel() !== null && self.logonViewModel().loggedIn()) {
            for (i = 0; i < self.tasksViewModel().tasks().length; i++) {
                task = self.tasksViewModel().tasks()[i];
                if (task.DisplayName === 'Profile' && task.value.Id !== undefined) {
                    return task.value;
                }
            }
        }
        return null;

    };

    /// <summary> 
    /// The currently logged in user's profile Id if possible.
    /// </summary>
    self.loggedInUserProfileId = function () {
        var profile = self.loggedInUserProfile();
        if (profile !== null) { return profile.Id(); }
        return null;
    };

    //functions

    self.viewNotifications = function () {
        if (self.loggedInUserProfile() !== null) {
            self.loggedInUserProfile().viewNotifications();
        }
    };

    /// <summary> 
    /// Initializes the LogonViewModel for the application.
    /// </summary>
    self.loadLogin = function (viewModel) {
        var i, task;
        self.logonViewModel(viewModel);
        self.logonViewModel().getLoginProviders();
        self.logonViewModel().loggedIn.subscribe(function (newValue) {
            if (newValue) {
                console.log("logonViewModel().loggedIn().subscribe - user is logged in");
                for (i = 0; i < self.tasksViewModel().tasks().length; i++) {
                    task = self.tasksViewModel().tasks()[i];
                    if (task.DisplayName === 'Profile') {
                        console.log("logonViewModel().loggedIn().subscribe - fetching user profile");
                        task.value.getProfile();
                    }
                }
            }
        });
    };

    /// <summary> 
    /// Initializes the RegistrationViewModel for the application.
    /// </summary>
    self.loadRegistration = function () {
        self.registrationViewModel(new RegistrationViewModel());
        self.registrationViewModel().registered.subscribe(function (newValue) {
            if (newValue) {
                self.logonViewModel.loggedIn(true);
                self.viewModelBackStack([]);
            }
        });
    };

    /// <summary> 
    /// Navigates to the viewModel indicated, optionally clearing back stack history.
    /// </summary>
    self.navigateTo = function (viewModel, clear) {
        console.log("application.navigateTo - Navigating to : " + viewModel.template);
        if (clear !== undefined && clear === true) {
            self.viewModelBackStack([]);
        }
        self.viewModelBackStack.push(viewModel);
    };

    /// <summary>
    /// Navigates to the homeView.
    /// </summary>
    self.navigateHome = function () {
        self.navigateTo(self, true);
    };

    /// <summary>
    /// Navigates to the ProfileViewModel, which backs the homeView.
    /// </summary>
    self.navigateRegister = function () {
        var viewModel = new RegistrationViewModel(self.baseUrl);
        viewModel.registered.subscribe(function (newValue) {
            if (newValue) {
                self.logonViewModel().loggedIn(true);
                self.viewModelBackStack([]);
            }
        });
        self.navigateTo(viewModel);
    };

    /// <summary>
    /// In the event of a HTTP 401, we need to clear the application.
    /// </summary>
    self.clear = function () {
        self.loadLogin(new LogonViewModel(self.baseUrl));
        self.viewModelBackStack([]);
        self.isProcessing(false);
        self.isComplete(false);
    };


    /// <summary> 
    /// Navigates back to the previous view if possible.
    /// </summary>
    self.back = function () {
        self.viewModelBackStack.pop();
        if (self.viewModelBackStack().length === 0 && self.logonViewModel().loggedIn()) {
            self.viewModelBackStack.push(new TasksViewModel());
        }
    };

    /// <summary> 
    /// Fetches the view to display for the current viewModel.
    /// </summary>
    self.templateSelector = function (viewModel) {
        return viewModel.template;
    };

    self.afterViewRender = function (elements) {
        //TODO - figure out how to apply this functionality inside the view models.
        console.log("application.afterViewRender()");
        $('#username').watermark('what\'s your email?');
        $('#new-username').watermark('what\'s your email?');
        $('#pw').watermark('what\'s your password?');
        $('#new-pw').watermark('what\'s your password?');
        $('#confirm-pw').watermark('confirm your password');
        $('#displayname').watermark('what\'s your name?');

        //refreshing jquery themes post view render via .trigger().
        //This is due to timing of knockout template rendering.  To accomplish
        //this I had to put a base div in each template with an id based
        //on the template name itself so I could ensure only the current
        //template is themed.
        var view = '#' + application.currentViewModel().template + '-content';
        $(view).trigger('create');
        if (view === '#welcomeView-content') {
            $("#logonForm").submit(function () {
                application.logonViewModel().login();
                return false;
            });
        }
        //refreshing the ui-content div size after the header appears post-login.
        $('#ui-content').trigger('resize');
        if (self.logonViewModel().loggedIn()) {
            self.afterAdd();
        }
    };
}