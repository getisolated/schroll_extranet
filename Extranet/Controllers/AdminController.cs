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

using Extranet.Models.Guards;
using Extranet.Models.News;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Extranet.Controllers
{
    [AdminGuard]
    public class AdminController(ILogger<HomeController> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        public async Task<IActionResult> News()
        {
            var realm = SchrollRealmConfig.GetNewRealmInstance(_mySettings);

            ViewBag.Actualites = Actualite.GetActualites(_mySettings);

            return View();
        }

        [HttpPost]
        public async Task<bool> CreateNews([FromBody] AjaxActualite actualite)
        {
            if (actualite != null &&
                !string.IsNullOrWhiteSpace(actualite.Description) &&
                !string.IsNullOrWhiteSpace(actualite.myfile))
            {
                var realm = SchrollRealmConfig.GetNewRealmInstance(_mySettings);
                realm.Write(() =>
                {
                    realm.Add(new Actualite()
                    {
                        Description = actualite.Description,
                        Image = actualite.myfile,
                        Link = actualite.Link ?? "#",
                        LinkTitle = actualite.LinkTitle ?? "Voir plus",
                        Date = DateTimeOffset.Now
                    });
                });
                return true;
            }

            return false;
        }

        [HttpPost]
        public async Task<bool> DeleteNews([FromBody] AjaxActualite actualite)
        {
            if (actualite != null &&
                !string.IsNullOrWhiteSpace(actualite.Id))
            {
                var realm = SchrollRealmConfig.GetNewRealmInstance(_mySettings);
                realm.Write(() =>
                {
                    realm.Remove(realm.Find<Actualite>(actualite.Id));
                });
                return true;
            }

            return false;
        }
    }
}
