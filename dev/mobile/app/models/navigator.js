function NavViewModel() {
  var self = this;

  self.handleMessage = function(msg) {
    console.log("NavViewModel - received msg.");
  };

  //self.onDeviceReady = function() {
    console.log("Initializing a NavViewModel instance");
    var url = window.localStorage.getItem('svcUrl');
    console.log("NavViewModel - baseUrl : " + url);
    self.baseUrl = url;

  //};

  //document.addEventListener("deviceready", function() {
  //  self.onDeviceReady();
  //}, false);
}
