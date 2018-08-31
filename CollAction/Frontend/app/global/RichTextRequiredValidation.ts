if (typeof $ !== "undefined" && typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {

    $.validator.setDefaults({
        ignore: ".ql-editor"
    });

    $.validator.addMethod("RichTextRequired",
        function(value, element, params) {
            let text = $(value).text().trim();
            return (text !== "");
        }
    );

    $.validator.unobtrusive.adapters.add("RichTextRequired",
        function (options) {
            options.rules["RichTextRequired"] = [];
            options.messages["RichTextRequired"] = options.message;

            $(options.element).on("change", function () {
                // Manually trigger validation on change event
                $(options.form).validate().element(this);
            });
        }
    );
}
