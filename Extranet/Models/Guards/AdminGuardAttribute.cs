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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Extranet.Models.Guards
{
    public class AdminGuardAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User.GetCurrentUser();

            if (user == null)
            {
                // User is not authenticated, redirect to Login action
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
            else if (user.IsAdmin)
            {
                context.HttpContext.Items["User"] = user;
            }
            else
            {
                context.HttpContext.Items["User"] = user;
                context.Result = new RedirectToActionResult("Accueil", "Home", null);
            }
        }
    }
}