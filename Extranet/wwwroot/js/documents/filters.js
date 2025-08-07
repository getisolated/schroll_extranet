$(() => {

    //Filter by period
    $("#searchPeriod, #searchPeriod-mobile").on('change', function () {
        location.href = window.location.href.split('?')[0] + "?year=" + $(this).val();
    });

    $("#searchPeriodConfirm").on('click', function () {
        let fromDateStr = $("#searchPeriodFrom").val();
        let fromDate = GetDateFormatted(fromDateStr);

        let toDateStr = $("#searchPeriodTo").val();
        let toDate = GetDateFormatted(toDateStr);

        if (fromDateStr && fromDateStr) {
            location.href = window.location.href.split('?')[0] + "?from=" + fromDate + "&to=" + toDate;
        } else {
            location.href = window.location.href.split('?')[0];
        }
    });


    // Ajouter des écouteurs d'événements pour les changements sur les sélections dynamiques
    $('.filter-select').on('change', function () {
        filterTable();
    });

    // Ajouter des écouteurs d'événements pour la saisie dans les champs de recherche textuelle
    $("#searchFilter, #searchFilter-mobile").keyup(function (e) {
        var disallow = [37, 38, 39, 40]; // ignorer les touches fléchées
        if ($.inArray(e.which, disallow) > -1) {
            return true;
        }
        filterTable();
    });

    // Réinitialiser le filtre si le texte de recherche est supprimé
    $("#searchFilter, #searchFilter-mobile").on('change', function () {
        if ($(this).val() == "") {
            filterTable();
            $('#table').bootstrapTable('refresh');
        }
    });

    const rows = document.querySelectorAll('.document');
    rows.forEach(row => {
        row.addEventListener('click', function (event) {
            const checkbox = row.querySelector('.chkDownload');//btnDownload
            const btnDownload = row.querySelector('.btnDownload');
            if (checkbox && event.target !== checkbox && event.target !== btnDownload) {  // Ensure the click didn't happen directly on the checkbox
                checkbox.checked = !checkbox.checked;
                checkbox.dispatchEvent(new Event('change'));  // Trigger the change event
            }
        });
    });
});

function datesFormatter(value) {
    //let parts = value.split(/<img/i);
    let dateYYYYMMDD = value.replace(/<img[^>"']*((("[^"]*")|('[^']*'))[^"'>]*)*>/g, "");
    let img = value.replace(dateYYYYMMDD, "");

    let dateStr = dateYYYYMMDD.trim();
    ////console.log('dateYYYYMMDD', dateYYYYMMDD);
    ////console.log('img', img);

    if (dateStr.length !== 8) return;
    
    let year = dateStr.slice(0, 4);
    let month = dateStr.slice(4, 6);
    let day = dateStr.slice(6, 8);

    let formattedYear = year.slice(2);

    return img + `${day}/${month}/${formattedYear}`;
}

function decFormatter(value) {
    let number = value.replace(/<img[^>"']*((("[^"]*")|('[^']*'))[^"'>]*)*>/g, "");
    let img = value.replace(number, "");

    let intPart = parseFloat(number);

    let numStr = intPart.toString();

    if (numStr.length <= 2) {
        return img + '0,' + numStr.padStart(2, '0') + (this.symbol !== "" ? this.symbol : "");
    }

    let integerPart = numStr.slice(0, -2);
    let decimalPart = numStr.slice(-2);

    integerPart = integerPart.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");

    return img + `${integerPart},${decimalPart}` + (this.symbol !== "" ? this.symbol : "");
}

function GetDateFormatted(dateStr) {
    let [dateYear, dateMonth, dateDay] = dateStr.split('-');
    let dateShortYear = dateYear.slice(-2);
    let dateFormatted = `${dateDay}/${dateMonth}/${dateShortYear}`;
    return dateFormatted;
}

function comparer(index) {
    return function (a, b) {
        var valA = getCellValue(a, index).replace(',', '.'), valB = getCellValue(b, index).replace(',', '.');
        return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
    }
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
                let [year, month, day] = filterValue.split('-');

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
    var $table = $('#table');
    $table.bootstrapTable('filterBy', filters, {
        'filterAlgorithm': (row, filters) => {
            var displayRow = false;
            if (filters == null || filters.length == 0) return true;

            for (const [key, value] of Object.entries(filters)) {
                //console.log(`${key}: ${value}`);

                var tmpvalue = value;
                if (Array.isArray(value)) {
                    tmpvalue.forEach((element) => element.replace('&', '&amp;'));
                    displayRow = tmpvalue.some((x) => row[key].includes(x));
                } else if (value.trim() != '') {
                    tmpvalue = value.replace('&', '&amp;');
                    displayRow = row[key].includes(tmpvalue);
                } else if (value.trim() == '') {
                    displayRow = true;
                }

                if (!displayRow) break;
            }

            return displayRow;
        }

    });

    FixBug();
}

//$('#table').on('all.bs.table', function (name, args) {
//    console.log(`name:${name}, args:${args}`);
//});

$('#table').on('page-change.bs.table', function () {
    FixBug();
});

function FixBug() {
    var options = $('#table').bootstrapTable('getOptions');

    var field = options.sortName;
    var sortOrder = options.sortOrder;
    $('#table').bootstrapTable('sortBy', {
        field,
        sortOrder
    });
}