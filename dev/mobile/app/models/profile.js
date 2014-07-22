function ProfileViewModel(data) {
  function P() {}
  P.prototype = data;
  P.prototype.template = "profileView";

  var self = new P();
  self.ShowUploader = ko.observable(false);
  self.Notifications = ko.observableArray([]);

  self.valid = ko.observable(true);

  self.fullname = ko.computed(function() {
    if (self.FirstName !== undefined && self.LastName !== undefined) {
      return self.FirstName() + ' ' + self.LastName();
    }
    return '';
  }, self);

  self.handleMessage = function(msg){
    console.log('BackgroundViewModel - Message received from ' + msg.data.src);
    var source = msg.data.src || '',
        action = msg.data.action || '';
    if (source === 'profile' && action === 'loaded') {
      var data = window.localStorage.getItem('credentials');
      if (data) {
        var json = JSON.parse(data);
        self.update(data);
        if (self.State !== undefined && self.State() !== '') {
          ko.utils.arrayForEach(self.availableStates, function(value) {
            if (value.Code === self.State()) {
              self.selectedState(value);
            }
          });
        }
        self.selectedState.subscribe(function(newValue) {
          if (newValue && newValue.Code !== self.State()) {
            self.State(newValue.Code);
          }
        });
      }
    }
  };

  window.addEventListener("message", self.handleMessage);

  self.saveProfile = function() {

    console.log("ProfileViewModel.saveProfile() called");

    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', function() {},
        'Error');
      return false;
    }
    application.isProcessing(true);

    var url = self.baseUrl + 'api/' + self.Id() + '/profile';

    var input = ko.mapping.toJS(self);
    if (input.Children === null) {
      input.Children = [];
    }
    if (input.MemberOf === null) {
      input.MemberOf = [];
    }
    if (input.Interests === null) {
      input.Interests = [];
    }
    //if (input.Friends == null) { input.Friends = []; }
    //if (input.Notifications == null) { input.Notifications = []; }
    var jqxhr = $.post(url, input, function(data) {
      console.log("ProfileViewModel.saveProfile() - ajax call complete");
      ko.mapping.fromJS(data, self);

    })
      .error(function(jqxhr, exception) {
        console.log("ProfileViewModel.saveProfile() - ajax POST errored : " +
          jqxhr.repsonseText);

        if (jqxhr.status === 401) {
          application.clear();
          return;
        }

        if (jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to load your profile.  Please try again later.', 'Error');
        }
      })
      .complete(function() {
        application.isProcessing(false);

      });
  };

  self.afterViewRender = function(elements) {
    console.log("ProfileViewModel.afterViewRender() called");

    $('#mobilePhone').watermark('phone number');
    $('#neighborhood').watermark('neighborhood');
    $('#state').watermark('state');
    $('#city').watermark('city');

    //refreshing jquery themes post view render via .trigger().
    //This is due to timing of knockout template rendering.  To accomplish
    //this I had to put a base div in each template with an id based
    //on the template name itself so I could ensure only the current
    //template is themed.
    var view = '#' + application.currentViewModel().template + '-content';
    var list = '#' + application.currentViewModel().template + '-list';
    $(view).trigger('create');

    //refreshing the ui-content div size after the header appears post-login.
    $('#ui-content').trigger('resize');

    self.afterAdd();
    //$(list).listview("refresh");
    //$(form).submit(function () {
    //    self.saveProfile();
    //    return false;
    //});
  };

  self.toggleUploader = function() {
    self.ShowUploader(!self.ShowUploader());
  };

  self.refreshNotifications = function(navigationActionCallback) {
    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', function() {},
        'Error');
      return false;
    }

    var url = self.baseUrl + 'api/notifications';
    var jqxhr = $.get(url, function(data) {
      console.log("ProfileViewModel.viewNotifications() - ajax call complete");
      self.Notifications([]);
      var i, max = data.length;
      for (i = 0; i < max; i++) {
        self.Notifications.push(data[i]);
      }
      if (navigationActionCallback !== undefined && navigationActionCallback !==
        null) {
        navigationActionCallback();
      }
    })
      .error(function(jqxhr, exception) {
        if (jqxhr.status === 401) {
          application.clear();
          return;
        }

        if (jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to load notifications.  Please try again later.', 'Error'
          );
        }
      })
      .complete(function() {
        application.isProcessing(false);

      });
  };

  self.viewNotifications = function() {
    self.refreshNotifications(function() {
      application.navigateTo(new NotificationsViewModel(self));
    });
  };

  self.viewFriends = function() {
    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', function() {},
        'Error');
      return false;
    }

    var url = self.baseUrl + 'api/' + self.Id() + '/friends';
    var jqxhr = $.get(url, function(data) {
      console.log("ProfileViewModel.viewFriends() - ajax call complete");
      self.Friends([]);
      var i, max = data.length;
      for (i = 0; i < max; i++) {
        var current = new PersonViewModel(self.baseUrl, data[i]);
        self.Friends.push(new FriendViewModel(current));
      }
      application.navigateTo(new FriendsViewModel(self));
    })
      .error(function(jqxhr, exception) {
        if (jqxhr.status === 401) {
          application.clear();
          return;
        }

        if (jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to load your friends.  Please try again later.', 'Error');
        }
      })
      .complete(function() {
        application.isProcessing(false);

      });
  };

  self.addChild = function() {

    console.log("Adding new child");
    var list = '#profileView-childrenlist';
    self.Children.push(new ChildViewModel({
      'Id': null,
      'Name': '',
      'Age': '',
      'Gender': ''
    }));
    //redraw the list to show the new child correctly.
    $(list).trigger('create');
    $(list).listview("refresh");

  };

  self.removeChild = function(value) {
    console.log("Removing child : " + value.Id);
    //use mapped Remove in the case of an existing child
    if (value.Id() !== null && value.Id() !== '') {
      self.Children.mappedRemove(value);
    } else {
      var index = ko.utils.arrayIndexOf(self.Children(), value);
      self.Children.remove(self.Children()[index]);
    }
  };

  return self;
}
