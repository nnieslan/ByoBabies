/// <reference path="..//_references.js" />

/*globals ko*/

function TasksViewModel(svcUrl, id) {
    /// <summary>
    /// The view model that manages the application task display and navigation
    /// </summary>

    //the application API web service Url
    this.baseUrl = svcUrl;

    //the default view for the application, shown when a user is not logged in.
    this.template = "homeView";


};