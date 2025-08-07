
$('a[href^="mailto:"]').on('click', function () {
    setTimeout(HideLoader, 2000);
});

function HideLoader() {
    $('.myloader').css('display', 'none');
}

window.onbeforeunload = function () {
    $('.myloader').css('display', 'block');
};
window.onpagehide = function () {
    HideLoader();
};

document.addEventListener("DOMContentLoaded", function () {
    HideLoader();
});


const news_row = document.querySelector('.news-row');
const news_item = document.querySelectorAll('.news-item');

news_item.forEach(function (tab) {
    tab.addEventListener('click', function () {
        news_row.querySelector('.active').classList.remove('active')
        tab.classList.add('active')
    })
})

let isDragging = false;

const dragging = (e) => {
    if (!isDragging) return;
    news_row.scrollLeft -= e.movementX;
    news_row.classList.add('dragging')
    //console.log(e.movementX);
    setTimeout(function () { handleIcon() }, 50)
};

const dragStop = () => {
    isDragging = false;
    news_row.classList.remove('dragging')
};


if ($('.news-row').length > 0) {
    news_row.addEventListener('mousemove', dragging);
    news_row.addEventListener('mousedown', () => isDragging = true);
    news_row.addEventListener('mouseup', dragStop);
    news_row.addEventListener('mouseleave', dragStop);
}


//#region ********************************** ERROR AJAX HANDLE DEB *********************************************/
var debug = true;

/*Failure ajax*/
function failure(response) {
    if (debug) {
        console.log('<p>failure:' + response.d + '</p>');
    }

    //TODO : $('.spinner-border').css('display', 'none');
}

/*Erreur ajax*/
function error(xhr, textStatus, err) {
    if (debug) {
        console.error('<p>error:\n readyState:' + xhr.readyState + '\n responseText: ' + xhr.responseText + '\n status: ' + xhr.status + '\n text status: ' + textStatus + '\n error: ' + err);
    }

    //TODO : $('.spinner-border').css('display', 'none');
}
//#endregion ********************************** ERROR AJAX HANDLE FIN *********************************************/
