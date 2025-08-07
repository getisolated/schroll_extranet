// <copyrights>
// Ce programme est la propriété de la société Cap Vision (capvision.fr).
// Tous droits réservés.
// Ce programme est protégé par les lois sur les droits d''auteur en vigueur en France
// et dans d''autres pays. Toute reproduction, modification, distribution ou utilisation
// sans autorisation préalable est strictement interdite.
//
// This program is the property of Cap Vision company (capvision.fr).
// All rights reserved.
// This program is protected by copyright laws in force in France
// and other countries. Any reproduction, modification, distribution or use
// without prior authorization is strictly prohibited.
// </copyrights>

using System.Dynamic;

namespace Extranet.Models.Stats
{
    public class Chart
    {
        public string type { get; set; }
        public ChartData data { get; set; }
        public dynamic options { get; set; }

        public Chart(string type, ChartData data, bool stackedX, bool stackedY, bool beginAtZero)
        {
            this.type = type;
            this.data = data;
            this.options = new ExpandoObject();
            options.x = new ExpandoObject();
            options.x.stacked = stackedX;
            options.y = new ExpandoObject();
            options.y.stacked = stackedY;
            options.y.beginAtZero = beginAtZero;
        }

        // Unused constructor
        public Chart(string type, List<string> labels, List<ChartDataSet> datasets, bool stackedX, bool stackedY, bool beginAtZero)
        {
            this.type = type;
            this.data = new ChartData(labels, datasets);
            this.options = new ExpandoObject();
            options.x = new ExpandoObject();
            options.x.stacked = stackedX;
            options.y = new ExpandoObject();
            options.y.stacked = stackedY;
            options.y.beginAtZero = beginAtZero;
        }
    }

    public class ChartData

    {
        public List<string> labels { get; set; }
        public List<ChartDataSet> datasets { get; set; }

        public ChartData()
        {
            labels = new List<string>();
            datasets = new List<ChartDataSet>();
        }

        public ChartData(List<string> labels, List<ChartDataSet> datasets)
        {
            this.labels = labels;
            this.datasets = datasets;
        }

    }

    public class ChartDataSet
    {
        public double[] data { get; set; }
        public string? label { get; set; }
        /// <summary>
        /// string: Quand le dataset ne représente qu'une seule donnée (bar, line...). exemple : "#9BD0F5"
        /// List<string>: Quand le dataset représente un jeu de données (pie). exemple ["#9BD0F5", "#FFB1C1", "#36A2EB"]
        /// </summary>
        public dynamic? backgroundColor { get; set; }
        /// <summary>
        /// string: Quand le dataset ne représente qu'une seule donnée (bar, line...). exemple : "#9BD0F5"
        /// List<string>: Quand le dataset représente un jeu de données (pie). exemple ["#9BD0F5", "#FFB1C1", "#36A2EB"]
        /// </summary>
        public dynamic? borderColor { get; set; }
        public double? borderWidth { get; set; }

        public ChartDataSet() { }

        public ChartDataSet(double[] data, string? label, dynamic? backgroundColor, dynamic? borderColor, double? borderWidth)
        {
            this.data = data;
            this.label = label;
            this.backgroundColor = backgroundColor;
            this.borderColor = borderColor;
            this.borderWidth = borderWidth;
        }

        public ChartDataSet(double[] data, string? label, string? backgroundColor)
        {
            this.data = data;
            this.label = label;
            this.backgroundColor = backgroundColor;
        }

        public ChartDataSet(double[] data, string? label, List<string>? backgroundColor, List<string>? borderColor, double? borderWidth)
        {
            this.data = data;
            this.label = label;
            this.backgroundColor = backgroundColor;
            this.borderColor = borderColor;
            this.borderWidth = borderWidth;
        }
    }
}
