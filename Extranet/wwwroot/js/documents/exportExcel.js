$(() => {

    document.getElementById('exportXlsx').addEventListener('click', function () {
        var $table = $('#table');
        var tableData = $table.bootstrapTable('getData', { formatted: true });
        
        var headers = Object.keys(tableData[0]).filter(key => !key.startsWith('_'));
        
        var ws_data = [headers];
        
        tableData.forEach(function (row) {
            var rowData = headers.map(header => decodeHtmlEntities(row[header]));
            ws_data.push(rowData);
        });

        var exportXlsx = document.getElementById('exportXlsx');
        var sheetName = exportXlsx.getAttribute('data-sheet-name');
        var title = exportXlsx.getAttribute('data-title');
        var fileName = exportXlsx.getAttribute('data-file-name') + '.xlsx';
        
        var wb = XLSX.utils.book_new();
        var ws = XLSX.utils.aoa_to_sheet(ws_data);
        XLSX.utils.book_append_sheet(wb, ws, sheetName);
        
        wb.Props = {
            Title: title,
            CreatedDate: new Date()
        };
        
        XLSX.writeFile(wb, fileName);
    });

    function decodeHtmlEntities(text) {
        var tempElement = document.createElement('textarea');
        tempElement.innerHTML = text;
        return tempElement.value;
    }
});
