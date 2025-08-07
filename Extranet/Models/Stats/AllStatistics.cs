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

using ECManagementWS;
using System.Globalization;

namespace Extranet.Models.Stats
{
    public class AllStatistics
    {
        //FIltres
        public DateTime fromDate { get; set; }

        //FIltres
        public DateTime toDate { get; set; }

        //FIltres
        public string CodeAdresseIntervention { get; set; }

        public List<string> CodesAdresseIntervention { get { return GetAdressesIntervention(); } }


        public SalesInvoiceHeader[] SalesInvoiceHeadersN0 { get; set; }
        public SalesInvoiceHeader[] SalesInvoiceHeadersN1 { get; set; }

        public SalesCrMemoHeader[] SalesCrMemoHeadersN0 { get; set; }
        public SalesCrMemoHeader[] SalesCrMemoHeadersN1 { get; set; }

        public ExtranetStatistics[] DechetsN0 { get; set; }
        public ExtranetStatistics[] DechetsN1 { get; set; }

        public ExtranetStatistics[] ValorisationsN0 { get; set; }
        public ExtranetStatistics[] ValorisationsN1 { get; set; }

        public ExtranetStatistics[] PassagesN0 { get; set; }
        public ExtranetStatistics[] PassagesN1 { get; set; }

        public int TbaPassagesN0 { get; set; }
        public int TbaPassagesN1 { get; set; }

        public ExtranetStatistics[] EcoTaxeN0 { get; set; }
        public ExtranetStatistics[] EcoTaxeN1 { get; set; }

        public InvoiceLine[] DetailsOfServiceCostsN0 { get; set; }
        public InvoiceLine[] DetailsOfServiceCostsN1 { get; set; }

        #region Vignettes 

        public decimal TbaCA
        {
            get
            {
                return SumCADocuments(SalesInvoiceHeadersN1, SalesCrMemoHeadersN1);
            }
        }
        public decimal prevTbaCA
        {
            get
            {
                return SumCADocuments(SalesInvoiceHeadersN0, SalesCrMemoHeadersN0);
            }
        }

        public decimal TbaValorisationMatiere
        {
            get
            {
                return GetPourcentValorisationMatiere(ValorisationsN1);
            }
        }
        public decimal prevTbaValorisationMatiere
        {
            get
            {
                return GetPourcentValorisationMatiere(ValorisationsN0);
            }
        }

        public decimal TbaDechetsEvacues
        {
            get
            {
                return DechetsN1?.Sum(x => decimal.Parse(x.Weight)) ?? 0;
            }
        }
        public decimal prevTbaDechetsEvacues
        {
            get
            {
                return DechetsN0?.Sum(x => decimal.Parse(x.Weight)) ?? 0;
            }
        }

        public decimal TbaNbPassage
        {
            get
            {
                return TbaPassagesN1;
            }
        }
        public decimal prevTbaNbPassage
        {
            get
            {
                return TbaPassagesN0;
            }
        }

