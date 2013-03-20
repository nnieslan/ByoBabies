/// <reference path="..//_references.js" />

/*globals ko*/

function ApplicationViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the view model back-stack
    /// </summary>

    var self = this;

    //the application API web service Url
    self.baseUrl = svcUrl;

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

    self.Pages = ko.observableArray([]);

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
        var stacklength = self.viewModelBackStack().length;
        if (stacklength > 0) {
            return self.viewModelBackStack()[stacklength - 1];
        } else {
            return this;
        }

    }, this);

    /// <summary> 
    /// The computed indicator denoting if a back button should be shown.
    /// </summary>
    self.backButtonRequired = ko.dependentObservable(function () {
        return (self.viewModelBackStack().length > 0 && self.currentViewModel().template != 'profileView' && !self.isProcessing() && !self.isComplete());
    }, this);


    /// <summary> 
    /// The computed indicator denoting if a logout button should be shown.
    /// </summary>
    self.logoutButtonRequired = ko.dependentObservable(function () {
        return (self.logonViewModel() != null && self.logonViewModel().loggedIn() && !self.isProcessing() && !self.isComplete());
    }, this);

    //functions

    /// <summary> 
    /// Initializes the LogonViewModel for the application.
    /// </summary>
    self.loadLogin = function (viewModel) {
        self.logonViewModel(viewModel);
        self.logonViewModel().loggedIn.subscribe(function (newValue) {
            if (newValue) {
                var profile = new ProfileViewModel(self.baseUrl);
                self.Pages.push({ DisplayName: 'Profile', value: profile });
                profile.getProfile();
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
        if (clear != undefined && clear == true) {
            self.viewModelBackStack([]);
        }
        self.viewModelBackStack.push(viewModel);
    };

    /// <summary>
    /// Navigates to the TasksViewModel, which backs the homeView.
    /// </summary>
    self.navigateHome = function () {
        self.navigateTo(self.profileViewModel(), true);
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
        self.profileViewModel(new ProfileViewModel(self.baseUrl));
        self.viewModelBackStack([]);
        self.isProcessing(false);
        self.isComplete(false);
    };


    /// <summary> 
    /// Navigates back to the previous view if possible.
    /// </summary>
    self.back = function () {
        self.viewModelBackStack.pop();
        if (self.viewModelBackStack().length == 0 && self.logonViewModel().loggedIn()) {
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
        if (view == '#welcomeView-content') {
            $("#logonForm").submit(function () {
                application.logonViewModel().login();
                return false;
            });
        }
        //refreshing the ui-content div size after the header appears post-login.
        $('#ui-content').trigger('resize');

    }
}