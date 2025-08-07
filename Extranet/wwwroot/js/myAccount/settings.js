const updatePasswordButton = document.getElementById('updatePasswordButton');
const passwordForm = document.getElementById('passwordForm');
const currentPasswordInput = document.getElementById('currentPassword');
const newPasswordInput = document.getElementById('newPassword');
const confirmPasswordInput = document.getElementById('confirmPassword');
const submitPasswordButton = document.getElementById('submitPasswordButton');
const currentPasswordError = document.getElementById('currentPasswordError');
const newPasswordError = document.getElementById('newPasswordError');
const confirmPasswordError = document.getElementById('confirmPasswordError');

updatePasswordButton.addEventListener('click', function () {
    passwordForm.style.display = 'block';
    updatePasswordButton.style.display = 'none';
});

currentPasswordInput.addEventListener('input', validateForm);
newPasswordInput.addEventListener('input', validateForm);
confirmPasswordInput.addEventListener('input', validateForm);

function validateCurrentPassword() {
    const password = currentPasswordInput.value;
    let errorMessage = '';

    if (!password) {
        errorMessage = 'Le mot de passe actuel est requis.';
    }

    currentPasswordError.innerHTML = errorMessage;
    currentPasswordError.style.display = errorMessage ? 'block' : 'none';

    return !errorMessage;
}

function validateNewPassword() {
    const password = newPasswordInput.value;
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

    newPasswordError.innerHTML = errorMessage;
    newPasswordError.style.display = errorMessage ? 'block' : 'none';

    return !errorMessage;
}

function validatePasswordConfirm() {
    const password = newPasswordInput.value;
    const passwordConfirm = confirmPasswordInput.value;
    let errorMessage = '';

    if (password !== passwordConfirm) {
        errorMessage = 'Les mots de passe ne correspondent pas.';
    }

    confirmPasswordError.innerHTML = errorMessage;
    confirmPasswordError.style.display = errorMessage ? 'block' : 'none';

    return !errorMessage;
}

function validateForm() {
    const isCurrentPasswordValid = validateCurrentPassword();
    const isNewPasswordValid = validateNewPassword();
    const isPasswordConfirmValid = validatePasswordConfirm();

    submitPasswordButton.disabled = !(isCurrentPasswordValid && isNewPasswordValid && isPasswordConfirmValid);
    return (isCurrentPasswordValid && isNewPasswordValid && isPasswordConfirmValid);
}

passwordForm.addEventListener('submit', function (event) {
    event.preventDefault();

    if (validateForm()) {
        const formData = {
            currentPassword: document.getElementById('currentPassword').value,
            newPassword: document.getElementById('newPassword').value,
            name: document.getElementById('name').value,
            lastName: document.getElementById('lastName').value,
            mail: document.getElementById('mail').value
        };

        $.ajax({
            url: "/MyAccount/ChangePassword",
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            data: JSON.stringify(formData),
            success: function (data) {
                passwordForm.reset();
                passwordForm.style.display = 'none';
                updatePasswordButton.style.display = 'block';
                toastr.success("Mot de passe mis à jour avec succès.");
            },
            error: function (xhr, status, error) {
                toastr.error("Une erreur s'est produite.");
            }
        });
    }
});