        private decimal SumCADocuments(SalesInvoiceHeader[] invoices, SalesCrMemoHeader[] crmemo)
        {
            var sumInvoices = invoices?.Sum(x => x.Amount) ?? 0;
            var sumCrMemo = crmemo?.Sum(x => x.Amount) ?? 0;

            return Math.Round(sumInvoices - sumCrMemo, 2, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Charts

        public Chart DechetN0Mois { get { return GetStackedBarChart(DechetsN0, StatTypes.Dechet, true); } }
        public Chart DechetN1Mois { get { return GetStackedBarChart(DechetsN1, StatTypes.Dechet); } }

        public Chart PassagesN0Mois { get { return GetStackedBarChart(PassagesN0, StatTypes.Passage, true); } }
        public Chart PassagesN1Mois { get { return GetStackedBarChart(PassagesN1, StatTypes.Passage); } }

        public Chart EcoTaxeN0Mois { get { return GetStackedBarChart(EcoTaxeN0, StatTypes.EcoTaxe, true); } }
        public Chart EcoTaxeN1Mois { get { return GetStackedBarChart(EcoTaxeN1, StatTypes.EcoTaxe); } }

        public Chart FacturationN0Mois { get { return GetFacturationChart(SalesInvoiceHeadersN0, SalesCrMemoHeadersN0, true); } }
        public Chart FacturationN1Mois { get { return GetFacturationChart(SalesInvoiceHeadersN1, SalesCrMemoHeadersN1); } }

        public Chart ServiceCostsN0Mois { get { return GetServiceCostsChart(DetailsOfServiceCostsN0, true); } }
        public Chart ServiceCostsN1Mois { get { return GetServiceCostsChart(DetailsOfServiceCostsN1); } }

        private Chart GetStackedBarChart(ExtranetStatistics[] filteredList, StatTypes statTypes, bool previousPeriod = false)
        {
            var months = GetMonths(previousPeriod);

            if (!string.IsNullOrWhiteSpace(CodeAdresseIntervention))
            {
                var arrCodeAdresseIntervention = CodeAdresseIntervention.Split('|');
                filteredList = filteredList?.Where(x => arrCodeAdresseIntervention.Contains(x.IA))?.ToArray();
            }
            else
                filteredList = filteredList?.ToArray();

            if (filteredList == null || filteredList.Count() == 0 || string.IsNullOrWhiteSpace(filteredList?.FirstOrDefault()?.MyDate)) return null;

            List<ChartDataSet> dataSets = new List<ChartDataSet>();
            foreach (var stat in filteredList)
            {
                DateTime dt = DateTime.ParseExact(stat.MyDate.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture);
                var monthSearch = dt.ToString("MMM yy");
                var monthIndex = months.FindIndex(x => x == monthSearch);

                string label = string.IsNullOrWhiteSpace(stat.MatterSubFamilyLabel) ? (string.IsNullOrWhiteSpace(stat.MatterSubFamilyCode) ? stat.MatterNo : stat.MatterSubFamilyCode) : stat.MatterSubFamilyLabel;
                var matiereInDataset = dataSets.Where(x => x.label == label)?.FirstOrDefault();

                double val = 0;
                switch (statTypes)
                {
                    case StatTypes.Dechet:
                        double.TryParse(stat.Weight, out val);
                        break;
                    case StatTypes.EcoTaxe:
                        double.TryParse(stat.EcoTaxeCO2, out val);
                        break;
                    case StatTypes.Passage:
                        val += (double)stat.NoOfVisits;
                        break;
                }

                if (monthIndex >= 0)
                    if (matiereInDataset == null)
                    {
                        string color = GetColorFromText(label);
                        var valeurs = new double[months.Count];
                        valeurs[monthIndex] += val;
                        dataSets.Add(new()
                        {
                            data = valeurs,
                            backgroundColor = color,
                            borderColor = color,
                            borderWidth = 1,
                            label = label,
                        });
                    }
                    else
                    {
                        var valeurs = matiereInDataset.data;
                        valeurs[monthIndex] += val;
                        matiereInDataset.data = valeurs;
                    }
            }

            dataSets = dataSets?.OrderBy(ds => ds.label)?.ToList();

            Chart chart = new Chart("bar", new() { datasets = dataSets, labels = months }, true, true, true);

            return chart;
        }

        public Chart GetFacturationChart(SalesInvoiceHeader[] invoices, SalesCrMemoHeader[] crMemos, bool previousPeriod = false)
        {
            var months = GetMonths(previousPeriod);

            if (!string.IsNullOrWhiteSpace(CodeAdresseIntervention))
            {
                var arrCodeAdresseIntervention = CodeAdresseIntervention.Split('|');
                invoices = invoices?.Where(x => arrCodeAdresseIntervention.Contains(x.IA.Split(" - ")[0]))?.ToArray();
                crMemos = crMemos?.Where(x => arrCodeAdresseIntervention.Contains(x.IA.Split(" - ")[0]))?.ToArray();
            }

            if (invoices == null && crMemos == null) return null;

            var facturationMensuelle = new Dictionary<string, decimal>();

            foreach (var month in months)
            {
                facturationMensuelle.Add(month, 0);
            }

            if (invoices != null)
                foreach (var invoice in invoices)
                {
                    DateTime dt = DateTime.ParseExact(invoice.PostingDate.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture);
                    var monthKey = dt.ToString("MMM yy");

                    if (!facturationMensuelle.ContainsKey(monthKey))
                        facturationMensuelle[monthKey] = 0;
                    else
                        facturationMensuelle[monthKey] += invoice.Amount;
                }

            if (crMemos != null)
                foreach (var crMemo in crMemos)
                {
                    DateTime dt = DateTime.ParseExact(crMemo.PostingDate.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture);
                    var monthKey = dt.ToString("MMM yy");

                    if (!facturationMensuelle.ContainsKey(monthKey))
                        facturationMensuelle[monthKey] = 0;
                    else
                        facturationMensuelle[monthKey] -= crMemo.Amount;
                }

            var valeurs = new double[months.Count];
            foreach (var mois in facturationMensuelle)
            {
                var monthIndex = months.FindIndex(x => x == mois.Key);
                if (monthIndex >= 0)
                    valeurs[monthIndex] = (double)mois.Value;
            }

            var dataSets = new List<ChartDataSet>{
                new ChartDataSet
                {
                    data = valeurs,
                    label = "€"
                }
            };

            Chart chart = new Chart("bar", new ChartData { datasets = dataSets, labels = months }, true, true, true);

            return chart;
        }

        public Chart GetServiceCostsChart(InvoiceLine[] filteredList, bool previousPeriod = false)
        {
            var months = GetMonths(previousPeriod);

            if (!string.IsNullOrWhiteSpace(CodeAdresseIntervention))
            {
                var arrCodeAdresseIntervention = CodeAdresseIntervention.Split('|');
                filteredList = filteredList?.Where(x => arrCodeAdresseIntervention.Contains(x.IA))?.ToArray();
            }
            else
                filteredList = filteredList?.ToArray();

            if (filteredList == null || filteredList.Count() == 0 || string.IsNullOrWhiteSpace(filteredList?.FirstOrDefault()?.PostingDate)) return null;

            List<ChartDataSet> dataSets = new List<ChartDataSet>();
            foreach (var stat in filteredList)
            {
                DateTime dt = DateTime.ParseExact(stat.PostingDate.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture);
                var monthSearch = dt.ToString("MMM yy");
                var monthIndex = months.FindIndex(x => x == monthSearch);

                string label = string.IsNullOrWhiteSpace(stat.SubFamilyDesc) ? stat.SubFamily : stat.SubFamilyDesc;
                var matiereInDataset = dataSets.Where(x => x.label == label)?.FirstOrDefault();

                double val = (double)stat.Amount;

                if (monthIndex >= 0)
                    if (matiereInDataset == null)
                    {
                        string color = GetColorFromText(label);
                        var valeurs = new double[months.Count];
                        valeurs[monthIndex] += val;
                        dataSets.Add(new()
                        {
                            data = valeurs,
                            backgroundColor = color,
                            borderColor = color,
                            borderWidth = 1,
                            label = label,
                        });
                    }
                    else
                    {
                        var valeurs = matiereInDataset.data;
                        valeurs[monthIndex] += val;
                        matiereInDataset.data = valeurs;
                    }
            }

            dataSets = dataSets?.OrderBy(ds => ds.label)?.ToList();

            Chart chart = new Chart("bar", new() { datasets = dataSets, labels = months }, true, true, true);

            return chart;
        }

        public Chart ValorisationsN0Mois { get { return GetValorisationChart(ValorisationsN0); } }
        public Chart ValorisationsN1Mois { get { return GetValorisationChart(ValorisationsN1); } }

        private Chart GetValorisationChart(ExtranetStatistics[] valorisations)
        {
            if (!string.IsNullOrWhiteSpace(CodeAdresseIntervention))
            {
                var arrCodeAdresseIntervention = CodeAdresseIntervention.Split('|');
                valorisations = valorisations?.Where(x => arrCodeAdresseIntervention.Contains(x.IA))?.ToArray();
            }

            if (valorisations == null || valorisations.Count() == 0 || string.IsNullOrWhiteSpace(valorisations?.FirstOrDefault()?.MyDate)) return null;

            List<ChartDataSet> dataSet = [];
            List<string> labels = [];
            List<double> valeurs = [];
            List<string> colors = [];

            var groupedValorisations = valorisations.GroupBy(v => new { v.TreatmentMode, v.TreatmentModeDescription }).OrderBy(g => g.Key.TreatmentMode);

            foreach (var group in groupedValorisations)
            {
                string label = string.IsNullOrWhiteSpace(group.Key.TreatmentModeDescription)
                            ? group.Key.TreatmentMode
                            : group.Key.TreatmentModeDescription;

                double sum = group.Sum(v => double.TryParse(v.TauxValoTraitement, out double val) ? val : 0);

                colors.Add(GetColorFromText(label));
                valeurs.Add(sum);
                labels.Add(label);
            }

            List<double> valeursPourcentage = [];
            var total = valeurs.Sum();
            foreach (var val in valeurs)
            {
                valeursPourcentage.Add(Math.Round(val * 100 / total, 2, MidpointRounding.AwayFromZero));
            }

            var dataSets = new List<ChartDataSet>{
                new() {
                    data = valeursPourcentage.ToArray(),
                    label = "%",
                    backgroundColor = colors,
                    borderColor = colors,
                    borderWidth = 1,
                }
            };

            Chart chart = new("pie", new() { datasets = dataSets, labels = labels }, false, false, false);

            return chart;
        }

        private decimal GetPourcentValorisationMatiere(ExtranetStatistics[] valorisations)
        {
            if (valorisations == null || valorisations.Count() == 0 || string.IsNullOrWhiteSpace(valorisations?.FirstOrDefault()?.MyDate)) return 0;

            List<Tuple<string, double>> valeurs = [];

            var groupedValorisations = valorisations.GroupBy(v => new { v.TreatmentMode, v.TreatmentModeDescription }).OrderBy(g => g.Key.TreatmentMode);

            foreach (var group in groupedValorisations)
            {
                string label = group.Key.TreatmentMode;

                double sum = group.Sum(v => double.TryParse(v.TauxValoTraitement, out double val) ? val : 0);

                valeurs.Add(new Tuple<string, double>(label, sum));
            }

            try
            {
                List<Tuple<string, double>> valeursPourcentage = [];
                var total = valeurs.Sum(x => x.Item2);

                var valoMat = valeurs.Where(x => x.Item1 == "VM");
                var val = valoMat?.First()?.Item2 ?? 0;

                return (decimal)Math.Round(val * 100 / total, 2, MidpointRounding.AwayFromZero);
            }
            catch
            {
                return 0;
            }
        }

        private static readonly Random rand = new Random();

        public string GetColorFromText(string input)
        {
            // Si l'input est null ou vide, retourner une couleur par défaut
            if (string.IsNullOrWhiteSpace(input))
                return "rgba(0,0,0,1)"; // Noir par défaut

            // Créer un hash du texte
            int hash = input.GetHashCode();

            // Extraire les composants de couleur en utilisant les bits du hash
            int r = (hash & 0xFF0000) >> 16; // Les 8 premiers bits
            int g = (hash & 0x00FF00) >> 8;  // Les 8 bits suivants
            int b = (hash & 0x0000FF);       // Les 8 derniers bits

            // Si la couleur est trop sombre ou trop claire, on ajuste pour une meilleure visibilité
            if (r + g + b < 100)
            {
                r = (r + 128) % 256;
                g = (g + 128) % 256;
                b = (b + 128) % 256;
            }

            // Retourner la couleur au format rgba
            return $"rgba({r},{g},{b},1)";
        }

        /// <summary>
        /// Liste des mois entre les 2 dates sélectionnées
        /// </summary>
        /// <returns></returns>
        public List<string> GetMonths(bool previousPeriod)
        {
            if (previousPeriod)
                return Enumerable.Range(0, Int32.MaxValue)
                     .Select(e => fromDate.AddYears(-1).AddMonths(e))
                     .TakeWhile(e => e <= toDate.AddYears(-1))
                     .Select(e => e.ToString("MMM yy")).ToList();
            else
                return Enumerable.Range(0, Int32.MaxValue)
                     .Select(e => fromDate.AddMonths(e))
                     .TakeWhile(e => e <= toDate)
                     .Select(e => e.ToString("MMM yy")).ToList();
        }

        #endregion

        public List<string> GetAdressesIntervention()
        {
            List<string> adresses = [
                .. DechetsN0?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. DechetsN1?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. ValorisationsN0?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. ValorisationsN1?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. EcoTaxeN0?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. EcoTaxeN1?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. PassagesN0?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. PassagesN1?.Select(x => x.IALabel)?.Where(x=>x != "" && x != null) ?? [],
                .. SalesInvoiceHeadersN0?.Select(x => x.IA)?.Where(x=>x != "" && x != null) ?? [],
                .. SalesInvoiceHeadersN1?.Select(x => x.IA)?.Where(x=>x != "" && x != null) ?? [],
                .. SalesCrMemoHeadersN0?.Select(x => x.IA)?.Where(x=>x != "" && x != null) ?? [],
                .. SalesCrMemoHeadersN1?.Select(x => x.IA)?.Where(x=>x != "" && x != null) ?? []];

            var distinctAddr = adresses?.Distinct()?.ToList();

            foreach (var adr in adresses?.Distinct()?.ToList())
            {
                if (adr != null && distinctAddr.Where(x => x.Contains(adr)).Count() > 1)
                {
                    distinctAddr.Remove(adr);
                }
            }

            return distinctAddr?.Order()?.ToList();
        }
    }
}
