
function LocationViewModel(data) {
    console.log("LocationViewModel - initializing instance");

    var self = this;
    ko.mapping.fromJS(data, {}, self);
}
