;
(function ($, $validator) {
    
    $validator = $validator || $.validator;
    if ($validator) {
        // Create a custom validator rule for our server errors that are just going to ensure that a different value is used
        $validator.addMethod("serverError", function (value, element) {
            return this.optional(element) || $(element).data("serverError-value") != value;
        }, $validator.format("{0} is not a valid value"));
    }

    // Add a flag for ajax errors to make global identification of them easier
    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        // Based on: http://lithostech.com/2011/04/jquery-deferreds-and-the-jquery-promise-method/
        var jqXHRWrapper = $.Deferred(function (defer) {
            jqXHR
                .done(defer.resolve)
                .fail(function (jqXHR, status, thrownError) {
                    jqXHR.isResponseValidationErrors = false;
                    if (thrownError === "Bad Request" && jqXHR.responseJSON && jqXHR.responseJSON.TypeName === "ValidationErrors") {
                        jqXHR.isResponseValidationErrors = true;
                    }
                    defer.rejectWith(this, [jqXHR, status, thrownError])
                })
            ;
        });

        // Impose our wrapper on the jqXHR
        jqXHRWrapper.promise(jqXHR);

        // For compatibility, map done/fail onto success/error handlers.
        jqXHR.success = jqXHR.done;
        jqXHR.error = jqXHR.fail;
    });

    // Add a global ajax error handler to deal with validation errors
    $(document).ajaxError(function (event, jqXHR, settings, thrownError) {
        if (jqXHR.isResponseValidationErrors === true && jqXHR.responseJSON) {
            var errors = $(jqXHR.responseJSON.Value);
            errors.each(function () {
                var that = this;
                $('[name="' + that.Key + '"]').each(function () {

                    // Store the field value that caused the error
                    $(this).data("serverError-value", $(this).val());

                    // Add the rule to this specific field
                    $(this).rules('add', {
                        serverError: true,
                        messages: {
                            serverError: that.Message
                        }
                    });

                    // Trigger the validation for this field immediately
                    var form = $(this).closest("form");
                    if (form.validate) {
                        var validator = form.validate();
                        validator.element(this);
                    }
                });
            })
        }
    });

})(jQuery, jQuery.validator);
