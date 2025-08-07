const contactForm = document.getElementById('contactForm');
const subjectInput = document.getElementById('subject');
const messageInput = document.getElementById('message');
const submitContactButton = document.getElementById('submitContactButton');
const subjectError = document.getElementById('subjectError');
const messageError = document.getElementById('messageError');

//subjectInput.addEventListener('input', validateForm);
//messageInput.addEventListener('input', validateForm);

function validateSubject() {
    const subject = subjectInput.value;
    let errorSubject = '';
    
    if (subject === "") {
        errorSubject += 'Vous devez choisir un sujet.<br>';
    }

    subjectError.innerHTML = errorSubject;
    subjectError.style.display = errorSubject ? 'block' : 'none';

    return !errorSubject;
}

function validateMessage() {
    const message = messageInput.value;
    let errorMessage = '';
    
    if (message.length == 0) {
        errorMessage += 'Le message ne peut pas être vide.<br>';
    }

    if (message.length > 250) {
        errorMessage += 'Le message ne peux pas dépasser 250 caractères.<br>';
    }

    messageError.innerHTML = errorMessage;
    messageError.style.display = errorMessage ? 'block' : 'none';

    return !errorMessage;
}

function validateForm() {
    const isSubjectValid = validateSubject();
    const isMessageValid = validateMessage();

    submitContactButton.disabled = !(isSubjectValid && isMessageValid);
    return (isSubjectValid && isMessageValid);
}

//contactForm.addEventListener('submit', function (event) {
//    event.preventDefault();

//    if (validateForm()) {
//        const formData = {
//            subject: document.getElementById('subject').value,
//            message: document.getElementById('message').value
//        };

//        $.ajax({
//            url: "/MyAccount/ContactMessage",
//            type: "POST",
//            contentType: "application/json;charset=UTF-8",
//            dataType: "json",
//            data: JSON.stringify(formData),
//            success: function (data) {
//                contactForm.reset();
//                toastr.success("Message envoyé avec succès.")
//            },
//            error: function (xhr, status, error) {
//                toastr.error("Une erreur s'est produite.");
//            }
//        });
//    }
//});