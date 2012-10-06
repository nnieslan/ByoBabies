/// <reference path="..//_references.js" />

/*globals ko*/

function ApplicationViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the view model back-stack
    /// </summary>

    //the application API web service Url
    this.baseUrl = svcUrl;

    //the default view for the application, shown when a user is not logged in.
    this.template = "welcomeView";

    /// <summary> 
    /// The observable view model managing user logon state and user profile.
    /// </summary>
    this.logonViewModel = ko.observable();

    /// <summary> 
    /// The observable indicator used to toggle progress bars.
    /// </summary>
    this.isProcessing = ko.observable(true);

    /// <summary> 
    /// The observable indicator user to toggle donation wizard UI status.
    /// </summary>
    this.isComplete = ko.observable(false);

    /// <summary> 
    /// The observable array managing the UI navigation back stack for the application.
    /// </summary>
    this.viewModelBackStack = ko.observableArray();


    /// <summary> 
    /// The computed current view model to display in the UI.
    /// </summary>
    this.currentViewModel = ko.computed(function () {
        var stacklength = this.viewModelBackStack().length;
        if (stacklength > 0) {
            return this.viewModelBackStack()[stacklength - 1];
        } else {
            return this;
        }

    }, this);

    /// <summary> 
    /// The computed indicator denoting if a back button should be shown.
    /// </summary>
    this.backButtonRequired = ko.dependentObservable(function () {
        return (this.viewModelBackStack().length > 0 && this.currentViewModel().template != 'homeView' && !this.isProcessing() && !this.isComplete());
    }, this);


    /// <summary> 
    /// The computed indicator denoting if a logout button should be shown.
    /// </summary>
    this.logoutButtonRequired = ko.dependentObservable(function () {
        return (this.logonViewModel() != null && this.logonViewModel().loggedIn() && !this.isProcessing() && !this.isComplete());
    }, this);

    //functions

    /// <summary> 
    /// Initializes the LoginViewModel for the application.
    /// </summary>
    this.loadLogin = function (viewModel) {
        this.logonViewModel(viewModel);
    };

    /// <summary> 
    /// Navigates to the viewModel indicated, optionally clearing back stack history.
    /// </summary>
    this.navigateTo = function (viewModel, clear) {
        if (clear != undefined && clear == true) {
            this.viewModelBackStack([]);
        }
        this.viewModelBackStack.push(viewModel);
    };

    /// <summary>
    /// Navigates to the TasksViewModel, which backs the homeView.
    /// </summary>
    this.navigateHome = function () {
        this.navigateTo(new TasksViewModel(), true);
    };

    /// <summary>
    /// In the event of a HTTP 401, we need to clear the application.
    /// </summary>
    this.clear = function () {
        this.logonViewModel().profile(null);
        this.viewModelBackStack([]);
        this.isProcessing(false);
        this.isComplete(false);
    };


    /// <summary> 
    /// Navigates back to the previous view if possible.
    /// </summary>
    this.back = function () {
        this.viewModelBackStack.pop();
        if (this.viewModelBackStack().length == 0 && this.logonViewModel().loggedIn()) {
            this.viewModelBackStack.push(new TasksViewModel());
        }
    };

    /// <summary> 
    /// Fetches the view to display for the current viewModel.
    /// </summary>
    this.templateSelector = function (viewModel) {
        return viewModel.template;
    };

    this.afterViewRender = function (elements) {
        //TODO - figure out how to apply this functionality inside the view models.
        //$('#username').watermark('username');
        //$('#pw').watermark('password');
        //$('#amount').watermark('amount');

        //refreshing jquery themes post view render via .trigger().
        //This is due to timing of knockout template rendering.  To accomplish
        //this I had to put a base div in each template with an id based
        //on the template name itself so I could ensure only the current
        //template is themed.
        var view = '#' + application.currentViewModel().template + '-content';
        $(view).trigger('create');
        //if (view == '#donateView-content') {
        //    $("#donateForm").submit(function () {
        //        application.currentViewModel().submit();
        //        return false;
        //    });
        //}
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