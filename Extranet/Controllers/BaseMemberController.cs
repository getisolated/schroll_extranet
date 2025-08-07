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
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Extranet.Controllers
{
    public class BaseMemberController(ILogger<Controller> logger, IOptions<WebSettings> values) : Controller
    {
        protected readonly ILogger<Controller> _logger = logger;
        protected readonly WebSettings _mySettings = values.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ViewBag.User = context.HttpContext.Items["User"] as Member;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);

            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            
            var tskMaintenance = dataProvider.GetMaintenanceInProgress();
            var tskNewDocToRead = dataProvider.GetNewDocsToRead();

            await Task.WhenAll(tskMaintenance, tskNewDocToRead);

            ViewBag.Maintenance = tskMaintenance.Result;
            ViewBag.NewDocToRead = tskNewDocToRead.Result;
        }
    }
}
