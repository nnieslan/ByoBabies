function GroupViewModel(data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
}
