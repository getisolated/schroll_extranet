
//(function () {
//    $('.stat-action').on('click', function () {
//        var stat = $(this).attr('data-stat');
//        var action = $(this).attr('data-action');

//        setApercuStatsPartial(stat, action);
//    });
//})();

function firstDayOfNextMonth(date) {
    const year = date.getFullYear();
    const nextMonth = date.getMonth() + 1;

    return new Date(year, nextMonth, 1);
}

function lastDayOfNextMonth(date) {
    const year = date.getFullYear();
    const nextMonth2 = date.getMonth() + 2;

    return new Date(year, nextMonth2, 0);
}

function firstDayOfLastMonth(date) {
    const year = date.getFullYear();
    const lastMonth = date.getMonth() - 1;

    return new Date(year, lastMonth, 1);
}

function lastDayOfLastMonth(date) {
    const year = date.getFullYear();
    const currentMonth = date.getMonth();

    return new Date(year, currentMonth, 0);
}

function getStringFromDate(date) {
    const month = date.getMonth() + 1;
    const day = date.getDate();
    const year = date.getFullYear() % 100;

    const formattedMonth = month.toString().padStart(2, '0');
    const formattedDay = day.toString().padStart(2, '0');
    const formattedYear = year.toString().padStart(2, '0');

    return `${formattedDay}/${formattedMonth}/${formattedYear}`;
}

function getDateFromString(dateString) {
    const parts = dateString.split('/');

    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1;
    const year = parseInt(parts[2], 10) + 2000;

    var CreatedDate = new Date(year, month, day);

    return CreatedDate;
}

function getMonthString(date) {
    const options = { month: 'long' };
    return date.toLocaleDateString('fr-FR', options);
}

function getYearString(date) {
    const options = { year: 'numeric' };
    return date.toLocaleDateString('fr-FR', options);
}
function formatNumber(value) {
    if (value >= 1000) {
        return (value / 1000).toFixed(2) + 'K €';
    } else {
        return value + ' €';
    }
}

function getDifferenceHtml(current, previous) {
    let difference = previous > 0 ? (current - previous) / previous * 100 : 100;
    if (previous == 0 && current == 0) difference = 0;
    let displayValue = '';
    let arrow = '';

    if (difference > 0) {
        arrow = 'fa-arrow-up';
        displayValue = difference.toFixed(0) + "%";
    } else if (difference < 0) {
        arrow = 'fa-arrow-down';
        displayValue = difference.toFixed(0) + "%";
    } else {
        arrow = 'fa-equals';
    }

    const cls = difference > 0 ? 'positive' : (difference < 0 ? 'negative' : 'equal');
    return `<span class="home-stats-variation ${cls}">
                <i class="fa-solid ${arrow}"></i> ${displayValue}
            </span>`;
}

