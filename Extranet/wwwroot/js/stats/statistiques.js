const sumDataLabel = {
    id: 'sumDataLabel',
    afterDatasetsDraw(chart, args, plugins) {
        const { ctx, data, scales: { y } } = chart;



        const metaByDataset = chart.data.datasets.map((_, i) => chart.getDatasetMeta(i));
        const dataLength = metaByDataset[0].data.length;

        for (let i = 0; i < dataLength; i++) {
            let total = 0;
            let lastY = null;
            let lastX = null;

            metaByDataset.forEach(meta => {
                const point = meta.data[i];
                if (!point) return;

                const value = chart.data.datasets[meta.index].data[i];
                total += value;

                lastY = point.y;
                lastX = point.x;
            });

            if (total > 0 && lastY !== null && lastX !== null) {
                ctx.save();
                ctx.font = 'bold 12px Arial';
                ctx.textAlign = 'center';
                ctx.fillStyle = plugins.color || '#656565';
                ctx.fillText(total.toFixed(plugins.digits || 0), lastX, lastY - 8);
                ctx.restore();
            }
        }
    },
    afterInit(chart, args, plugins) {
        const originalFit = chart.legend.fit;
        const margin = plugins.margin || 20;
        chart.legend.fit = function fit() {
            if (originalFit) {
                originalFit.call(this);
            }
            return this.height += margin;
        }
    }
};

function appendChartSummary(chart, wrapperId, applyColor) {
    const wrapper = document.getElementById(wrapperId);
    if (!wrapper) return;

    const oldSummary = wrapper.querySelector('.chart-summary');
    if (oldSummary) oldSummary.remove();

    const summary = document.createElement('div');
    summary.className = 'chart-summary';
    summary.style.display = 'none';

    const table = document.createElement('table');
    table.style.borderCollapse = 'collapse';
    table.style.width = '100%';
    table.border = '1';

    const thead = document.createElement('thead');
    const headRow = document.createElement('tr');

    const thFlux = document.createElement('th');
    thFlux.textContent = 'Flux';
    thFlux.style.padding = '4px';
    headRow.appendChild(thFlux);

    chart.data.labels.forEach(label => {
        const th = document.createElement('th');
        th.textContent = label;
        th.style.padding = '4px';
        th.style.fontWeight = 'bold';
        headRow.appendChild(th);
    });

    thead.appendChild(headRow);
    table.appendChild(thead);

    const tbody = document.createElement('tbody');

    chart.data.datasets.forEach(dataset => {
        const tr = document.createElement('tr');

        const tdFlux = document.createElement('td');
        tdFlux.textContent = dataset.label || 'Données';
        tdFlux.style.padding = '4px';
        if (applyColor) {
            tdFlux.style.color = dataset.backgroundColor || '#000';
        } else {
            tdFlux.style.color = '#000';
        }
        
        tdFlux.style.fontWeight = 'bold';
        tr.appendChild(tdFlux);

        chart.data.labels.forEach((label, index) => {
            const value = dataset.data[index];
            const tdValue = document.createElement('td');
            tdValue.style.padding = '4px';
            tdValue.style.textAlign = 'center';

            if (value != 0 && value != null) {
                tdValue.textContent = `${value.toFixed(2)} ${dataset.unit || ''}`;
            } else {
                tdValue.textContent = '-';
                tdValue.style.color = '#aaa';
            }

            tr.appendChild(tdValue);
        });

        tbody.appendChild(tr);
    });

    table.appendChild(tbody);
    summary.appendChild(table);
    wrapper.appendChild(summary);

    if (!document.getElementById('chart-summary-style')) {
        const style = document.createElement('style');
        style.id = 'chart-summary-style';
        style.textContent = `
            .chart-summary table tbody tr:nth-child(even) {
                background-color: #f2f2f2;
            }
            .chart-summary table th,
            .chart-summary table td {
                text-align: center;
            }
        `;
        document.head.appendChild(style);
    }
}


