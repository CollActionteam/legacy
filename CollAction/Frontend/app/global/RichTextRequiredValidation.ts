if (typeof $ !== "undefined" && typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {

    $.validator.setDefaults({
        ignore: ".ql-editor"
    });

    $.validator.addMethod("richtextrequired",
        function(value, element, params) {
            let text = $(value).text().trim();
            return (text !== "");
        }
    );

    $.validator.unobtrusive.adapters.add("richtextrequired",
        function (options) {
            options.rules["richtextrequired"] = [];
            options.messages["richtextrequired"] = options.message;

            $(options.element).on("change", function () {
                // Manually trigger validation on change event
                $(options.form).validate().element(this);
            });
        }
    );
}
