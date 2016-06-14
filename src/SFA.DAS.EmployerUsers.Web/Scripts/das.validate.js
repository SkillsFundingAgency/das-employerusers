(function () {
    var enableValidation = function () {
        $("form").validate({
            showErrors: function (errorMap, errorList) {
                $.each(this.validElements(), function (index, element) {
                    var $element = $(element);
                    var $errorLabel = $("span[data-valmsg-for='" + $element.attr('id') + "']");
                    var $container = $element.closest('.form-group');

                    $errorLabel.text("");
                    $container.removeClass("input-validation-error");
                });

                $.each(errorList, function (index, error) {
                    var $element = $(error.element);
                    var $errorLabel = $("span[data-valmsg-for='" + $element.attr('id') + "']");
                    var $container = $element.closest('.form-group');

                    $errorLabel.text(error.message);
                    $container.addClass("input-validation-error");
                });
            }
        });
    };
    
    window.das = window.das || {};
    window.das.validation = {
        enableValidation: enableValidation
    }

})()