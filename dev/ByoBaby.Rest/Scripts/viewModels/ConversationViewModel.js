/// <reference path="..//_references.js" />

/*globals ko*/

function ConversationViewModel(svcUrl, id) {
    /// <summary>
    /// The view model that manages a detail view of a conversation
    /// </summary>

    //the application API web service Url
    this.baseUrl = svcUrl;

    //the detaile view of a conversation.
    this.template = "conversationView";


};