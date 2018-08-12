import * as moment from "moment";

if (typeof $ !== "undefined" && typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {

    $.validator.addMethod("withinmonthsaftertoday",
        function(value, element, params) {
            let today = moment.utc(params[0]);
            let checkDate = moment.utc(value);

            let months = params[1];
            let maxDate = moment(today).add(months, "months");

            return (checkDate > today && checkDate <= maxDate);
        }
    );

    $.validator.unobtrusive.adapters.add("withinmonthsaftertoday",
        ["today", "months"],
        function (options) {
            options.rules["withinmonthsaftertoday"] = [options.params["today"], options.params["months"]];
            options.messages["withinmonthsaftertoday"] = options.message;
       }
    );

    $.validator.addMethod("withinmonthsafterdate",
        function(value, element, params) {
            let dateField = params[0];
            let startDate = moment.utc(dateField.val());
            let checkDate = moment.utc(value);

            let months = params[1];
            let maxDate = moment(startDate).add(months, "months").add(1, "days");

            return (checkDate > startDate && checkDate <= maxDate);
        }
    );

    $.validator.unobtrusive.adapters.add("withinmonthsafterdate",
        ["propertyname", "months"],
        function (options) {
            let propertyName = options.params["propertyname"];
            let dateField = $(`input[name="${propertyName}"]`);

            if (dateField.length === 0) {
                throw new Error(`Cannot find input field ${propertyName}.`);
            }

            options.rules["withinmonthsafterdate"] = [dateField, options.params["months"]];
            options.messages["withinmonthsafterdate"] = options.message;
       }
    );
}
