$(() => {
    //Téléchargements

    document.querySelectorAll('.chkDownload').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            if (document.querySelectorAll('.chkDownload:checked').length > 0) {
                // afficher le téléchargement de sélection
                document.querySelector('.btnDownloadSelection').style.display = 'block';
            } else {
                document.querySelector('.btnDownloadSelection').style.display = 'none';
            }
        });
    });

    $('body').on('click', '.btnDownloadSelection, .btnDownload', function () {
        var ids = [];
        $('#downloadZip').attr('data-nos', '');
        if ($(this).hasClass('btnDownloadSelection')) {
            $('.chkDownload:checked').each(function () {
                ids.push($(this).attr('data-docno'));
            });
        } else {
            ids.push($(this).parents('tr').find('.chkDownload').attr('data-docno'));
        }

        if (ids.length > 0) {
            //console.log('ids', ids);
            $('#downloadZip').attr('data-nos', JSON.stringify(ids));

            $('#ulDownloadDocs').html('');
            $.each(ids, function (index, value) {
                $('#ulDownloadDocs').append('<li>' + value + '&nbsp;<span id="' + value + 'Loader"><img src="/img/loading/loader.svg" class="downloadLoader" style = "width:15px;heigth:15px;margin-top:-3px;" />T&eacute;l&eacute;chargement en cours...</span> <a id="' + value + 'Link" href="#" target="_blank" style="display:none;">T&eacute;l&eacute;charger le document</a></li > ');
            });

            $('#downloadZip').css('display', 'none');
            $('#downloadModal').modal('show');

            downloadDocs(ids);
        }
    });

    $('body').on('click', '.btnDownloadAll', function () {
        var ids = [];
        $('#downloadZip').attr('data-nos', '');

        var $table = $('#table')
        var dataList = $table.bootstrapTable('getData')
        dataList.forEach(item => {
            ids.push(item["N\u00B0 dossier"]);
        });

        if (ids.length > 0) {
            //console.log('ids', ids);
            $('#downloadZip').attr('data-nos', JSON.stringify(ids));

            $('#downloadZip').css('display', 'none');
            $('#downloadModal').modal('show');

            downloadDocs(ids);
        }
    });

    $('body').on('click', '#downloadZip', function () {

        var docNos = $('#downloadZip').attr('data-nos');
        //console.log('docNos', docNos);

        $.ajax({
            url: '/Documents/Zip',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ type: $('#hfDocType').val(), docNos: JSON.parse(docNos) }),//TODO : rajouter un paramètre pour le type de document
            success: function () {
                //console.log('downloadZip', data);
                window.open('/Documents/DownloadZip', '_blank');
            },
            error: function (xhr, status, error) {
                console.log('error', error);
                console.log('status', status);
                console.log('xhr', xhr);
                $('.downloadLoader').each(function () {
                    $(this).css('display', 'none');
                    //TODO : afficher un message d'erreur
                });
            }
        });
    });

    $('#downloadModal').on('hidden.bs.modal', function () {
        clearTimeout(downloadTimer);
    });

});


var timeout = 3000;
var downloadTimer;
function downloadDocs(ids) {
    var docNos = ids;
    //console.log('downloadDocs');
    //console.log('ids', ids);
    clearTimeout(downloadTimer);
    $.ajax({
        url: '/Documents/DownloadDocs',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ type: $('#hfDocType').val(), docNos: docNos }),
        success: function (data) {
            //console.log('downloadDocs data', data);
            var stopTimer = true;
            $.each(data, function (index, value) {
                if (value.fileName == null || value.fileName == '') {
                    stopTimer = false;
                    //console.log('each.stopTimer', stopTimer);
                    return true;
                }

                $('#' + value.docNo + 'Loader').css('display', 'none');
                $('#' + value.docNo + 'Link').attr('href', value.fileName);

                $('#' + value.docNo + 'Link').show();

                docNos = $.grep(docNos, function (grepvalue) {
                    return grepvalue != value.docNo;
                });
                //console.log('grep', docNos);
                //console.log('each.stopTimer2', stopTimer);
            });

            //console.log('stopTimer', stopTimer);
            if (stopTimer) {
                clearTimeout(downloadTimer);
                if (ids.length > 1)
                    $('#downloadZip').css('display', 'block');
            } else {
                downloadTimer = setTimeout(function () { downloadDocs(docNos) }, timeout);
            }
        },
        error: function (xhr, status, error) {
            clearTimeout(downloadTimer);
            $('.downloadLoader').each(function () {
                $(this).css('display', 'none');
            });

            $('#ulDownloadDocs').html('');
            $('#ulDownloadDocs').append('<li>Erreur durant le t&eacute;l&eacute;chargement du fichier, veuillez recharger la page</li>');
        }
    });
}