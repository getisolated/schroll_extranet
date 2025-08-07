document.getElementById('email').addEventListener('input', function () {
    checkInputs();
});

document.getElementById('passwordInput').addEventListener('input', function () {
    checkInputs();
});

function checkInputs() {
    var emailInput = document.getElementById('emailInput').value;
    var passwordInput = document.getElementById('passwordInput').value;
    var submitButton = document.getElementById('submitButton');

    if (isValidEmail(emailInput) && passwordInput.length > 0) {
        submitButton.disabled = false;
    } else {
        submitButton.disabled = true;
    }
}

function isValidEmail(email) {
    return /\S+@\S+\.\S+/.test(email); //TODO: Validation appropriée
}

$('#forgottenpassword').on('click', function () {
    if ($("#emailInput").val().trim() == "") {
        $("#errormessage").text("Veuillez saisir votre adresse email");
        $("#message").css('display', 'none');
        $("#errormessage").css('display', '');
        return;
    }

    $.ajax({
        url: "/Auth/SubmitForgottenPwdRequest",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        data: JSON.stringify({
            email: $("#emailInput").val()
        }),
        success: function (data) {
            if (data.success) {
                $("#emailInput").val("");
                $("#message").text(data.message);
                $("#message").css('display', '');
                $("#errormessage").css('display', 'none');
            } else {
                $("#errormessage").text(data.message);
                $("#message").css('display', 'none');
                $("#errormessage").css('display', '');
            }
        }
    });
});