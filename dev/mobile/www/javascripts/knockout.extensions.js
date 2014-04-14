
ko.extenders.required = function (target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    //define a function to do validation
    function validate(newValue) {
        target.hasError(newValue ? false : true);
        target.validationMessage(newValue ? "" : overrideMessage || "This field is required");
    }

    //initial validation
    validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};
 

ko.extenders.areEqual = function (target, comparable) {
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();
    target.comparisonValue = ko.observable(comparable);

    function validate(inputValue) {
        if (inputValue != target.comparisonValue()) {
            target.hasError(true);
            target.validationMessage("The values entered do not match");
        } else {
            target.hasError(false);
        }
    };

    validate(target());

    target.subscribe(validate);

    return target;
};

ko.extenders.greaterThanZero = function (target, overrideMessage) {
    //add some sub-observables to our observable
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    //define a function to do validation
    function validate(newValue) {
        var parsedValue = isNaN(newValue) ? 0 : parseFloat(+newValue);
        target.hasError(parsedValue > 0 ? false : true);
        target.validationMessage(parsedValue > 0 ? "" : overrideMessage || "value must be greater than zero");
    }

    //initial validation
    validate(target());

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
};


ko.extenders.numeric = function (target, precision) {
    //create a writeable computed observable to intercept writes to our observable
    var result = ko.computed({
        read: target,  //always return the original observables value
        write: function (newValue) {
            var current = target(),
                roundingMultiplier = Math.pow(10, precision),
                newValueAsNum = isNaN(newValue) ? 0 : parseFloat(+newValue).toFixed(precision),
                valueToWrite = Math.round(newValueAsNum * roundingMultiplier) / roundingMultiplier;

            //only write if it changed
            if (valueToWrite !== current) {
                target(valueToWrite);
            } else {
                //if the rounded value is the same, but a different value was written, force a notification for the current field
                if (newValue !== current) {
                    target.notifySubscribers(valueToWrite);
                }
            }
        }
    });

    //initialize with current value to make sure it is rounded appropriately
    result(target());

    //return the new computed observable
    return result;
};


ko.bindingHandlers.stopBindings = {
    init: function () {
        return { controlsDescendantBindings: true };
    }
};

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        $(element).toggle(isVisible); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        isVisible ? $(element).fadeIn() : $(element).fadeOut();
    }
};

ko.bindingHandlers.slideVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        $(element).toggle(isVisible);
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slide the element in or out
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        isVisible ? $(element).slideDown() : $(element).slideUp();
    }
};

ko.bindingHandlers.simpleVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        isVisible ? $(element).show() : $(element).hide();
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slide the element in or out
        var value = valueAccessor();
        var isVisible = ko.utils.unwrapObservable(value);
        if (isVisible) {
            $(element).remove('bb-hide');
        }
        isVisible ? $(element).show() : $(element).hide();
    }
};

ko.bindingHandlers.lightboxVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slide the element in or out
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value)) {
            $(element).lightbox_me({ centered: true, closeClick: false });
        } else {
            setTimeout(function () {
                $(element).trigger('close');
                $(element).css('display', 'none');
            }, 310);
        }
    }
};

ko.bindingHandlers.fadeMessage = {
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value) !== "") {
            $(element).html(value()).show();
            window.setTimeout(function () {
                $(element).fadeOut();
                value("");
            }, 3000);
        }
    }
};
