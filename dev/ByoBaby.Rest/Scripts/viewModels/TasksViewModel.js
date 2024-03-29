﻿/// <reference path="..//_references.js" />

/*globals ko*/

function TasksViewModel(svcUrl) {
    /// <summary>
    /// The view model that manages the application task display and navigation
    /// </summary>

    NavViewModel.apply(this, [svcUrl]);

    var self = this;

    //the menu view for the application, shown when a user is logged in and clicks the menu button.
    self.template = "menuView";

    //the observable list of available user tasks
    self.tasks = ko.observableArray([]);

    self.loadTasks = function () {
        console.log("Loading the left nav menu view models");
        self.tasks.push({ DisplayName: 'Profile', value: new ProfileViewModel(new PersonViewModel(self.baseUrl)) });
        self.tasks.push({ DisplayName: 'Check-in', value: new CheckinViewModel(self.baseUrl) });
        self.tasks.push({ DisplayName: 'Near-by', value: new NearByViewModel(self.baseUrl) });
        self.tasks.push({ DisplayName: 'Conversations', value: new ConversationsViewModel(self.baseUrl) });
        
    };

}