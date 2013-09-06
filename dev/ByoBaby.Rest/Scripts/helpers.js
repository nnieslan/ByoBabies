/// <reference path="..//_references.js" />

/*global ko: false */
/*jslint regexp: true */

function Helpers() {
    /// <summary>
    /// A helper object for utilities that are application wide.
    /// </summary>

    var self = this;

    self.urlParams = {};
    self.notifyUser = function (message, title) {
        if (navigator !== undefined && navigator.notification !== undefined) {
            navigator.notification.alert(message, function () { }, title);
        } else {
            alert(message);
        }
    };



    /// <summary>
    /// Ensures the application has a data connection.
    /// </summary>
    self.checkConnection = function () {
         //check for network object existence to ensure we are on a device
        if (navigator.network !== undefined) {

            var networkState = navigator.network.connection.type;

            if (networkState === undefined || networkState === Connection.NONE || networkState === Connection.UNKNOWN) {
                return false;
            }
        }
        return true;
    };

    self.ensureTemplates = function (list, loaded) {
        var loadedTemplates = [];
        ko.utils.arrayForEach(list, function (name) {
            $.get("Templates/" + name + ".html", function (template) {
                $("body").append("<script id=\"" + name + "\" type=\"text/x-jquery-tmpl\">" + template + "<\/script>");
                loadedTemplates.push(name);
                if (list.length === loadedTemplates.length) {
                    loaded();
                }
            });
        });
    };

    self.parseUrlQueryString = function () {
        var match,
            pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = window.location.search.substring(1);
        while (match = search.exec(query)) {
            self.urlParams[decode(match[1])] = decode(match[2]);
        }
    };
}
