import * as $ from "jquery";
import "jquery-validation";
import "jquery-validation-unobtrusive";
import * as moment from "moment";

$.validator.addMethod("WithinMonthsAfterToday",
    function(value, element, params) {
        let today = moment.utc(params[0]);
        let checkDate = moment.utc(value);

        let months = params[1];
        let maxDate = moment(today).add(months, "months");

        return (checkDate > today && checkDate <= maxDate);
    }
);

$.validator.unobtrusive.adapters.add("WithinMonthsAfterToday",
    ["today", "months"],
    function (options) {
        options.rules["WithinMonthsAfterToday"] = [options.params["today"], options.params["months"]];
        options.messages["WithinMonthsAfterToday"] = options.message;
    }
);

$.validator.addMethod("WithinMonthsAfterDate",
    function(value, element, params) {
        let dateField = params[0];
        let startDate = moment.utc(dateField.val());
        let checkDate = moment.utc(value);

        let months = params[1];
        let maxDate = moment(startDate).add(months, "months").add(1, "days");

        return (checkDate > startDate && checkDate <= maxDate);
    }
);

$.validator.unobtrusive.adapters.add("WithinMonthsAfterDate",
    ["propertyname", "months"],
    function (options) {
        let propertyName = options.params["propertyname"];
        let dateField = $(`input[name="${propertyName}"]`);

        if (dateField.length === 0) {
            throw new Error(`Cannot find input field ${propertyName}.`);
        }

        options.rules["WithinMonthsAfterDate"] = [dateField, options.params["months"]];
        options.messages["WithinMonthsAfterDate"] = options.message;
    }
);

