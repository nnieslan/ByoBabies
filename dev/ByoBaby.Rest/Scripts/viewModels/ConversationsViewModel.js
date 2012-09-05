/// <reference path="..//_references.js" />

/*globals ko*/

function ConversationsViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the graph of conversation threads
    /// </summary>

    //the application API web service Url
    this.baseUrl = svcUrl;

    //the default view for the application, shown when a user is not logged in.
    this.template = "conversationsView";

    /// <summary>
    /// An observable array containing the loaded conversation threads.
    /// </summary>
    self.conversations = ko.observableArray([]);

    self.getConversations = function () {

        application.isProcessing(true);
        var url = self.baseUrl + '/api/conversations';
        
        var jqxhr = $.get(url, function (data) {
            self.conversations(data);
        })
        .error(function (jqxHR, exception) {
            application.isProcessing(false);
            if (jqxHR.responseText != '') {
                utilities.notifyUser(jqxHR.responseText, 'Error');
            } else {
                utilities.notifyUser('Unable to fetch conversations.  Please try again later.', 'Error');
            }
        })
        .complete(function () {
        });

    };

    self.getMyConversations = function () {

        application.isProcessing(true);
        var url = self.baseUrl + '/api/' + application.user() + '/conversations';

        var jqxhr = $.get(url, function (data) {
            self.conversations(data);
        })
        .error(function (jqxHR, exception) {
            application.isProcessing(false);
            if (jqxHR.responseText != '') {
                utilities.notifyUser(jqxHR.responseText, 'Error');
            } else {
                utilities.notifyUser('Unable to fetch conversations.  Please try again later.', 'Error');
            }
        })
        .complete(function () {
        });

    };
};