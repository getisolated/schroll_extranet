
// Fonction pour changer la société en fonction de l'ID du lien
function changeCompany(newCompany) {
    // Envoi d'une requête AJAX pour changer la société en utilisant le contrôleur correspondant
    fetch('/Home/ChangeCompany', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': '@Request.Cookies["CSRF-TOKEN"]' // Assurez-vous de passer le jeton CSRF s'il est utilisé => ? à quoi ça sert ?
        },
        body: JSON.stringify(newCompany = newCompany)
    }).then(response => {
        window.location.href = '/Home/Accueil';
    })
        .catch(error => {
            console.error('Erreur lors de la requête AJAX :', error);
        });
}

// Intercepter le clic sur chaque lien et appeler la fonction changeSociety avec le nom de la société correspondante
$('.switch-company').on('click', function () {
    var newCompany = this.getAttribute('data-company');
    //console.log('newcompany', newCompany);
    changeCompany(newCompany);

});

function toggleSidebar() {
    var sidebar = document.getElementById('sidebar');
    var sidebarBack = document.getElementById('sidebar-back');
    if (sidebar.classList.contains('active')) {
        sidebar.classList.remove('active');
        sidebarBack.classList.remove('active');
    } else {
        sidebar.classList.add('active');
        sidebarBack.classList.add('active');
    }
}

$('.switch-account').on('click', function () {

    var jdata = JSON.stringify(accountNo = this.getAttribute('data-account-no'));
    //console.log('jdata', jdata);

    changeAccount(jdata);
});

function changeAccount(jdata) {
    $.ajax({
        type: "POST",
        url: "/Home/ChangeAccount",
        dataType: "json",
        contentType: "application/json",
        data: jdata,
        success: function onSucess(result) {
            if (result) {
                window.location.href = '/Home/Accueil';
            }
            else {
                $('#error').css('display', '');
            }
        },
        failure: function (response) {
            failure(response);
        },
        error: function (xhr, textStatus, err) {
            error(xhr, textStatus, err);
        }
    });
}

$('.searchAccounts').on('input', function () {
    filterAccounts($(this));
});

function filterAccounts($this) {
    var value = $this.val().toLowerCase();
    $("ul li.account").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });

    $('.dropdown-menu').animate({
        scrollTop: $(".searchAccounts").offset().top
    }, 0);
}

$('#myDropdown').on('shown.bs.dropdown', function () {
    $('.searchAccounts').trigger('focus');
});