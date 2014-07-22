$.support.cors = true;


(function($) {
  if (!window.ByoBabies) window.ByoBabies = {};

  function Helpers() {

    console.log('ByoBabies.Utilities - initializing helpers');
    var self = this;

    self.notifyUser = function(message, title) {
      console.log('notifying user : ' + message);
      if (navigator !== undefined && navigator.notification !== undefined) {
        navigator.notification.alert(message, function() {}, title);
      } else {
        alert(message);
      }
    };

    self.checkConnection = function() {
      console.log('verifying network connection in phoneGap');
      //check for network object existence to ensure we are on a device
      if (navigator.network !== undefined) {

        var networkState = navigator.network.connection.type;

        if (networkState === undefined || networkState === Connection.NONE ||
          networkState === Connection.UNKNOWN) {
          return false;
        }
      }
      return true;
    };

    self.ensureTemplates = function(list, loaded) {
      var loadedTemplates = [];
      ko.utils.arrayForEach(list, function(name) {
        $.get("Templates/" + name + ".html", function(template) {
          $("body").append("<script id=\"" + name +
            "\" type=\"text/x-jquery-tmpl\">" + template + "<\/script>");
          loadedTemplates.push(name);
          if (list.length === loadedTemplates.length) {
            loaded();
          }
        });
      });
    };

    self.parseUrlQueryString = function(url) {

      var match,
        queryParams = {},
        pl = /\+/g, // Regex for replacing addition symbol with a space
        search = /([^&#=]+)=?([^&#]*)/g,
        decode = function(s) {
          return decodeURIComponent(s.replace(pl, " "));
        },
        query = url.split(/\?/)[1];
      match = search.exec(query);
      while (match) {
        queryParams[decode(match[1])] = decode(match[2]);
        match = search.exec(query);
      }
      return queryParams;
    };
  };
  window.ByoBabies.Utilities = new Helpers();
})(jQuery);


(function($) {
  $.getAntiForgeryToken = function(tokenWindow, appPath) {
    // HtmlHelper.AntiForgeryToken() must be invoked to print the token.
    tokenWindow = tokenWindow && typeof tokenWindow === typeof window ?
      tokenWindow : window;

    appPath = appPath && typeof appPath === "string" ? "_" + appPath.toString() :
      "";
    // The name attribute is either __RequestVerificationToken,
    // or __RequestVerificationToken_{appPath}.
    var tokenName = "__RequestVerificationToken" + appPath;

    // Finds the <input type="hidden" name={tokenName} value="..." /> from the specified window.
    // var inputElements = tokenWindow.$("input[type='hidden'][name=' + tokenName + "']");
    var inputElements = tokenWindow.document.getElementsByTagName("input");
    for (var i = 0; i < inputElements.length; i++) {
      var inputElement = inputElements[i];
      if (inputElement.type === "hidden" && inputElement.name === tokenName) {
        return {
          name: tokenName,
          value: inputElement.value
        };
      }
    }
  };

  $.appendAntiForgeryToken = function(data, token) {
    // Converts data if not already a string.
    if (data && typeof data !== "string") {
      data = $.param(data);
    }

    // Gets token from current window by default.
    token = token ? token : $.getAntiForgeryToken(); // $.getAntiForgeryToken(window).

    data = data ? data + "&" : "";
    // If token exists, appends {token.name}={token.value} to data.
    return token ? data + encodeURIComponent(token.name) + "=" +
      encodeURIComponent(token.value) : data;
  };

  // Wraps $.post(url, data, callback, type) for most common scenarios.
  $.postAntiForgery = function(url, data, callback, type) {
    return $.post(url, $.appendAntiForgeryToken(data), callback, type);
  };

  // Wraps $.ajax(settings).
  $.ajaxAntiForgery = function(settings) {
    // Supports more options than $.ajax():
    // settings.token, settings.tokenWindow, settings.appPath.
    var token = settings.token ? settings.token : $.getAntiForgeryToken(
      settings.tokenWindow, settings.appPath);
    settings.data = $.appendAntiForgeryToken(settings.data, token);
    return $.ajax(settings);
  };
})(jQuery);
