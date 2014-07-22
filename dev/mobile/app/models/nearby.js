function NearByViewModel() {
  console.log("initializing an instance of a NearByViewModel");
  NavViewModel.apply(this);

  var self = this;
  self.template = "nearByView";
  self.latitude = ko.observable(39.7561387);
  self.longitude = ko.observable(-104.9272044);
  self.map = null;
  self.searchManager = null;
  self.currentBox = null;
  self.currentPin = ko.observable('');
  self.searchTerm = ko.observable('');
  self.checkins = ko.observableArray([]);

  self.auth = {
    bingKey: "AuBiDC9YFcYJr09uJZnkeKb_bflX5EbLUNU7wVJ7E0P414Ptj4kIMq3GqLOQQEB6"
  };

  self.getNearByCheckins = function(callback) {
    console.log("NearByViewModel.getNearByCheckins() - beginning ajax call");
    if (!ByoBabies.Utilities.checkConnection()) {
      ByoBabies.Utilities.notifyUser(
        'No data connection is available. Please try again later.', function() {},
        'Error');
      return false;
    }

    var url = self.baseUrl + 'api/nearby/getcheckins?lat=' + self.latitude() +
      '&lon=' + self.longitude() + '&radius=4000';
    var jqxhr = $.get(url, function(data) {
      console.log("NearByViewModel.getNearByCheckins() - ajax call complete");
      self.checkins([]);
      var i, max = data.length;
      for (i = 0; i < max; i++) {
        self.checkins.push(new LocationViewModel(data[i]));
      }
      if (callback !== undefined) {
        callback();
      }
    })
      .error(function(jqxhr, exception) {
        if (jqxhr.status === 401) {
          //TODO - handle 401 unauthorized globally
          return;
        }

        if (jqxhr.responseText !== '') {
          ByoBabies.Utilities.notifyUser(jqxhr.responseText, 'Error');
        } else {
          ByoBabies.Utilities.notifyUser(
            'Unable to load near-by check-ins.  Please try again later.',
            'Error');
        }
      })
      .complete(function() {});

  };

  self.initializeMap = function() {
    console.log('NearByViewModel - calling initializeMap()');
    Microsoft.Maps.loadModule('Microsoft.Maps.Themes.BingTheme', {
      callback: function() {
        self.map = new Microsoft.Maps.Map($('#nearbyMap')[0], {
          credentials: self.auth.bingKey,
          mapTypeId: Microsoft.Maps.MapTypeId.road,
          enableClickableLogo: false,
          enableSearchLogo: false,
          showMapTypeSelector: false,
          showScaleBar: false,
          showDashboard: false,
          tileBuffer: 2,
          center: new Microsoft.Maps.Location(self.latitude(), self.longitude()),
          zoom: 14,
          theme: new Microsoft.Maps.Themes.BingTheme()
        });
      }
    });
  };

  self.createMapPin = function(result) {
    if (result) {
      var loc = new Microsoft.Maps.Location(result.Checkin.Location.Latitude(),
        result.Checkin.Location.Longitude());
      var pin = new Microsoft.Maps.Pushpin(loc, null);
      Microsoft.Maps.Events.addHandler(pin, 'click', function() {
        self.showPinInfo(result)
      });
      self.map.entities.push(pin);
    }
  };

  self.showPinInfo = function(result) {
    if (self.currentBox) {
      self.currentBox.setOptions({
        visible: true
      });
      self.map.entities.remove(self.currentBox);
    }
    self.currentPin(result);
    var infoBoxHtml =
      '<div id="selectedMapPin" data-bind="template: { name: &quot;mapPinInfoView&quot;, data: $data }"></div>';
    //'<div id="selectedMapPin" class="pushpin-infobox"><b id="infoboxTitle" data-bind="text: Checkin.Owner.FirstName()">'
    //+ '</b><a id="infoboxDescription">Checked in at <span data-bind="text: Checkin.DisplayStartTime()"></span>'
    //+ "Checked in at " + result.Checkin.DisplayStartTime() + " for an estimated " +  result.Checkin.Duration() + " minutes."
    //+'</a></div>'
    self.currentBox = new Microsoft.Maps.Infobox(
      new Microsoft.Maps.Location(result.Checkin.Location.Latitude(), result.Checkin
        .Location.Longitude()), {
        //title: [result.Checkin.Owner.FirstName(), result.Checkin.Owner.LastName()].join(' '),
        //description: "Checked in at " + result.Checkin.DisplayStartTime() + " for an estimated " +  result.Checkin.Duration() + " minutes.",
        showPointer: true,
        titleAction: null,
        titleClickHandler: null,
        htmlContent: infoBoxHtml
      });

    self.currentBox.setOptions({
      visible: true
    });
    self.map.entities.push(self.currentBox);
    ko.applyBindings(result, $('#selectedMapPin')[0]);
    $('#mapPinInfoView-content').trigger('create');
  }

  //TODO - create a KNOCKOUT binding for this
  self.renderPins = function() {
    self.map.entities.clear();
    if (self.checkins().length > 0) {
      for (var i = 0; i < self.checkins().length; i++) {
        self.createMapPin(self.checkins()[i]);
      }
    }
  };

  self.afterViewRender = function(elements) {
    var view = '#' + self.template + '-content';
    $(view).trigger('create');
  };

  self.initialize = function() {
    console.log("NearByViewModel - calling initialize()");
    self.initializeMap();
    self.getNearByCheckins(self.renderPins);
  };
}
