/// <reference path="_references.js" />

/*globals $ document window $ ApplicationViewModel ko*/

$.support.cors = true;

var application;
var utilities;

var views = ["welcomeView", "profileView", "registerView", "conversationsView", "conversationView", "friendsListView", "friendDetailView"];
function initializeViewModel() {
    utilities = new Helpers();
    console.log("InitializeViewModels - calling utilities.ensureTemplates ");
    utilities.ensureTemplates(views, function () {
        console.log("ensureTemplates().Callback - inititalizing ApplicationViewModel");
        application = new ApplicationViewModel(''); //param is placeholder for web services REST Url (when hosted in web app, it's just blank)
        application.loadLogin(new LogonViewModel(application.baseUrl));
        application.logonViewModel().loggedIn(true);
        ko.applyBindings(application);

        if (navigator.network == undefined) {
            application.isProcessing(false);
        }
    });
    

};

$(document).ready(function () {
    //console.log("onDocumentReady - Listening for the PhoneGap deviceReady event.");
    //document.addEventListener("deviceready", onDeviceReady, false);
    initializeViewModel();
});


// Cordova is loaded and it is now safe to make calls Cordova methods
//
function onDeviceReady() {
    //console.log("onDeviceReady - Initializing the ApplicationViewModel and ensuring connectivity.");
    initializeViewModel();
    application.isProcessing(false);
    //if (!utilities.checkConnection()) {
    //    utilities.notifyUser('No data connection is available. You will not be able to use ByoBabies at this time.', 'Error');
    //}
}


