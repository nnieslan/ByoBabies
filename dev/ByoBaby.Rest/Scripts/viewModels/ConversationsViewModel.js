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


};