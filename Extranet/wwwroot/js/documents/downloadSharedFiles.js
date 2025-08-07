$(() => {

    //Téléchargements

    document.querySelectorAll('.chkDownload').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            if (document.querySelectorAll('.chkDownload:checked').length > 0) {
                // afficher le téléchargement de sélection
                document.querySelector('.btnDownloadSelectionShared').style.display = 'block';
            } else {
                document.querySelector('.btnDownloadSelectionShared').style.display = 'none';
            }
        });
    });

    $('body').on('click', '.btnDownloadSelectionShared, .btnDownloadShared', function () {
        downloadZip($(this));
    });


    const rows = document.querySelectorAll('.document');
    rows.forEach(row => {
        row.addEventListener('click', function (event) {
            const checkbox = row.querySelector('.chkDownload');
            if (checkbox && event.target !== checkbox) {  // Ensure the click didn't happen directly on the checkbox
                checkbox.checked = !checkbox.checked;
                checkbox.dispatchEvent(new Event('change'));  // Trigger the change event
            }
        });
    });

});
function datesFormatter(value) {
    let parts = value.split(/<img/i);

    let dateStr = parts[0].trim();

    if (dateStr.length !== 8) {
        throw new Error("Invalid date format. Expected YYYYMMDD.");
    }

    let year = dateStr.slice(0, 4);
    let month = dateStr.slice(4, 6);
    let day = dateStr.slice(6, 8);

    let formattedYear = year.slice(2);

    return `${day}/${month}/${formattedYear}` + (parts[1] ? '<img' + parts[1] : '');
}

function decFormatter(value) {
    let parts = value.split(/<img/i);

    let intPart = parseFloat(parts[0]);

    let numStr = intPart.toString();

    if (numStr.length <= 2) {
        return '0.' + numStr.padStart(2, '0');
    }

    let integerPart = numStr.slice(0, -2);
    let decimalPart = numStr.slice(-2);

    return `${integerPart},${decimalPart}` + (parts[1] ? '<img' + parts[1] : '');
}

function GetSelectedDoc($this) {
    var ids = [];
    if ($this.hasClass('btnDownloadSelectionShared')) {
        $('.chkDownload:checked').each(function () {
            ids.push($(this).attr('data-docno'));
        });
    } else {
        ids.push($this.parents('tr').find('.chkDownload').attr('data-docno'));
    }
    return ids;
}

function downloadZip($this) {
    var ids = GetSelectedDoc($this);
    //console.log('ids', ids);
    $.ajax({
        url: '/MyAccount/ZipShared',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ type: '', docNos: ids }),
        success: function () {
            window.open('/MyAccount/DownloadZipShared', '_blank');
        },
        error: function (xhr, status, error) {
            console.log('error', error);
            console.log('status', status);
            console.log('xhr', xhr);
        }
    });
}

function decodeHtmlEntities(text) {
    var tempElement = document.createElement('textarea');
    tempElement.innerHTML = text;
    return tempElement.value;
}

function getCellValue(row, index) { return $(row).children('td').eq(index).text(); }

function filterTable() {
    var filters = {};

    // Itérer sur chaque filtre dynamique et ajouter au filtre
    $('.filter-select').each(function () {
        var filterValue = $(this).val();
        var propName = $(this).data('propname');

        // Si un filtre a une valeur
        if (filterValue) {
            if (propName.toLowerCase().includes("date")) {
                let [day, month, year] = filterValue.split('/');

                day = day.padStart(2, '0');
                month = month.padStart(2, '0');
                if (year.length === 2) {
                    year = '20' + year;
                }

                filterValue = year + month + day;
            }

            filters[propName] = filterValue;
        }
    });

    // Appliquer les filtres à la table
    var $table = $('#table')
    $table.bootstrapTable('filterBy', filters);
}