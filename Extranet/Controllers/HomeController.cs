using ECManagementWS;
using Extranet.Models;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.News;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class HomeController(ILogger<HomeController> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        public async Task<IActionResult> Accueil()
        {
            ViewBag.Actualites = Actualite.GetActualites(_mySettings);

            //récupérer les dates de clotures pour fromDate et toDate
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            var accountingPeriod = await dataProvider.GetLastAccountingPeriod();

            DateHelper.GetFirstAndLastDayOfMonth(DateTime.Now, _mySettings.NavSettings.NavDateFormat, out string nowFromDate, out string nowToDate);
            ViewBag.fromDate = accountingPeriod?.Data?.startingDate ?? nowFromDate;
            ViewBag.toDate = accountingPeriod?.Data?.endingDate ?? nowToDate;

            ViewBag.AllStatistics = await StatsController.GetStatsData(ViewBag.fromDate, ViewBag.toDate, null, ViewBag.User, _mySettings, true);
            return View();
        }

        [HttpPost]
        public async Task<bool> ChangeCompany([FromBody] string newCompany)
        {
            if (!string.IsNullOrWhiteSpace(newCompany))
            {
                await Member.SetCurrentCompany(HttpContext, newCompany);
                return true;
            }

            return false;
        }

        [HttpPost]
        public async Task<bool> ChangeAccount([FromBody] string newAccountNo)
        {
            if (!string.IsNullOrWhiteSpace(newAccountNo))
            {
                await Member.SetCurrentAccount(HttpContext, newAccountNo);
                return true;
            }

            return false;
        }

        [MemberGuard]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
