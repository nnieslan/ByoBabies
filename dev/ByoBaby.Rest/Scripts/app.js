/// <reference path="_references.js" />


$.support.cors = true;

var application;
var utilities;

var views = [
        "welcomeView",
        "profileView",
        "registerView",
        "conversationsView",
        "conversationView",
        "friendsListView",
        "friendDetailView",
        "notificationsView",
        "requestView",
        "nearByView",
        "checkinView"
    ];
function initializeViewModel() {
    utilities = new Helpers();
    console.log("InitializeViewModels - calling utilities.ensureTemplates ");
    utilities.ensureTemplates(views, function () {
        console.log("ensureTemplates().Callback - inititalizing ApplicationViewModel");
        //param is placeholder for web services REST Url (blank when hosted ) 
        application = new ApplicationViewModel('');
        application.loadLogin(new LogonViewModel(application.baseUrl));
        application.logonViewModel().loggedIn(true);
        ko.applyBindings(application);

        if (navigator.network === undefined) {
            application.isProcessing(false);
        }
    });
}

$(document).ready(
    function () {
         //console.log("onDocumentReady - Listening for the PhoneGap deviceReady event.");
         //document.addEventListener("deviceready", onDeviceReady, false);
        initializeViewModel();
    }
);


// Cordova is loaded and it is now safe to make calls Cordova methods
//
function onDeviceReady() {
    console.log("onDeviceReady - Initializing the application and connectivity.");
    initializeViewModel();
    application.isProcessing(false);
    if (!utilities.checkConnection()) {
        var errorMessage = 'No data connection is available. Please try again later.';
        utilities.notifyUser(errorMessage, 'Error');
    }
}