(function () {
    const buttons = document.querySelectorAll('.chart-selector');
    buttons.forEach(button => {
        button.addEventListener('click', selectChart);
        if (button.classList.contains('active')) {
            const chartId = button.getAttribute('data-chart');
            const chartElement = document.getElementById(chartId);
            chartElement.style.display = 'block';
            // Ensure to create the chart only if it doesn't already exist
            if (!chartElement.hasAttribute('data-chart-initialized')) {
                showLoading(chartElement);
                initChart(chartElement, chartId);
                chartElement.setAttribute('data-chart-initialized', 'true');
            }
        }
    });
})();

$("#searchPeriodConfirm").on('click', function () {
    let fromDateStr = $("#searchPeriodFrom").val();
    let [fromDateYear, fromDateMonth, fromDateDay] = fromDateStr.split('-');
    let fromDateShortYear = fromDateYear.slice(-2);
    let fromDate = `${fromDateDay}/${fromDateMonth}/${fromDateShortYear}`;

    let toDateStr = $("#searchPeriodTo").val();
    let [toDateYear, toDateMonth, toDateDay] = toDateStr.split('-');
    let toDateShortYear = toDateYear.slice(-2);
    let toDate = `${toDateDay}/${toDateMonth}/${toDateShortYear}`;

    let ia = $("#searchIA").val();

    let params = [];

    if (fromDate) {
        params.push("from=" + fromDate);
    }
    if (toDate) {
        params.push("to=" + toDate);
    }
    if (ia) {
        params.push("ia=" + ia.join("|"));
    }

    location.href = window.location.href.split('?')[0] + "?" + params.join("&");
})

function selectChart(event) {
    const chartId = event.target.getAttribute('data-chart');
    const container = event.target.closest('.chart-container');
    const charts = container.querySelectorAll('canvas');
    const buttons = container.querySelectorAll('.chart-selector');

    charts.forEach(chart => {
        if (chart.id === chartId) {
            chart.style.display = 'block';
            if (!chart.hasAttribute('data-chart-initialized')) {
                showLoading(chart);
                initChart(chart, chartId);
                chart.setAttribute('data-chart-initialized', 'true');
            }
        } else {
            chart.style.display = 'none';
        }
    });

    buttons.forEach(button => {
        if (button === event.target) {
            button.classList.add('active');
        } else {
            button.classList.remove('active');
        }
    });

    const printContainer = container.querySelector('.chart-print-container');
    if (printContainer) {
        const printButtons = printContainer.querySelectorAll('.print-chart');

        printButtons.forEach(printButton => {
            const targetCanvasId = printButton.getAttribute('data-canvas').replace('#', '');
            if (targetCanvasId === chartId) {
                printButton.style.display = 'block';
            } else {
                printButton.style.display = 'none';
            }
        });
    }
}


function showLoading(chart) {
    const loadingImage = document.createElement('img');
    loadingImage.src = '/img/loading/loader.svg';
    loadingImage.width = 100;
    loadingImage.height = 100;
    loadingImage.classList.add('myloader');
    chart.parentElement.style.position = 'relative';
    loadingImage.style.position = 'absolute';
    loadingImage.style.top = '100%';
    loadingImage.style.left = '50%';
    loadingImage.style.transform = 'translate(-50%, -50%)';
    chart.parentElement.appendChild(loadingImage);
}

function hideLoading(chart) {
    const loadingImage = chart.parentElement.querySelector('.myloader');
    if (loadingImage) {
        loadingImage.remove();
    }
}

