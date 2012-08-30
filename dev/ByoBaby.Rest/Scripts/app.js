/// <reference path="_references.js" />

/*globals $ document window $ ApplicationViewModel ko*/

$.support.cors = true;

var application;
var utilities;

function initializeViewModel() {
   // utilities = new Helpers();
    application = new ApplicationViewModel('http://localhost/byobabies');
    application.loadLogin(new LogonViewModel(application.baseUrl));
    ko.applyBindings(application);

    if (navigator.network == undefined) {
        application.isProcessing(false);
    }

};

$(document).ready(function () {
    //console.log("onDocumentReady - Listening for the PhoneGap deviceReady event.");
    document.addEventListener("deviceready", onDeviceReady, false);
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


