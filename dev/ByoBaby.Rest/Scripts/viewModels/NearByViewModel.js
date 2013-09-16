
function NearByViewModel(baseUrl) {
    NavViewModel.apply(this, [baseUrl]);

    var self = this;
    self.button(new MenuButtonModel());
    self.template = "nearByView";
    self.latitude = ko.observable(39.7561387);
    self.longitude = ko.observable(-104.9272044);
    self.map = null;
    self.searchManager = null;
    self.currentBox = null;
    self.searchTerm = ko.observable('');

    self.initializeMap = function () {
        Microsoft.Maps.loadModule('Microsoft.Maps.Themes.BingTheme', {
            callback: function () {
                self.map = new Microsoft.Maps.Map($('#nearbyMap')[0],
                {
                    credentials: 'AuBiDC9YFcYJr09uJZnkeKb_bflX5EbLUNU7wVJ7E0P414Ptj4kIMq3GqLOQQEB6',
                    mapTypeId: Microsoft.Maps.MapTypeId.road,
                    enableClickableLogo: false,
                    enableSearchLogo: false,
                    center: new Microsoft.Maps.Location(self.latitude(), self.longitude()),
                    zoom: 14,
                    theme: new Microsoft.Maps.Themes.BingTheme()
                });
            }
        });
    };

    self.createMapPin = function (result) {
        if (result) {
            var pin = new Microsoft.Maps.Pushpin(result.location, null);
            Microsoft.Maps.Events.addHandler(pin, 'click', function () {
                self.showPinInfo(result)
            });
            self.map.entities.push(pin);
        }
    };

    self.showPinInfo = function (result) {
        if (self.currentBox) {
            self.currentBox.setOptions({ visible: true });
            self.map.entities.remove(self.currentBox);
        }
        self.currentBox = new Microsoft.Maps.Infobox(
            result.location,
            {
                title: result.name,
                description: [result.address, result.city, result.state,
                  result.country, result.phone].join(' '),
                showPointer: true,
                titleAction: null,
                titleClickHandler: null
            });
        self.currentBox.setOptions({ visible: true });
        self.map.entities.push(self.currentBox);
    }

    self.createSearchManager = function () {
        self.map.addComponent('searchManager', new Microsoft.Maps.Search.SearchManager(self.map));
        self.searchManager = self.map.getComponent('searchManager');
    };

    self.search = function () {
        Microsoft.Maps.loadModule('Microsoft.Maps.Search', { callback: self.searchRequest })
    };

    self.searchRequest = function () {
        self.createSearchManager();
        var request =
            {
                query: self.searchTerm(),
                count: 20,
                startIndex: 0,
                bounds: self.map.getBounds(),
                callback: self.search_onSearchSuccess,
                errorCallback: self.search_onSearchFailure
            };
        self.searchManager.search(request);
    };

    self.search_onSearchSuccess = function (result, userData) {
        self.map.entities.clear();
        var searchResults = result && result.searchResults;
        if (searchResults) {
            for (var i = 0; i < searchResults.length; i++) {
                self.createMapPin(searchResults[i]);
            }
            if (result.searchRegion && result.searchRegion.mapBounds) {
                self.map.setView({ bounds: result.searchRegion.mapBounds.locationRect });
            }
            else {
                alert('No near-by businesses found');
            }
        }
    }

    self.search_onSearchFailure = function (result, userData) {
        alert('Unable to search near-by locations');
    };

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $('#searchTerm').watermark('where are you?');
        $(view).trigger('create');
        self.afterAdd();
        self.initializeMap();
    };


}
