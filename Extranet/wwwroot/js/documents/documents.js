function submitFiltersForm() {
    document.getElementById("filterForm").submit();
}

function resetFilters() {
    var fromDateFilter = new Date();
    fromDateFilter.setMonth(fromDateFilter.getMonth() - 6);
    var toDateFilter = new Date();
    toDateFilter.setMonth(toDateFilter.getMonth() + 6);
    document.getElementById("searchFilter").value = "";
    document.getElementById("siteFilter").value = "All";
    document.getElementById("fromDateFilter").value = fromDateFilter.getFullYear() + '-' + ('0' + (fromDateFilter.getMonth() + 1)).slice(-2) + '-' + ('0' + fromDateFilter.getDate()).slice(-2);
    document.getElementById("toDateFilter").value = toDateFilter.getFullYear() + '-' + ('0' + (toDateFilter.getMonth() + 1)).slice(-2) + '-' + ('0' + toDateFilter.getDate()).slice(-2);
    document.getElementById("typeFilter").value = "All";
    submitFiltersForm();
}

function toggleAdvancedFilters() {
    var advancedFilters = document.getElementsByClassName("advancedFilter");
    var advancedFiltersToggle = document.getElementsByClassName("advancedFiltersToggle")[0];
    if (!advancedFilters[0].classList.contains("show")) {
        Array.prototype.forEach.call(advancedFilters, function (advancedFilter) {
            advancedFilter.classList.add("show");
        });
        advancedFiltersToggle.innerHTML = '<i class="fa-solid fa-chevron-up"></i> Moins de filtres';
    } else {
        Array.prototype.forEach.call(advancedFilters, function (advancedFilter) {
            advancedFilter.classList.remove("show");
        });
        advancedFiltersToggle.innerHTML = '<i class="fa-solid fa-chevron-down"></i> Plus de filtres';
    }
}

function removeFilter(filterKey) {
    var url = new URL(window.location.href);
    url.searchParams.delete(filterKey);

    window.location.href = url.toString();
}