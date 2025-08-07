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

using Extranet.Models;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class ComptaController(ILogger<Controller> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        public async Task<IActionResult> Factures(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetSalesInvoiceList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> Avoirs(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetSalesCreditMemoList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> Decomptes_achat(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetPurchaseStatementList(fromDate, toDate);

            return View();
        }
    }
}
