
function FriendsViewModel(data) {
    NavViewModel.apply(this, [data.baseUrl]);

    var self = this;
    self.button(new BackButtonModel());
    self.template = "friendsListView";
    self.profile = data;

    self.afterViewRender = function (elements) {
        var view = '#' + self.template + '-content';
        $(view).trigger('create');
        self.afterAdd();
    };
}
