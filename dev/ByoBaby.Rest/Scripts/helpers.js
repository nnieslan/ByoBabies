/// <reference path="..//_references.js" />

/*globals ko*/

function Helpers() {
    /// <summary>
    /// A helper object for utilities that are application wide.
    /// </summary>


    /// <summary>
    /// Notifies the user of any error messages or alerts.
    /// </summary>
    this.notifyUser = function (message, title) {
        if (navigator != undefined && navigator.notification != undefined) {
            navigator.notification.alert(message, function () { }, title);
        } else {
            alert(message);
        }
    };



    /// <summary>
    /// Ensures the application has a data connection.
    /// </summary>
    this.checkConnection = function () {
        if (navigator.network != undefined) { //check for network object existence to ensure we are on a device

            var networkState = navigator.network.connection.type;

            if (networkState == undefined || networkState == Connection.NONE || networkState == Connection.UNKNOWN) {
                return false;
            }
        }
        return true;
    };
};
