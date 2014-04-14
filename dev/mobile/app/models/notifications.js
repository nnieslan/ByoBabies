
function NotificationsViewModel(data) {
    NavViewModel.apply(this, [data.baseUrl]);

    var self = this;
    //self.button(new BackButtonModel());
    self.template = "notificationsView";
    self.profile = data;

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };

    self.navigateToOrigin = function (selected) {
        if (selected !== null && selected.OriginatorType === 'FriendRequest') {
            var url = self.baseUrl + 'api/requests/' + selected.OriginatorId;
            var jqxhr = $.get(url, function (data) {
                console.log("NotificationsViewModel.navigateToOrigin() - ajax call for origin request is complete");
                application.navigateTo(new RequestViewModel(self.baseUrl, data));
            })
                .error(function (jqxhr, exception) {
                    if (jqxhr.status === 401) {
                        application.clear();
                        return;
                    }

                    if (jqxhr.responseText !== '') {
                        utilities.notifyUser(jqxhr.responseText, 'Error');
                    } else {
                        utilities.notifyUser('Unable to load notifications.  Please try again later.', 'Error');
                    }
                })
                .complete(function () {
                    application.isProcessing(false);

                });
        }
    };
}