function setApercuStats(data) {
    //console.log(data);
    if (fromMonth == toMonth && fromYear == toYear) {
        document.getElementById('tauxDate').innerText = `${toMonth} ${toYear}`;
        document.getElementById('tauxCurrentMonth').innerText = `${toMonth} ${toYear}`;
    } else {
        document.getElementById('tauxDate').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
        document.getElementById('tauxCurrentMonth').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
    }
    document.getElementById('tauxValue').innerHTML = `${data.tbaValorisationMatiere}<span class="home-stats-unit">%</span>`;
    document.getElementById('tauxVariation').innerHTML = `${getDifferenceHtml(data.tbaValorisationMatiere, data.prevTbaValorisationMatiere)} vs ${previousMonth} ${previousYear}`;
    document.getElementById('tauxStat').setAttribute('data-fromdate', fromDate);
    document.getElementById('tauxStat').setAttribute('data-todate', toDate);

    if (fromMonth == toMonth && fromYear == toYear) {
        document.getElementById('dechetsDate').innerText = `${toMonth} ${toYear}`;
        document.getElementById('dechetsCurrentMonth').innerText = `${toMonth} ${toYear}`;
    } else {
        document.getElementById('dechetsDate').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
        document.getElementById('dechetsCurrentMonth').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
    }
    document.getElementById('dechetsValue').innerHTML = `${data.tbaDechetsEvacues}<span class="home-stats-unit">t</span>`;
    document.getElementById('dechetsVariation').innerHTML = `${getDifferenceHtml(data.tbaDechetsEvacues, data.prevTbaDechetsEvacues)} vs ${previousMonth} ${previousYear}`;
    document.getElementById('dechetsStat').setAttribute('data-fromdate', fromDate);
    document.getElementById('dechetsStat').setAttribute('data-todate', toDate);

    if (fromMonth == toMonth && fromYear == toYear) {
        document.getElementById('facturationDate').innerText = `${toMonth} ${toYear}`;
        document.getElementById('facturationCurrentMonth').innerText = `${toMonth} ${toYear}`;
    } else {
        document.getElementById('facturationDate').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
        document.getElementById('facturationCurrentMonth').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
    }
    document.getElementById('facturationValue').innerHTML = `${formatNumber(data.tbaCA)}<span class="home-stats-unit"> HT</span>`;
    document.getElementById('facturationVariation').innerHTML = `${getDifferenceHtml(data.tbaCA, data.prevTbaCA)} vs ${previousMonth} ${previousYear}`;
    document.getElementById('facturationStat').setAttribute('data-fromdate', fromDate);
    document.getElementById('facturationStat').setAttribute('data-todate', toDate);

    if (fromMonth == toMonth && fromYear == toYear) {
        document.getElementById('passagesDate').innerText = `${toMonth} ${toYear}`;
        document.getElementById('passagesCurrentMonth').innerText = `${toMonth} ${toYear}`;
    } else {
        document.getElementById('passagesDate').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
        document.getElementById('passagesCurrentMonth').innerText = `${fromMonth} ${fromYear} - ${toMonth} ${toYear}`;
    }
    document.getElementById('passagesValue').innerHTML = `${data.tbaNbPassage}<span class="home-stats-unit"></span>`;
    document.getElementById('passagesVariation').innerHTML = `${getDifferenceHtml(data.tbaNbPassage, data.prevTbaNbPassage)} vs ${previousMonth} ${previousYear}`;
    document.getElementById('passagesStat').setAttribute('data-fromdate', fromDate);
    document.getElementById('passagesStat').setAttribute('data-todate', toDate);

}

function setApercuStatsPartial(stat, month) {

    let currentFromDateString = document.getElementById(`${stat}Stat`).getAttribute("data-fromdate");
    let currentToDateString = document.getElementById(`${stat}Stat`).getAttribute("data-todate");

    currentFromDate = getDateFromString(currentFromDateString);
    currentToDate = getDateFromString(currentToDateString);

    let newFromDate;
    let newToDate;

    if (month === "next") {
        newFromDate = firstDayOfNextMonth(currentToDate);
        newToDate = lastDayOfNextMonth(currentToDate);
    } else if (month === "previous") {
        newFromDate = firstDayOfLastMonth(currentFromDate);
        newToDate = lastDayOfLastMonth(currentFromDate);
    } else {
        newFromDate = new Date(currentFromDate);
        newToDate = new Date(currentToDate);
    }

    let partialPreviousDate = new Date();
    partialPreviousDate.setMonth(newFromDate.getMonth() - 1);

    let partialPreviousMonth = getMonthString(partialPreviousDate);
    const partialPreviousYear = getYearString(partialPreviousDate);

    $.ajax({
        url: '/Stats/ApercuStats',
        type: 'GET',
        contentType: 'json',
        data: {
            "fromDate": getStringFromDate(newFromDate),
            "toDate": getStringFromDate(newToDate)
        },
        success: function (data) {

            let value = "";
            let prevValue = "";
            let unit = "";

            if (stat == "taux") {
                value = data.TbaValorisationMatiere;
                prevValue = data.prevTbaValorisationMatiere
                unit = "%"
            } else if (stat == "dechets") {
                value = data.TbaDechetsEvacues;
                prevValue = data.prevTbaDechetsEvacues;
                unit = "t";
            } else if (stat == "facturation") {
                value = formatNumber(data.TbaCA);
                prevValue = data.prevTbaCA;
                unit = " HT";
            } else if (stat == "passages") {
                value = data.TbaNbPassage;
                prevValue = data.prevTbaNbPassage;
            }

            document.getElementById(`${stat}Date`).innerText = "";
            document.getElementById(`${stat}Value`).innerHTML = `${value}<span class="home-stats-unit">${unit}</span>`;
            document.getElementById(`${stat}Variation`).innerHTML = `${getDifferenceHtml(value, prevValue)} vs ${partialPreviousMonth} ${partialPreviousYear}`;
            document.getElementById(`${stat}CurrentMonth`).innerText = getMonthString(newFromDate) + " " + getYearString(newFromDate);
            document.getElementById(`${stat}Stat`).setAttribute('data-fromDate', getStringFromDate(newFromDate));
            document.getElementById(`${stat}Stat`).setAttribute('data-toDate', getStringFromDate(newToDate));
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });

}