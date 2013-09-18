﻿
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
    self.locations = ko.observableArray([]);

    self.auth = {
        bingKey: "AuBiDC9YFcYJr09uJZnkeKb_bflX5EbLUNU7wVJ7E0P414Ptj4kIMq3GqLOQQEB6",
        consumerKey: "urh0CQQyRtrG7Li6ro-faA",
        consumerSecret: "o1Yb0CipArdBmqhCs1Fq_OGVSMM",
        accessToken: "2CJWkwJFmh1vksIVoVL9XtAQZv7VSNZM",
        // This example is a proof of concept, for how to use the Yelp v2 API with javascript.
        // You wouldn't actually want to expose your access token secret like this in a real application.
        accessTokenSecret: "ZjVjpt2A_3dhNEC4GmB-UyfWZMs",
        serviceProvider: {
            signatureMethod: "HMAC-SHA1"
        }
    };

    self.getNearByLocations = function () {

        if (!utilities.checkConnection()) {
            utilities.notifyUser('No data connection is available. Please try again later.', function () { }, 'Error');
            return false;
        }

        var url = self.baseUrl + 'api/locations?lat=' + self.latitude() + '&lon=' + self.longitude();
        var jqxhr = $.get(url, function (data) {
            console.log("NearByViewModel.getNearByLocations() - ajax call complete");
            self.locations([]);
            var i, max = data.length;
            for (i = 0; i < max; i++) {
                self.locations.push(data[i]);
                self.createMapPin(data[i]);
            }
        })
            .error(function (jqxhr, exception) {
                if (jqxhr.status === 401) {
                    application.clear();
                    return;
                }

                if (jqxhr.responseText !== '') {
                    utilities.notifyUser(jqxhr.responseText, 'Error');
                } else {
                    utilities.notifyUser('Unable to load near-by locations.  Please try again later.', 'Error');
                }
            })
            .complete(function () {
                application.isProcessing(false);

            });

        //Client-side YELP API example
        //var accessor = {
        //    consumerSecret: self.auth.consumerSecret,
        //    tokenSecret: self.auth.accessTokenSecret
        //};

        //parameters = [];
        ////parameters.push(['term', terms]);
        //parameters.push(['radius_filter', 5000]);
        //parameters.push(['ll', self.latitude() + ',' + self.longitude()]);
        //parameters.push(['callback', 'cb']);
        //parameters.push(['oauth_consumer_key', self.auth.consumerKey]);
        //parameters.push(['oauth_consumer_secret', self.auth.consumerSecret]);
        //parameters.push(['oauth_token', self.auth.accessToken]);
        //parameters.push(['oauth_signature_method', 'HMAC-SHA1']);

        //var message = {
        //    'action': 'http://api.yelp.com/v2/search',
        //    'method': 'GET',
        //    'parameters': parameters
        //};

        //OAuth.setTimestampAndNonce(message);
        //OAuth.SignatureMethod.sign(message, accessor);

        //var parameterMap = OAuth.getParameterMap(message.parameters);
        //parameterMap.oauth_signature = OAuth.percentEncode(parameterMap.oauth_signature)
        //console.log(parameterMap);

        //$.ajax({
        //    'url': message.action,
        //    'data': parameterMap,
        //    'cache': true,
        //    'dataType': 'jsonp',
        //    'jsonpCallback': 'cb',
        //    'success': function (data, textStats, XMLHttpRequest) {

        //        for (var i = 0; i < data.businesses.length; i++) {
        //            var b = data.businesses[i];
        //            console.log(b);
        //            self.createMapPin(b);
        //        }
        //    }
        //});
    };
 
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
            //var geocode = 'http://dev.virtualearth.net/REST/v1/Locations?key=' + self.auth.bingKey + '&query=' + encodeURIComponent(result.location.display_address.join(','))
            //console.log(geocode);
            //$.get(geocode, function (data) {
            //        console.log(data);
            //});
            var loc = new Microsoft.Maps.Location(result.Latitude, result.Longitude);
            var pin = new Microsoft.Maps.Pushpin(loc, null);
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
                title: result.Name,
                description: [result.PhoneNumber, result.Address].join(','),
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
