function CheckinViewModel(baseUrl) {
    NavViewModel.apply(this, [baseUrl]);

    var self = this;
    //self.button(new MenuButtonModel());
    self.template = "checkinView";
    self.latitude = ko.observable(39.7561387); //TODO = get from PhoneGap
    self.longitude = ko.observable(-104.9272044);
    self.note = ko.observable();
    self.duration = ko.observable();
    self.locations = ko.observableArray([]);
    self.selected = ko.observable();
    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
        self.getNearByLocations();
    };

    self.getNearByLocations = function () {

        if (!ByoBabies.Utilities.checkConnection()) {
            ByoBabies.Utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }
        application.isProcessing(true);

        var url = self.baseUrl + 'api/nearby/getlocations?lat=' + self.latitude() + '&lon=' + self.longitude();
        var jqxhr = $.get(url, function (data) {
            console.log("CheckInViewModel.getNearByLocations() - ajax call complete");
            self.locations([]);
            var i, max = data.length;
            for (i = 0; i < max; i++) {
                self.locations.push(new LocationViewModel(data[i]));
            }
            $('ul').listview('refresh');
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === 401) {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    ByoBabies.Utilities.notifyUser('Unable to load near-by locations.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });
    };

    self.setSelected = function (location) {
        self.selected(location);
        $('#popupCheckin').popup('open');
        $('#popupCheckin-popup').trigger('create');
    };

    self.checkin = function (location) {

        //TODO - validate the form before processing.
        //self.valid(!self.selected.hasError() && !self.password.hasError() && !self.confirmPassword.hasError());
        //if (!self.valid()) {
        //    return false;
        //}

        if (!ByoBabies.Utilities.checkConnection()) {
            ByoBabies.Utilities.notifyUser('No data connection is available. Please try again later.', 'Error');
            return false;
        }

        application.isProcessing(true);
        var url = self.baseUrl + 'api/nearby/checkin';
        var input = {
            Location: self.selected(),
            Duration: self.duration(),
            Note: self.note(),
        };

        var jqxhr = $.post(url, input, function (data) {
            application.back();
        })
            .error(function (jqxHR, exception) {
                application.isProcessing(false);
                if (jqxHR.responseText !== '') {
                    ByoBabies.Utilities.notifyUser(jqxHR.responseText, 'Error');
                    //TODO - we probably need an error case for 'Account exists'
                } else {
                    ByoBabies.Utilities.notifyUser('Unable to check-in.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                $('#popupCheckin').popup('close');
                application.isProcessing(false);
            });
    };

}
