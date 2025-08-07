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

using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Principal;

namespace Extranet.Models
{
    public static class User
    {
        public static Member? GetCurrentUser(this IPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            string? cryptedValue = ((ClaimsIdentity)user.Identity).FindFirst(Member.meInTheVoid)?.Value;

            string? decryptedValue = cryptedValue != null
                ? DESHelper.DecryptFromBase64String(cryptedValue)
                : null;

            return decryptedValue != null
                ? JsonConvert.DeserializeObject<Member>(decryptedValue)
                : null;
        }
    }
}
