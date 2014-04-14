function TasksViewModel() {
  NavViewModel.apply(this);

  var self = this;

  //the menu view for the application, shown when a user is logged in and clicks the menu button.
  self.template = "menuView";

  //the observable list of available user tasks
  self.tasks = ko.observableArray([]);

  self.loadTasks = function() {
    console.log("Loading the left nav menu view models");
    self.tasks.push(new TaskViewModel({
      name: 'Profile',
      id: 'profile',
      url: '/views/profile/index.html'
    }));
    self.tasks.push(new TaskViewModel({
      name: 'Check-in',
      id: 'checkin',
      url: '/views/checkin/index.html'
    }));
    self.tasks.push(new TaskViewModel({
      name: 'Near-by',
      id: 'nearby',
      url: '/views/nearby/index.html'
    }));
    self.tasks.push(new TaskViewModel({
      name: 'Conversations',
      id: 'conversations',
      url: '/views/conversations/index.html'
    }));

  };
}

function TaskViewModel(config) {
  var self = this;
  self.displayName = ko.observable(config.name || '');
  self.id = config.id || '';
  self.url = config.url || '';
  self.select = function() {
    self.notify();
  }

  self.notify = function() {
    var msg = {
      src: 'drawer',
      selection: {
        id: self.id,
        url: self.url
      }
    };
    console.log('notifying background of drawer selection - ' + self.id)
    window.postMessage(msg, '*');
    steroids.drawers.hideAll();
  };

}
