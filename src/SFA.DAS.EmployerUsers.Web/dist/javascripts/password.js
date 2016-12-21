$.fn.delayKeyup = function (callback, ms) {
    var timer = 0;
    var el = $(this);
    $(this).keyup(function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
            callback(el)
        }, ms);
    });
    return $(this);
};

$(function () {

    var pwdField = $('#Password, #NewPassword'),
        pwdStr = $('<p />').html('Password strength: ').attr('class', 'password-strength'),
        pwdSpn = $('<span />').attr({ 'id': 'pwd-score', 'aria-live': 'polite' }).appendTo(pwdStr);

    pwdField.after(pwdStr).delayKeyup(function (el) {

        var pwd = el.val();

        if (pwd.length > 0) {

            var pwdTest = zxcvbn(pwd),
                helperText = '';

            if (pwdTest.score === 0) {
                helperText = 'Very weak';
            }
            if (pwdTest.score === 1) {
                helperText = 'Weak';
            }
            if (pwdTest.score === 2) {
                helperText = 'Good';
            }
            if (pwdTest.score === 3) {
                helperText = 'Strong';
            }
            if (pwdTest.score === 4) {
                helperText = 'Very strong';
            }

            $('#pwd-score').text(helperText).parent().attr('class', 'score score-' + pwdTest.score);

        } else {
            $('#pwd-score').empty().parent().attr('class', 'password-strength');
        }

    }, 100);
});