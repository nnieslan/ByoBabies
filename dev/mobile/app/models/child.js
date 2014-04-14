function ChildViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
}
