$(() => {
    $('body').on('change', '#searchPeriodFrom, #searchPeriodTo', function () {
        //if (!VerifyYears()) return;

        if (!VerifyDates()) {
            $('.error').text('La période choisie est invalide, veuillez vérifier les dates saisies.').css('display', '');
            $('#searchPeriodConfirm').attr('disabled', true);
        } else {
            $('.error').text('').css('display', 'none');
            $('#searchPeriodConfirm').attr('disabled', false);
        }
    });
});


function VerifyDates() {
    var dateFrom = $('#searchPeriodFrom').val();
    var dateTo = $('#searchPeriodTo').val();

    return new Date(dateTo) > new Date(dateFrom);
}
function VerifyYears() {
    let fromDateStr = $("#searchPeriodFrom").val();
    let [fromDateYear, fromDateMonth, fromDateDay] = fromDateStr.split('-');

    let toDateStr = $("#searchPeriodTo").val();
    let [toDateYear, toDateMonth, toDateDay] = toDateStr.split('-');

    //console.log('fromDateYear', fromDateYear);
    //console.log('toDateYear', toDateYear);
    //console.log('fromDateYear !== toDateYear', fromDateYear !== toDateYear);

    if (fromDateYear !== toDateYear) {
        $('.error').text('Vous ne pouvez exporter que les données d\'une même année.').css('display', '');
        return false;
    } else {
        $('.error').css('display', 'none');
        return true;
    }
}