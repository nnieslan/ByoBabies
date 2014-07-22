function RequestViewModel(svcUrl, data) {
    NavViewModel.apply(this, [svcUrl]);

    var self = this;
    //self.button(new BackButtonModel());
    self.template = "requestView";

    ko.mapping.fromJS(data, {}, self);

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };

    self.accept = function () {
        console.log("RequestViewModel.accept() called");
        self.postResponse('accept');
    };

    self.deny = function () {
        console.log("RequestViewModel.deny() called");
        self.postResponse('deny');
    };

    self.postResponse = function (actionValue) {
        console.log("RequestViewModel.postResponse() called");

        if (!ByoBabies.Utilities.checkConnection()) {
            ByoBabies.Utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }
        application.isProcessing(true);

        var url = self.baseUrl + 'api/requests/' + self.Id();
        var jqxhr = $.post(url, '=' + actionValue, function (data) {
            console.log("RequestViewModel.postResponse() - ajax call complete");
            application.back();
            application.loggedInUserProfile().refreshNotifications();
        })
            .error(function (jqxhr, exception) {
                console.log("RequestViewModel.postResponse() - ajax POST errored : " + jqxhr.repsonseText);

                if (jqxhr.status === 401) {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    ByoBabies.Utilities.notifyUser('Unable to respond to request.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };
}
