var ext;
var myfile64;
var myfile;
var maxSize = 10;
$('#myfile').change(function (e) {
    if (e.target.files.length > 0) {
        var pattern = /image-*/;

        var filesize = ((e.target.files[0].size / 1024) / 1024).toFixed(4);
        if (filesize > maxSize) {
            $('#error').text('La taille de l\'image ne doit pas dépasser les ' + maxSize +'Mo.');
            $(this).val('');
            return;
        }

        if (!e.target.files[0].type.match(pattern)) {
            $('#error').text('L\'image doit être au format JPG, JPEG ou PNG.');
            $(this).val('');
            return;
        }

        $('#error').text('');

        var splits = e.target.files[0].name.split('.');
        ext = splits[splits.length - 1];
        myfile = e.target.files[0];
        getBase64(myfile);
    }
    $('#error').text();
});

function getBase64(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        myfile64 = reader.result;
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

$('#create-news').on('click', function () {
    $('#error').text('');

    var data = quill.getSemanticHTML();
    //console.log('data', data);

    var data = {
        description: quill.getSemanticHTML(),
        link: $('#link').val(),
        linkTitle: $('#linkTitle').val(),
        myfile: myfile64,
        ext: ext
    };

    var jdata = JSON.stringify(data);
    //console.log('jdata', jdata);

    $.ajax({
        type: "POST",
        url: "/Admin/CreateNews",
        dataType: "json",
        contentType: "application/json",
        data: jdata,
        success: function onSucess(result) {
            if (result) {
                window.location.reload();
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
});

$('.delete-news-btn').on('click', function () {
    var newsItem = $(this).closest('.news-item');
    var newsId = newsItem.data('id');

    $.ajax({
        type: "POST",
        url: "/Admin/DeleteNews",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify({ id: newsId }),
        success: function (result) {
            if (result) {
                newsItem.remove();
            } else {
                alert("Échec de la suppression de l'actualité.");
            }
        },
        failure: function (response) {
            failure(response);
        },
        error: function (xhr, textStatus, err) {
            alert("Une erreur s'est produite lors du traitement de votre demande.");
        }
    });
});

var descriptionElement = document.getElementById('actu-desc');
var maxHeight = 240;
var ellipsisText = "...";

if (descriptionElement.scrollHeight > maxHeight) {
    while (descriptionElement.scrollHeight > maxHeight) {
        descriptionElement.innerHTML = descriptionElement.innerHTML.slice(0, -1);
    }
    descriptionElement.innerHTML += ellipsisText;
}