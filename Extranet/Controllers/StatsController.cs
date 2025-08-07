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
using Extranet.Models;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Extranet.Models.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class StatsController(ILogger<Controller> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        public async Task<IActionResult> Statistiques(string? from, string? to, string? ia)
        {
            DateHelper.VerifyInputDate(ref from, ref to, _mySettings.NavSettings.NavDateFormat);

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                DateHelper.GetFirstAndLastDayOfMonth(DateTime.Now, _mySettings.NavSettings.NavDateFormat, out string nowFromDate, out string nowToDate);
                DateHelper.GetFirstAndLastDayOfMonth(DateTime.Now.AddYears(-1), _mySettings.NavSettings.NavDateFormat, out string lastyearFromDate, out string lastyearToDate);
                ViewBag.fromDate = lastyearFromDate;
                ViewBag.toDate = nowToDate;
            }
            else
            {
                ViewBag.fromDate = from;
                ViewBag.toDate = to;
                if (string.IsNullOrWhiteSpace(ia))
                {
                    ViewBag.ia = null;
                }
                else
                {
                    ViewBag.ia = ia;
                }
            }

            ViewBag.fromDateDT = DateTime.ParseExact(ViewBag.fromDate, _mySettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture);
            ViewBag.toDateDT = DateTime.ParseExact(ViewBag.toDate, _mySettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture);

            AllStatistics allStatistics = await GetStatsData(ViewBag.fromDate, ViewBag.toDate, ViewBag.ia, ViewBag.User, _mySettings);
            ViewBag.AllStatistics = allStatistics;
            ViewBag.codesAdresseInter = allStatistics.GetAdressesIntervention();

            return View();
        }

        public static async Task<AllStatistics> GetStatsData(string fromDate, string toDate, string? ia, Member user, WebSettings webSettings, bool home = false)
        {
            var dataprovider = new DataProvider(webSettings, user);

            var dtFromDate = DateTime.ParseExact(fromDate, webSettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture);
            var dtToDate = DateTime.ParseExact(toDate, webSettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture);

            var prevDtFromDate = home ? dtFromDate.AddMonths(-1) : dtFromDate.AddYears(-1);
            var prevDtToDate = home ? dtToDate.AddMonths(-1) : dtToDate.AddYears(-1);
            string prevFromDate;
            string prevToDate;
            if (home)
                DateHelper.GetFirstAndLastDayOfMonth(prevDtFromDate, webSettings.NavSettings.NavDateFormat, out prevFromDate, out prevToDate);
            else
            {
                prevFromDate = prevDtFromDate.ToString(webSettings.NavSettings.NavDateFormat);
                prevToDate = prevDtToDate.ToString(webSettings.NavSettings.NavDateFormat);
            }

            var prevGetTbaCAInvoice = dataprovider.GetSalesInvoiceList(prevFromDate, prevToDate);
            var GetTbaCAInvoice = dataprovider.GetSalesInvoiceList(fromDate, toDate);

            var prevGetTbaCACrMemo = dataprovider.GetSalesCreditMemoList(prevFromDate, prevToDate);
            var GetTbaCACrMemo = dataprovider.GetSalesCreditMemoList(fromDate, toDate);

            var prevGetTbaValorisationMatiere = dataprovider.GetStatistics(prevFromDate, prevToDate, StatTypes.Valorisation);
            var GetTbaValorisationMatiere = dataprovider.GetStatistics(fromDate, toDate, StatTypes.Valorisation);

            var prevGetTbaDechetsEvacues = dataprovider.GetStatistics(prevFromDate, prevToDate, StatTypes.Dechet);
            var GetTbaDechetsEvacues = dataprovider.GetStatistics(fromDate, toDate, StatTypes.Dechet);

            var prevGetNbPassage = dataprovider.GetStatistics(prevFromDate, prevToDate, StatTypes.Passage);
            var GetNbPassage = dataprovider.GetStatistics(fromDate, toDate, StatTypes.Passage);
            var prevGetTbaNbPassage = dataprovider.GetNbPassage(prevFromDate, prevToDate);
            var GetTbaNbPassage = dataprovider.GetNbPassage(fromDate, toDate);

            var prevGetEcoTaxe = dataprovider.GetStatistics(prevFromDate, prevToDate, StatTypes.EcoTaxe);
            var GetEcoTaxe = dataprovider.GetStatistics(fromDate, toDate, StatTypes.EcoTaxe);

            var prevGetDetailsOfServiceCosts = dataprovider.GetDetailsOfServiceCosts(prevFromDate, prevToDate);
            var GetDetailsOfServiceCosts = dataprovider.GetDetailsOfServiceCosts(fromDate, toDate);

            List<Task> tasks = new List<Task>() {             
                // periode en cours
                GetTbaDechetsEvacues,
                GetTbaNbPassage,
                GetNbPassage,
                GetTbaValorisationMatiere,
                GetTbaCAInvoice,
                GetTbaCACrMemo,
                GetEcoTaxe,
                GetDetailsOfServiceCosts,
                // période précédente
                prevGetTbaDechetsEvacues,
                prevGetTbaNbPassage,
                prevGetNbPassage,
                prevGetTbaValorisationMatiere,
                prevGetTbaCACrMemo,
                prevGetTbaCAInvoice,
                prevGetEcoTaxe,
                prevGetDetailsOfServiceCosts
            };

            await Task.WhenAll(tasks);

            AllStatistics allStatistics = new()
            {
                fromDate = DateTime.ParseExact(fromDate, webSettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture),
                toDate = DateTime.ParseExact(toDate, webSettings.NavSettings.NavDateFormat, CultureInfo.InvariantCulture),
                CodeAdresseIntervention = (string.IsNullOrWhiteSpace(ia)) ? null : ia,

                SalesInvoiceHeadersN0 = (SalesInvoiceHeader[])prevGetTbaCAInvoice?.Result?.Data,
                SalesInvoiceHeadersN1 = (SalesInvoiceHeader[])GetTbaCAInvoice?.Result?.Data,

                SalesCrMemoHeadersN0 = (SalesCrMemoHeader[])prevGetTbaCACrMemo?.Result?.Data,
                SalesCrMemoHeadersN1 = (SalesCrMemoHeader[])GetTbaCACrMemo?.Result?.Data,

                ValorisationsN0 = prevGetTbaValorisationMatiere?.Result?.Data?.statisticList?.ExtranetStatistics,
                ValorisationsN1 = GetTbaValorisationMatiere?.Result?.Data?.statisticList?.ExtranetStatistics,

                DechetsN0 = prevGetTbaDechetsEvacues.Result?.Data?.statisticList?.ExtranetStatistics,
                DechetsN1 = GetTbaDechetsEvacues?.Result?.Data?.statisticList?.ExtranetStatistics,

                PassagesN0 = prevGetNbPassage?.Result?.Data?.statisticList?.ExtranetStatistics,
                PassagesN1 = GetNbPassage?.Result?.Data?.statisticList?.ExtranetStatistics,

                TbaPassagesN0 = prevGetTbaNbPassage?.Result?.Data?.return_value ?? 0,
                TbaPassagesN1 = GetTbaNbPassage?.Result?.Data?.return_value ?? 0,

                EcoTaxeN0 = prevGetEcoTaxe?.Result?.Data?.statisticList?.ExtranetStatistics,
                EcoTaxeN1 = GetEcoTaxe?.Result?.Data?.statisticList?.ExtranetStatistics,

                DetailsOfServiceCostsN0 = prevGetDetailsOfServiceCosts?.Result?.Data?.detailsServiceCosts?.InvoiceLine,
                DetailsOfServiceCostsN1 = GetDetailsOfServiceCosts?.Result?.Data?.detailsServiceCosts?.InvoiceLine,
            };

            return allStatistics;
        }

        public async Task<IActionResult> DechetsEvacuesChart1()
        {
            return View();
        }

        public async Task<IActionResult> DechetsEvacuesChart2()
        {
            return View();
        }

        [HttpGet]
        public async Task<object> ApercuStats(string fromDate, string toDate)
        {
            var allstats = ViewBag.AllStatistics;

            return null;//stats;
        }
    }
}
