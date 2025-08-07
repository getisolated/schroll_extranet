$(() => {
    $('.btnDownloads').css('display', '');

    //Téléchargements

    $('body').on('click', '.btnDownloadRegister', function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (!VerifyDates()) return;

        var ids = ["Registre"];

        if (ids.length > 0) {
            $('#ulDownloadDocs').html('');
            $.each(ids, function (index, value) {
                $('#ulDownloadDocs').append('<li>' + value + '&nbsp;<span id="' + value + 'Loader"><img src="/img/loading/loader.svg" class="downloadLoader" style = "width:15px;heigth:15px;margin-top:-3px;" />T&eacute;l&eacute;chargement en cours...</span> <a id="' + value + 'Link" href="#" target="_blank" style="display:none;">T&eacute;l&eacute;charger le document</a></li > ');
            });
            if (!VerifyYears()) return;

            $('#downloadZip').css('display', 'none');
            $('#downloadModal').modal('show');

            downloadDocs(ids);
        }
    });

    $('#downloadModal').on('hidden.bs.modal', function () {
        clearTimeout(downloadTimer);
    });

});

var timeout = 3000;
var downloadTimer;
function downloadDocs(ids) {
    var docNos = ids;
    //const searchParams = new URLSearchParams(window.location.search);
    //var from = searchParams.get('from');
    //var to = searchParams.get('to');

    var $ai = $('select[data-filter="AIFilter"]');
    var aival = $ai.length > 0 ? $ai.val() : null;

    clearTimeout(downloadTimer);

    let fromDateStr = $("#searchPeriodFrom").val();
    let from = GetDateFormatted(fromDateStr);
    let toDateStr = $("#searchPeriodTo").val();
    let to = GetDateFormatted(toDateStr);

    $.ajax({
        url: '/Registre/DownloadDocs?from=' + (from != null ? from : '') + '&to=' + (to != null ? to : '') + (aival != null ? '&ai=' + aival : ''),
        type: 'POST',
        contentType: 'application/json',
        data: "{}",
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
                //$('#downloadZip').css('display', 'block');
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