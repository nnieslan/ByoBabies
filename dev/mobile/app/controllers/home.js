//home controller


console.log("Executing the home view controller");


var vm = new HomeViewModel();

document.addEventListener("DOMContentLoaded", function() {
  console.log("Binding the home view to knockout viewmodel");
  ko.applyBindings(vm);
});

steroids.view.navigationBar.show("byobabies");

console.log(
  "Pre-loading a background controller webview to host ajax and webview preloads"
);
var bgWebView = new steroids.views.WebView({
  location: "/views/background/index.html",
  id: "bgView"
});
bgWebView.preload();
