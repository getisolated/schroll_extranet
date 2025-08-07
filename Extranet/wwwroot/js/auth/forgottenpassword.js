const emailInput = document.getElementById('emailInput');
const passwordInput = document.getElementById('passwordInput');
const passwordConfirmInput = document.getElementById('passwordConfirmInput');
const submitButton = document.getElementById('submitButton');
const emailError = document.getElementById('emailError');
const passwordError = document.getElementById('passwordError');
const passwordConfirmError = document.getElementById('passwordConfirmError');

document.getElementById('passwordInput').addEventListener('input', function () {
    validateForm();
});
document.getElementById('passwordConfirmInput').addEventListener('input', function () {
    validateForm();
});

document.getElementById('email').addEventListener('input', function () {
    validateForm();
});

function validateEmail() {
    const email = emailInput.value;
    let errorMessage = '';

    if (!/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)) {
        errorMessage += "Le format de l'email est invalide.<br>";
    }

    emailError.innerHTML = errorMessage;
    if (email != '') {
        emailError.style.display = errorMessage ? 'block' : 'none';
    }

    return !errorMessage;
}

function validatePassword() {
    const password = passwordInput.value;
    let errorMessage = '';

    if (!/(?=.*[a-z])(?=.*[A-Z])/.test(password)) {
        errorMessage += 'Le mot de passe doit contenir au moins 1 majuscule et 1 minuscule.<br>';
    }
    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
        errorMessage += 'Le mot de passe doit contenir au moins 1 caractère spécial.<br>';
    }
    if (password.length < 8) {
        errorMessage += 'Le mot de passe doit contenir au moins 8 caractères.<br>';
    }

    passwordError.innerHTML = errorMessage;
    if (password != '') {
        passwordError.style.display = errorMessage ? 'block' : 'none';
    }

    return !errorMessage;
}

function validatePasswordConfirm() {
    const password = passwordInput.value;
    const passwordConfirm = passwordConfirmInput.value;
    let errorMessage = '';

    if (password !== passwordConfirm) {
        errorMessage = 'Les mots de passe ne correspondent pas.';
    }

    passwordConfirmError.innerHTML = errorMessage;
    if (passwordConfirm != '') {
        passwordConfirmError.style.display = errorMessage ? 'block' : 'none';
    }

    return !errorMessage;
}

function validateForm() {
    const isEmailValid = validateEmail();
    const isPasswordValid = validatePassword();
    const isPasswordConfirmValid = validatePasswordConfirm();

    submitButton.disabled = !(isPasswordValid && isPasswordConfirmValid && isEmailValid);
}