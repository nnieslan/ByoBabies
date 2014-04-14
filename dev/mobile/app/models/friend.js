function FriendViewModel(data) {

    function F() { }
    F.prototype = data;
    F.prototype.template = "friendDetailView";
    var retval = new F();

    return retval;
}