function initChart(chart, chartId) {
    switch (chartId) {
        case "DechetsEvacuesChart1":
            hideLoading(chart);
            new Chart(chart, {
                ...data.dechetN1Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.dechetN1Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.dechetN1Mois, "DechetsEvacuesChart1-wrapper", true);
            break;
        case "DechetsEvacuesChart2":
            hideLoading(chart);
            new Chart(chart, {
                ...data.dechetN0Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.dechetN0Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.dechetN0Mois, "DechetsEvacuesChart2-wrapper", true);
            break;
        case "NombreDePassageChart1":
            hideLoading(chart);
            new Chart(chart, {
                ...data.passagesN1Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.passagesN1Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 0
                        }
                    }
                }
            });
            appendChartSummary(data.passagesN1Mois, "NombreDePassageChart1-wrapper", true);
            break;
        case "NombreDePassageChart2":
            hideLoading(chart);
            new Chart(chart, {
                ...data.passagesN0Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.passagesN0Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 0
                        }
                    }
                }
            });
            appendChartSummary(data.passagesN0Mois, "NombreDePassageChart2-wrapper", true);
            break;
        case "RapportCo2Chart1":
            hideLoading(chart);
            new Chart(chart, data.ecoTaxeN1Mois);
            appendChartSummary(data.ecoTaxeN1Mois, "RapportCo2Chart1-wrapper", true);
            break;
        case "RapportCo2Chart2":
            hideLoading(chart);
            new Chart(chart, data.ecoTaxeN0Mois);
            appendChartSummary(data.ecoTaxeN0Mois, "RapportCo2Chart2-wrapper", true);
            break;
        case "TauxDeValorisationChart1":
            hideLoading(chart);
            new Chart(chart, data.valorisationsN1Mois);
            appendChartSummary(data.valorisationsN1Mois, "TauxDeValorisationChart1-wrapper", false);
            break;
        case "TauxDeValorisationChart2":
            hideLoading(chart);
            new Chart(chart, data.valorisationsN0Mois);
            appendChartSummary(data.valorisationsN0Mois, "TauxDeValorisationChart2-wrapper", false);
            break;
        case "FacturationChart1":
            hideLoading(chart);
            new Chart(chart, {
                ...data.facturationN1Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.facturationN1Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.facturationN1Mois, "FacturationChart1-wrapper", false);
            break;
        case "FacturationChart2":
            hideLoading(chart);
            new Chart(chart, {
                ...data.facturationN0Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.facturationN0Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.facturationN0Mois, "FacturationChart2-wrapper", false);
            break;

        case "ServiceCostsChart1":
            hideLoading(chart);
            new Chart(chart, {
                ...data.serviceCostsN1Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.serviceCostsN1Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.serviceCostsN1Mois, "ServiceCostsChart1-wrapper", true);
            break;
        case "ServiceCostsChart2":
            hideLoading(chart);
            new Chart(chart, {
                ...data.serviceCostsN0Mois,
                plugins: [sumDataLabel],
                options: {
                    ...data.serviceCostsN0Mois.options,
                    plugins: {
                        sumDataLabel: {
                            digits: 2
                        }
                    }
                }
            });
            appendChartSummary(data.serviceCostsN0Mois, "ServiceCostsChart2-wrapper", true);
            break;
    }

}

document.querySelectorAll('.print-chart').forEach(button => {
    button.addEventListener('click', () => {
        const wrapperSelector = button.getAttribute('data-target');
        var wrapper = document.querySelector(wrapperSelector);
        const canvas = wrapper.querySelector('canvas');

        if (!canvas) {
            alert('Aucun graphique trouvé !');
            return;
        }

        let closestH3 = canvas.closest('div')?.querySelector('h3');
        if (!closestH3) {
            let parent = canvas.parentElement;
            while (parent) {
                closestH3 = parent.querySelector('h3');
                if (closestH3) break;
                parent = parent.parentElement;
            }
        }

        const title = closestH3 ? closestH3.textContent.trim() : 'Graphique';

        const chartSummary = canvas.parentElement.querySelector('.chart-summary');
        const summaryHTML = chartSummary ? chartSummary.innerHTML : '';

        const imgData = canvas.toDataURL('image/png');

        const printWindow = window.open('', '_blank');

        if (!printWindow) {
            alert('Impossible d\'ouvrir une fenêtre d\'impression. Veuillez autoriser les popups.');
            return;
        }

        printWindow.document.write(`
            <html>
            <head>
                <title>${title}</title>
                <style>
                    body { margin: 0; text-align: center; font-family: "Inter", sans-serif; color: '#656565';}
                    img { max-width: 100%; height: auto; }
                    @media print {
                        .chart-summary table tbody tr:nth-child(even) { background-color: #f2f2f2; }
                    }
                </style>
            </head>
            <body>
                <img src="${imgData}" alt="$${title}">
                <div class="chart-summary" style="justify-items: baseline; margin: 2em;">
                    ${summaryHTML}
                </div>
                <script>
                    window.onload = function() {
                        window.print();
                        setTimeout(() => { window.close(); }, 100);
                    };
                </script>
            </body>
            </html>
        `);

        printWindow.document.close();
    })
});

